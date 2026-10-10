using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Dịch vụ nhúng tiếp nhận Webhook biến động số dư ngân hàng qua HttpListener siêu nhẹ,
/// hỗ trợ xác thực chữ ký số HMAC-SHA256 và tự động kích hoạt đối soát hóa đơn.
/// </summary>
public class WebhookListenerService : IWebhookListenerService, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true
    };

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _filePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private HttpListener? _listener;
    private CancellationTokenSource? _cts;
    private Task? _listenerTask;
    private bool _isRunning;
    private int _activePort;
    private WebhookSettingsDto? _cachedSettings;

    /// <summary>
    /// Khởi tạo WebhookListenerService
    /// </summary>
    /// <param name="scopeFactory">Scope factory phân giải IPaymentReconciliationService độc lập cho từng request</param>
    /// <param name="customFilePath">Đường dẫn tệp cấu hình tùy chọn (phục vụ unit test)</param>
    public WebhookListenerService(IServiceScopeFactory scopeFactory, string? customFilePath = null)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _filePath = string.IsNullOrWhiteSpace(customFilePath)
            ? DatabasePathResolver.ResolveDatabasePath("webhooksettings.json")
            : customFilePath;
    }

    /// <inheritdoc />
    public bool IsRunning => _isRunning;

    /// <inheritdoc />
    public int ActivePort => _activePort;

    /// <inheritdoc />
    public async Task<WebhookSettingsDto> GetSettingsAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_cachedSettings != null)
            {
                return _cachedSettings;
            }

            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath);
                var settings = JsonSerializer.Deserialize<WebhookSettingsDto>(json, JsonOptions);
                if (settings != null)
                {
                    _cachedSettings = settings;
                    return _cachedSettings;
                }
            }

            _cachedSettings = new WebhookSettingsDto();
            return _cachedSettings;
        }
        catch
        {
            return new WebhookSettingsDto();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task<bool> SaveSettingsAsync(WebhookSettingsDto settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        await _semaphore.WaitAsync();
        try
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize(settings, JsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
            _cachedSettings = settings;

            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task<bool> StartAsync()
    {
        if (_isRunning)
        {
            return true;
        }

        var settings = await GetSettingsAsync();

        try
        {
            _listener = new HttpListener();

            var normalizedPath = settings.Path.Trim();
            if (!normalizedPath.StartsWith("/"))
            {
                normalizedPath = "/" + normalizedPath;
            }
            if (!normalizedPath.EndsWith("/"))
            {
                normalizedPath += "/";
            }

            _listener.Prefixes.Add($"http://localhost:{settings.Port}{normalizedPath}");
            _listener.Prefixes.Add($"http://127.0.0.1:{settings.Port}{normalizedPath}");

            _listener.Start();
            _isRunning = true;
            _activePort = settings.Port;
            _cts = new CancellationTokenSource();

            var token = _cts.Token;
            _listenerTask = Task.Run(() => ListenLoopAsync(token), token);

            return true;
        }
        catch
        {
            await StopAsync();
            return false;
        }
    }

    /// <inheritdoc />
    public Task StopAsync()
    {
        _isRunning = false;
        _activePort = 0;

        try
        {
            _cts?.Cancel();
            _listener?.Stop();
            _listener?.Close();
        }
        catch
        {
            // Bỏ qua lỗi giải phóng
        }
        finally
        {
            _listener = null;
            _cts?.Dispose();
            _cts = null;
        }

        return Task.CompletedTask;
    }

    private async Task ListenLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _listener?.IsListening == true)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                _ = Task.Run(() => HandleRequestAsync(context), cancellationToken);
            }
            catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        try
        {
            if (!context.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 405; // Method Not Allowed
                context.Response.Close();
                return;
            }

            using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding ?? Encoding.UTF8);
            var rawJson = await reader.ReadToEndAsync();

            var signature = context.Request.Headers["X-Signature"]
                ?? context.Request.Headers["X-Webhook-Signature"]
                ?? context.Request.Headers["X-PayOS-Signature"]
                ?? context.Request.Headers["X-Casso-Signature"]
                ?? context.Request.Headers["X-Token"]
                ?? context.Request.Headers["Authorization"]
                ?? string.Empty;

            var settings = await GetSettingsAsync();

            // 1. Kiểm tra xác thực chữ ký bảo mật
            if (!ValidateSignature(rawJson, signature, settings.SecretKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json; charset=utf-8";
                var errorBytes = Encoding.UTF8.GetBytes("{\"error\": \"Unauthorized: Invalid webhook signature or token\"}");
                context.Response.ContentLength64 = errorBytes.Length;
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                context.Response.Close();
                return;
            }

            // 2. Phân tích chuỗi JSON theo cổng thanh toán
            var payload = ParseIncomingPayload(rawJson, settings.Provider);
            if (payload == null)
            {
                context.Response.StatusCode = 400; // Bad Request
                context.Response.ContentType = "application/json; charset=utf-8";
                var badBytes = Encoding.UTF8.GetBytes("{\"error\": \"Bad Request: Could not parse webhook payload JSON\"}");
                context.Response.ContentLength64 = badBytes.Length;
                await context.Response.OutputStream.WriteAsync(badBytes, 0, badBytes.Length);
                context.Response.Close();
                return;
            }

            if (string.IsNullOrEmpty(payload.Signature))
            {
                payload.Signature = signature;
            }

            // 3. Phản hồi HTTP 200 OK ngay lập tức cho ngân hàng tránh timeout
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json; charset=utf-8";
            var successBytes = Encoding.UTF8.GetBytes("{\"success\": true, \"message\": \"Webhook received successfully\"}");
            context.Response.ContentLength64 = successBytes.Length;
            await context.Response.OutputStream.WriteAsync(successBytes, 0, successBytes.Length);
            context.Response.Close();

            // 4. Kích hoạt đối soát tự động ở background thread
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var reconciliationService = scope.ServiceProvider.GetRequiredService<IPaymentReconciliationService>();
                    await reconciliationService.ProcessTransactionAsync(payload);
                }
                catch
                {
                    // Lỗi background không làm crash ứng dụng
                }
            });
        }
        catch
        {
            try
            {
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
            catch
            {
                // Bỏ qua lỗi đóng response
            }
        }
    }

    /// <inheritdoc />
    public bool ValidateSignature(string payload, string signature, string secretKey)
    {
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            // Nếu hệ thống không cấu hình SecretKey -> Chấp nhận mọi request
            return true;
        }

        if (string.IsNullOrWhiteSpace(signature))
        {
            return false;
        }

        var cleanSignature = signature.Trim();
        var cleanSecret = secretKey.Trim();

        var sigUtf8 = Encoding.UTF8.GetBytes(cleanSignature);
        var secUtf8 = Encoding.UTF8.GetBytes(cleanSecret);

        // 1. So khớp trực tiếp dạng static token chống Timing Attack
        if (CryptographicOperations.FixedTimeEquals(sigUtf8, secUtf8))
        {
            return true;
        }

        // 2. So khớp chữ ký HMAC-SHA256 an toàn chống Timing Attack
        try
        {
            using var hmac = new HMACSHA256(secUtf8);
            var expectedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

            byte[] providedBytes;
            try
            {
                providedBytes = Convert.FromHexString(cleanSignature);
            }
            catch
            {
                return false;
            }

            return CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public WebhookPayloadDto? ParseIncomingPayload(string rawJson, string provider)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;
            var prov = (provider ?? "Generic").Trim().ToLowerInvariant();

            if (prov == "payos")
            {
                // Định dạng PayOS: { code, desc, data: { orderCode, amount, description, accountNumber, reference, transactionDateTime }, signature }
                if (root.TryGetProperty("data", out var dataElem) && dataElem.ValueKind == JsonValueKind.Object)
                {
                    var orderCode = dataElem.TryGetProperty("orderCode", out var oc) ? oc.ToString() : string.Empty;
                    var reference = dataElem.TryGetProperty("reference", out var rf) ? rf.GetString() : string.Empty;
                    var transactionId = !string.IsNullOrWhiteSpace(reference) ? reference : orderCode;

                    var amount = dataElem.TryGetProperty("amount", out var am) && am.TryGetDecimal(out var amt) ? amt : 0m;
                    var description = dataElem.TryGetProperty("description", out var ds) ? ds.GetString() ?? string.Empty : string.Empty;
                    var accountNumber = dataElem.TryGetProperty("accountNumber", out var an) ? an.GetString() ?? string.Empty : string.Empty;
                    var sig = root.TryGetProperty("signature", out var sg) ? sg.GetString() ?? string.Empty : string.Empty;

                    var date = DateTime.UtcNow;
                    if (dataElem.TryGetProperty("transactionDateTime", out var td) && DateTime.TryParse(td.GetString(), out var parsedDate))
                    {
                        date = parsedDate;
                    }

                    return new WebhookPayloadDto
                    {
                        Gateway = "PayOS",
                        TransactionId = transactionId,
                        Amount = amount,
                        Description = description,
                        AccountNumber = accountNumber,
                        TransactionDate = date,
                        Signature = sig,
                        RawData = rawJson
                    };
                }
            }
            else if (prov == "casso")
            {
                // Định dạng Casso: { error: 0, data: [ { id, tid, description, amount, when, subAccId, bankName } ] }
                if (root.TryGetProperty("data", out var dataElem) && dataElem.ValueKind == JsonValueKind.Array && dataElem.GetArrayLength() > 0)
                {
                    var item = dataElem[0];
                    var tid = item.TryGetProperty("tid", out var t) ? t.GetString() : string.Empty;
                    var id = item.TryGetProperty("id", out var i) ? i.ToString() : string.Empty;
                    var transactionId = !string.IsNullOrWhiteSpace(tid) ? tid : id;

                    var amount = item.TryGetProperty("amount", out var am) && am.TryGetDecimal(out var amt) ? amt : 0m;
                    var description = item.TryGetProperty("description", out var ds) ? ds.GetString() ?? string.Empty : string.Empty;
                    var subAcc = item.TryGetProperty("subAccId", out var sa) ? sa.GetString() ?? string.Empty : string.Empty;

                    var date = DateTime.UtcNow;
                    if (item.TryGetProperty("when", out var wh) && DateTime.TryParse(wh.GetString(), out var parsedDate))
                    {
                        date = parsedDate;
                    }

                    return new WebhookPayloadDto
                    {
                        Gateway = "Casso",
                        TransactionId = transactionId,
                        Amount = amount,
                        Description = description,
                        AccountNumber = subAcc,
                        TransactionDate = date,
                        RawData = rawJson
                    };
                }
            }

            // Generic fallback hoặc cấu trúc phẳng
            var genTxId = string.Empty;
            if (root.TryGetProperty("transactionId", out var txElem)) genTxId = txElem.GetString() ?? txElem.ToString();
            else if (root.TryGetProperty("id", out var idElem)) genTxId = idElem.ToString();
            else if (root.TryGetProperty("reference", out var refElem)) genTxId = refElem.GetString() ?? string.Empty;

            var genAmount = root.TryGetProperty("amount", out var gAm) && gAm.TryGetDecimal(out var gAmt) ? gAmt : 0m;
            var genDesc = root.TryGetProperty("description", out var gDs) ? gDs.GetString() ?? string.Empty : string.Empty;
            var genAcc = root.TryGetProperty("accountNumber", out var gAc) ? gAc.GetString() ?? string.Empty : string.Empty;
            var genBin = root.TryGetProperty("bankBin", out var gBn) ? gBn.GetString() ?? string.Empty : string.Empty;
            var genSig = root.TryGetProperty("signature", out var gSg) ? gSg.GetString() ?? string.Empty : string.Empty;

            var genDate = DateTime.UtcNow;
            if (root.TryGetProperty("transactionDate", out var gDt) && DateTime.TryParse(gDt.GetString(), out var parsedGenDate))
            {
                genDate = parsedGenDate;
            }

            return new WebhookPayloadDto
            {
                Gateway = "Generic",
                TransactionId = genTxId,
                Amount = genAmount,
                Description = genDesc,
                AccountNumber = genAcc,
                BankBin = genBin,
                TransactionDate = genDate,
                Signature = genSig,
                RawData = rawJson
            };
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PaymentReconciliationResultDto> TestWebhookAsync(WebhookPayloadDto testPayload)
    {
        if (testPayload == null)
        {
            throw new ArgumentNullException(nameof(testPayload));
        }

        using var scope = _scopeFactory.CreateScope();
        var reconciliationService = scope.ServiceProvider.GetRequiredService<IPaymentReconciliationService>();
        return await reconciliationService.ProcessTransactionAsync(testPayload);
    }

    public void Dispose()
    {
        StopAsync().GetAwaiter().GetResult();
        _semaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}
