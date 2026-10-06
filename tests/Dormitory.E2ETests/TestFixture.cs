using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Dormitory.Application.Interfaces;
using Dormitory.Application.Services;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using Dormitory.Infrastructure.Data;
using Dormitory.Infrastructure.Security;
using Dormitory.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dormitory.E2ETests;

/// <summary>
/// Mock triển khai IFileService để ghi nhận các lời gọi lưu tệp và mảng byte dữ liệu xuất ra
/// </summary>
public class FakeFileService : IFileService
{
    public string? LastSavedFileName { get; private set; }
    public string? LastSavedExtension { get; private set; }
    public byte[]? LastSavedBytes { get; private set; }
    public int SaveCallCount { get; private set; }

    public Task<bool> SaveFileAsync(string defaultFileName, string extension, string fileTypeFilter, byte[] content)
    {
        LastSavedFileName = defaultFileName;
        LastSavedExtension = extension;
        LastSavedBytes = content;
        SaveCallCount++;
        return Task.FromResult(true);
    }

    public Task<byte[]?> OpenFileAsync(string title, string[] extensions)
    {
        return Task.FromResult<byte[]?>(null);
    }
}

/// <summary>
/// Mock triển khai IDialogService để hỗ trợ chạy thử nghiệm giao diện tự động không bị treo modal
/// </summary>
public class FakeDialogService : IDialogService
{
    public bool ConfirmResult { get; set; } = true;
    public object? DialogResult { get; set; } = true;
    public List<string> Messages { get; } = new();

    public Task<bool> ShowConfirmAsync(string title, string message)
    {
        return Task.FromResult(ConfirmResult);
    }

    public Task ShowMessageAsync(string title, string message)
    {
        Messages.Add($"{title}: {message}");
        return Task.CompletedTask;
    }

    public Func<Window, Task<object?>>? OnShowDialogAsync { get; set; }

    public async Task<TResult?> ShowDialogAsync<TResult>(Window dialog)
    {
        if (OnShowDialogAsync != null)
        {
            var res = await OnShowDialogAsync(dialog);
            return (TResult?)res;
        }

        if (DialogResult is TResult result)
            return result;
        return default;
    }
}

/// <summary>
/// Test fixture cung cấp môi trường cơ sở dữ liệu SQLite In-Memory độc lập, nạp sẵn dữ liệu Seed mẫu
/// và hệ thống DI container đầy đủ cho toàn bộ ViewModel/Service
/// </summary>
public class TestFixture : IDisposable
{
    public SqliteConnection Connection { get; }
    public DormitoryDbContext Context { get; }
    public ServiceProvider ServiceProvider { get; }
    public FakeFileService FileService { get; } = new();
    public FakeDialogService DialogService { get; } = new();
    public IUserSession UserSession { get; } = new UserSession();

    public TestFixture()
    {
        Connection = new SqliteConnection("DataSource=:memory:");
        Connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(Connection)
            .Options;

        Context = new DormitoryDbContext(options);
        Context.Database.EnsureCreated();

        // Nạp dữ liệu mẫu ban đầu
        DataSeeder.SeedAsync(Context).GetAwaiter().GetResult();

        var services = new ServiceCollection();

        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DormitoryDb"] = "DataSource=:memory:",
            ["BackupSettings:Directory"] = "backups",
            ["SystemSettings:AppName"] = "KTX Dormitory Test"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfig)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton(Context);
        services.AddSingleton<IDormitoryDbContext>(Context);
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IFileService>(FileService);
        services.AddSingleton<IDialogService>(DialogService);
        services.AddSingleton<IUserSession>(UserSession);

        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IContractService, ContractService>();
        services.AddScoped<IBillService, BillService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IViolationService, ViolationService>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IPdfExportService, PdfExportService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddScoped<IReportService, ReportService>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<RoomListViewModel>();
        services.AddTransient<EquipmentListViewModel>();
        services.AddTransient<EquipmentDialogViewModel>();
        services.AddTransient<StudentListViewModel>();
        services.AddTransient<ContractListViewModel>();
        services.AddTransient<BillListViewModel>();
        services.AddTransient<EmployeeListViewModel>();
        services.AddTransient<ViolationListViewModel>();
        services.AddTransient<ViolationDialogViewModel>();
        services.AddTransient<ReportListViewModel>();
        services.AddTransient<ReportGenerateDialogViewModel>();
        services.AddTransient<SystemSettingsViewModel>();
        services.AddTransient<MainWindowViewModel>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public T GetService<T>() where T : notnull => ServiceProvider.GetRequiredService<T>();

    public MainWindowViewModel CreateMainWindowViewModel() => GetService<MainWindowViewModel>();
    public EquipmentListViewModel CreateEquipmentListViewModel() => GetService<EquipmentListViewModel>();
    public BillListViewModel CreateBillListViewModel() => GetService<BillListViewModel>();
    public ViolationListViewModel CreateViolationListViewModel() => GetService<ViolationListViewModel>();
    public ReportListViewModel CreateReportListViewModel() => GetService<ReportListViewModel>();
    public ReportGenerateDialogViewModel CreateReportGenerateDialogViewModel() => GetService<ReportGenerateDialogViewModel>();

    public void Dispose()
    {
        Context.Dispose();
        Connection.Dispose();
        ServiceProvider.Dispose();
        GC.SuppressFinalize(this);
    }
}
