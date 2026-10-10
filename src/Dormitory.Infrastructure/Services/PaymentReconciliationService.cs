using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Triển khai dịch vụ bóc tách cú pháp chuyển khoản và đối soát tự động hóa đơn qua Webhook ngân hàng
/// </summary>
public class PaymentReconciliationService : IPaymentReconciliationService
{
    private readonly IDormitoryDbContext _context;

    public PaymentReconciliationService(IDormitoryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public string? ExtractBillCode(string description, string prefix = "KTX")
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var escapedPrefix = Regex.Escape(prefix.Trim());
        // Biểu thức chính quy: Nhận diện tiền tố KTX (đứng đầu chuỗi hoặc sau khoảng trắng/kí tự phân cách),
        // theo sau bởi các ký tự phân cách tùy chọn (khoảng trắng, dấu gạch nối, hai chấm, gạch dưới),
        // rồi bóc tách mã hóa đơn gồm các ký tự chữ, số và dấu gạch nối/dưới.
        var pattern = $@"(?:^|\b|\s){escapedPrefix}[-_:\s]*([A-Za-z0-9]+(?:[-_][A-Za-z0-9]+)*)";
        var match = Regex.Match(description, pattern, RegexOptions.IgnoreCase);

        if (match.Success && match.Groups.Count > 1)
        {
            var code = match.Groups[1].Value.Trim();
            return string.IsNullOrEmpty(code) ? null : code;
        }

        return null;
    }

    /// <inheritdoc />
    public async Task<PaymentReconciliationResultDto> ProcessTransactionAsync(WebhookPayloadDto payload)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        // 1. Kiểm tra Idempotency - Không xử lý lặp lại nếu mã giao dịch ngân hàng đã tồn tại
        var existingTx = await _context.PaymentTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransactionId == payload.TransactionId);

        if (existingTx != null)
        {
            return new PaymentReconciliationResultDto
            {
                IsSuccess = false,
                Status = PaymentTransactionStatus.Duplicate,
                TransactionId = payload.TransactionId,
                BillId = existingTx.BillId,
                BillCode = existingTx.BillCode,
                Amount = existingTx.Amount,
                Message = $"Giao dịch {payload.TransactionId} đã tồn tại trong hệ thống (trùng lặp giao dịch)."
            };
        }

        // 2. Trích xuất mã hóa đơn từ nội dung chuyển khoản
        var billCode = ExtractBillCode(payload.Description);

        if (string.IsNullOrWhiteSpace(billCode))
        {
            var unmatchedTx = new PaymentTransaction
            {
                TransactionId = payload.TransactionId,
                BankBin = payload.BankBin,
                AccountNumber = payload.AccountNumber,
                Amount = payload.Amount,
                Description = payload.Description,
                TransactionDate = payload.TransactionDate,
                BillId = null,
                BillCode = string.Empty,
                Status = PaymentTransactionStatus.Unmatched,
                RawPayload = payload.RawData ?? string.Empty,
                Note = "Không tìm thấy mã hóa đơn hợp lệ trong nội dung chuyển khoản",
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(unmatchedTx);
            await _context.SaveChangesAsync();

            return new PaymentReconciliationResultDto
            {
                IsSuccess = false,
                Status = PaymentTransactionStatus.Unmatched,
                TransactionId = payload.TransactionId,
                Amount = payload.Amount,
                Message = "Không tìm thấy mã hóa đơn hợp lệ trong nội dung chuyển khoản."
            };
        }

        // 3. Tìm kiếm hóa đơn trong CSDL
        var bill = await _context.Bills
            .FirstOrDefaultAsync(b => b.BillCode.ToLower() == billCode.ToLower());

        if (bill == null)
        {
            var unmatchedTx = new PaymentTransaction
            {
                TransactionId = payload.TransactionId,
                BankBin = payload.BankBin,
                AccountNumber = payload.AccountNumber,
                Amount = payload.Amount,
                Description = payload.Description,
                TransactionDate = payload.TransactionDate,
                BillId = null,
                BillCode = billCode,
                Status = PaymentTransactionStatus.Unmatched,
                RawPayload = payload.RawData ?? string.Empty,
                Note = $"Mã hóa đơn '{billCode}' không tồn tại trong hệ thống",
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(unmatchedTx);
            await _context.SaveChangesAsync();

            return new PaymentReconciliationResultDto
            {
                IsSuccess = false,
                Status = PaymentTransactionStatus.Unmatched,
                TransactionId = payload.TransactionId,
                BillCode = billCode,
                Amount = payload.Amount,
                Message = $"Mã hóa đơn '{billCode}' không tồn tại trong hệ thống."
            };
        }

        // 4. Đối soát số tiền thanh toán
        var totalAmount = bill.TotalAmount;
        if (payload.Amount >= totalAmount)
        {
            // Thanh toán đủ hoặc thừa tiền -> Gạch nợ hóa đơn thành công
            bill.Status = BillStatus.Paid;
            bill.PaidDate = DateTime.UtcNow;

            string note;
            if (payload.Amount > totalAmount)
            {
                var overpaid = payload.Amount - totalAmount;
                note = $"Đối soát tự động thành công (Nộp thừa {overpaid:N0} VNĐ)";
            }
            else
            {
                note = "Đối soát tự động thành công (Thanh toán đủ)";
            }

            var successTx = new PaymentTransaction
            {
                TransactionId = payload.TransactionId,
                BankBin = payload.BankBin,
                AccountNumber = payload.AccountNumber,
                Amount = payload.Amount,
                Description = payload.Description,
                TransactionDate = payload.TransactionDate,
                BillId = bill.Id,
                BillCode = bill.BillCode,
                Status = PaymentTransactionStatus.Success,
                RawPayload = payload.RawData ?? string.Empty,
                Note = note,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(successTx);
            await _context.SaveChangesAsync();

            return new PaymentReconciliationResultDto
            {
                IsSuccess = true,
                Status = PaymentTransactionStatus.Success,
                TransactionId = payload.TransactionId,
                BillId = bill.Id,
                BillCode = bill.BillCode,
                Amount = payload.Amount,
                Message = note
            };
        }
        else
        {
            // Nộp thiếu tiền -> Hóa đơn giữ nguyên Unpaid, giao dịch ghi nhận PartiallyPaid
            var underpaid = totalAmount - payload.Amount;
            var note = $"Đã thanh toán một phần (Nộp thiếu {underpaid:N0} VNĐ)";

            var partialTx = new PaymentTransaction
            {
                TransactionId = payload.TransactionId,
                BankBin = payload.BankBin,
                AccountNumber = payload.AccountNumber,
                Amount = payload.Amount,
                Description = payload.Description,
                TransactionDate = payload.TransactionDate,
                BillId = bill.Id,
                BillCode = bill.BillCode,
                Status = PaymentTransactionStatus.PartiallyPaid,
                RawPayload = payload.RawData ?? string.Empty,
                Note = note,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(partialTx);
            await _context.SaveChangesAsync();

            return new PaymentReconciliationResultDto
            {
                IsSuccess = false,
                Status = PaymentTransactionStatus.PartiallyPaid,
                TransactionId = payload.TransactionId,
                BillId = bill.Id,
                BillCode = bill.BillCode,
                Amount = payload.Amount,
                Message = note
            };
        }
    }

    /// <inheritdoc />
    public async Task<bool> ManuallyAssignBillAsync(int transactionId, int billId, string note = "")
    {
        var tx = await _context.PaymentTransactions.FindAsync(transactionId);
        if (tx == null)
        {
            return false;
        }

        var bill = await _context.Bills.FindAsync(billId);
        if (bill == null)
        {
            return false;
        }

        tx.BillId = bill.Id;
        tx.BillCode = bill.BillCode;

        var baseNote = string.IsNullOrWhiteSpace(note) ? "Kế toán xác nhận thủ công" : note.Trim();

        if (tx.Amount >= bill.TotalAmount)
        {
            bill.Status = BillStatus.Paid;
            bill.PaidDate = DateTime.UtcNow;

            tx.Status = PaymentTransactionStatus.Success;
            if (tx.Amount > bill.TotalAmount)
            {
                var overpaid = tx.Amount - bill.TotalAmount;
                tx.Note = $"{baseNote} (Nộp thừa {overpaid:N0} VNĐ)";
            }
            else
            {
                tx.Note = $"{baseNote} (Thanh toán đủ)";
            }
        }
        else
        {
            var underpaid = bill.TotalAmount - tx.Amount;
            tx.Status = PaymentTransactionStatus.PartiallyPaid;
            tx.Note = $"{baseNote} (Nộp thiếu {underpaid:N0} VNĐ)";
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<List<PaymentTransactionDto>> GetTransactionsAsync(PaymentTransactionFilterDto filter)
    {
        if (filter == null)
        {
            filter = new PaymentTransactionFilterDto();
        }

        var query = _context.PaymentTransactions.AsNoTracking().AsQueryable();

        if (filter.Status.HasValue)
        {
            query = query.Where(t => t.Status == filter.Status.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= filter.ToDate.Value);
        }

        if (filter.BillId.HasValue)
        {
            query = query.Where(t => t.BillId == filter.BillId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
        {
            var keyword = filter.SearchKeyword.Trim().ToLower();
            query = query.Where(t =>
                t.TransactionId.ToLower().Contains(keyword) ||
                t.Description.ToLower().Contains(keyword) ||
                t.BillCode.ToLower().Contains(keyword) ||
                t.AccountNumber.ToLower().Contains(keyword));
        }

        return await query
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.CreatedAt)
            .Select(t => new PaymentTransactionDto
            {
                Id = t.Id,
                TransactionId = t.TransactionId,
                BankBin = t.BankBin,
                AccountNumber = t.AccountNumber,
                Amount = t.Amount,
                Description = t.Description,
                TransactionDate = t.TransactionDate,
                BillId = t.BillId,
                BillCode = t.BillCode,
                Status = t.Status,
                RawPayload = t.RawPayload,
                Note = t.Note,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();
    }
}
