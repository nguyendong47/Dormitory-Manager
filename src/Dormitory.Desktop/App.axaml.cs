using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Dormitory.Application.Interfaces;
using Dormitory.Application.Services;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using Dormitory.Desktop.Views;
using Dormitory.Infrastructure.Data;
using Dormitory.Infrastructure.Security;
using Dormitory.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dormitory.Desktop;

public partial class App : Avalonia.Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        // 0. Cấu hình IConfiguration từ appsettings.json (hỗ trợ cấu hình động)
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        services.AddSingleton<IConfiguration>(config);

        // 1. Cấu hình DbContext SQLite với chuỗi kết nối an toàn giải quyết qua DatabasePathResolver
        var rawConnectionString = config.GetConnectionString("DormitoryDb") ?? "Data Source=dormitory.db";
        var connectionString = DatabasePathResolver.BuildConnectionString(rawConnectionString);
        services.AddDbContext<DormitoryDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IDormitoryDbContext>(provider =>
            provider.GetRequiredService<DormitoryDbContext>());

        // 2. Cấu hình Security & Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
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
        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IUserSession, UserSession>();

        // 3. Cấu hình ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<RoomListViewModel>();
        services.AddTransient<EquipmentListViewModel>();
        services.AddTransient<EquipmentDialogViewModel>();
        services.AddTransient<StudentListViewModel>();
        services.AddTransient<ContractListViewModel>();
        services.AddTransient<BillListViewModel>();
        services.AddTransient<EmployeeListViewModel>();
        services.AddTransient<SystemSettingsViewModel>();
        services.AddTransient<MainWindowViewModel>();

        // 4. Cấu hình Views
        services.AddTransient<MainWindow>();

        Services = services.BuildServiceProvider();

        // Tự động seed dữ liệu mẫu ban đầu
        try
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DormitoryDbContext>();
            await DataSeeder.SeedAsync(dbContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Lỗi khởi tạo DB Seed]: {ex.Message}");
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindowVm = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow(mainWindowVm);
        }

        base.OnFrameworkInitializationCompleted();
    }
}