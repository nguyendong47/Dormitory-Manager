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

        // 1. Cấu hình DbContext SQLite
        services.AddDbContext<DormitoryDbContext>(options =>
            options.UseSqlite("Data Source=dormitory.db"));

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
        services.AddScoped<IExportService, ExportService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IFileService, FileService>();

        // 3. Cấu hình ViewModels
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<RoomListViewModel>();
        services.AddTransient<StudentListViewModel>();
        services.AddTransient<ContractListViewModel>();
        services.AddTransient<BillListViewModel>();
        services.AddTransient<EmployeeListViewModel>();
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