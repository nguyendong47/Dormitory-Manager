using System;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using Dormitory.Desktop;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using Xunit;

namespace Dormitory.E2ETests.Journeys;

/// <summary>
/// Kiểm thử luồng E2E cho xác thực người dùng và điều hướng các màn hình chức năng trên Sidebar
/// </summary>
public class AuthAndNavigationE2ETests : IDisposable
{
    private readonly TestFixture _fixture = new();

    /// <summary>
    /// Kịch bản E2E: Khởi động giao diện chính -> Xác thực thất bại -> Đăng nhập thành công -> Điều hướng toàn bộ thanh Sidebar
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Authenticate_Admin_Successfully_And_Navigate_Across_All_Views()
    {
        // 1. Khởi động MainWindow với MainWindowViewModel
        var vm = _fixture.CreateMainWindowViewModel();
        var mainWindow = new MainWindow(vm);
        mainWindow.Show();

        mainWindow.DataContext.Should().Be(vm);
        mainWindow.IsVisible.Should().BeTrue();

        // 2. Ban đầu khi chưa đăng nhập, CurrentView phải là LoginViewModel và IsLoggedIn là false
        vm.IsLoggedIn.Should().BeFalse();
        vm.CurrentView.Should().BeOfType<LoginViewModel>();
        vm.LoginVm.Should().NotBeNull();

        // 3. Nhập mật khẩu sai -> Đăng nhập thất bại và hiển thị thông báo lỗi
        vm.LoginVm.Username = "admin";
        vm.LoginVm.Password = "SaiMatKhau@2026";
        await vm.LoginVm.LoginCommand.ExecuteAsync(null);

        vm.IsLoggedIn.Should().BeFalse();
        vm.LoginVm.ErrorMessage.Should().NotBeNullOrWhiteSpace("Hệ thống phải báo lỗi khi nhập sai mật khẩu");
        vm.CurrentView.Should().BeOfType<LoginViewModel>("Người dùng vẫn phải ở màn hình đăng nhập");

        // 4. Nhập đúng tài khoản quản trị admin / Admin@123456 -> Đăng nhập thành công
        vm.LoginVm.Username = "admin";
        vm.LoginVm.Password = "Admin@123456";
        await vm.LoginVm.LoginCommand.ExecuteAsync(null);

        vm.IsLoggedIn.Should().BeTrue("Trạng thái đăng nhập phải là true sau khi xác thực thành công");
        vm.IsAdmin.Should().BeTrue("Tài khoản admin phải có cờ IsAdmin = true");
        vm.CurrentUserName.Should().Be("Quản Trị Viên Hệ Thống");
        vm.CurrentView.Should().BeOfType<DashboardViewModel>("Màn hình mặc định sau đăng nhập phải là Dashboard");
        vm.ActiveMenu.Should().Be("Dashboard");

        // 5. Điều hướng qua các menu chức năng trên thanh Sidebar
        // 5.1 Điều hướng tới danh sách phòng ở
        vm.NavigateToRoomsCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<RoomListViewModel>();
        vm.ActiveMenu.Should().Be("Rooms");

        // 5.2 Điều hướng tới danh sách sinh viên
        vm.NavigateToStudentsCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<StudentListViewModel>();
        vm.ActiveMenu.Should().Be("Students");

        // 5.3 Điều hướng tới danh sách quản lý tài sản, thiết bị
        vm.NavigateToEquipmentsCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<EquipmentListViewModel>();
        vm.ActiveMenu.Should().Be("Equipments");

        // 5.4 Điều hướng tới danh sách hóa đơn & dịch vụ
        vm.NavigateToBillsCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<BillListViewModel>();
        vm.ActiveMenu.Should().Be("Bills");

        // 5.5 Điều hướng tới danh sách hợp đồng
        vm.NavigateToContractsCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<ContractListViewModel>();
        vm.ActiveMenu.Should().Be("Contracts");

        // 5.6 Điều hướng tới danh sách nhân viên
        vm.NavigateToEmployeesCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<EmployeeListViewModel>();
        vm.ActiveMenu.Should().Be("Employees");

        // 5.7 Điều hướng tới quản lý vi phạm & kỷ luật
        vm.NavigateToViolationsCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<ViolationListViewModel>();
        vm.ActiveMenu.Should().Be("Violations");

        // 5.8 Điều hướng tới báo cáo & thống kê
        vm.NavigateToReportsCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<ReportListViewModel>();
        vm.ActiveMenu.Should().Be("Reports");

        // 5.9 Điều hướng tới cài đặt hệ thống (Admin)
        await vm.NavigateToSettingsCommand.ExecuteAsync(null);
        vm.CurrentView.Should().BeOfType<SystemSettingsViewModel>();
        vm.ActiveMenu.Should().Be("Settings");

        // 6. Đăng xuất khỏi hệ thống
        vm.LogoutCommand.Execute(null);
        vm.IsLoggedIn.Should().BeFalse("Trạng thái đăng nhập phải chuyển về false");
        vm.CurrentView.Should().BeOfType<LoginViewModel>("Màn hình phải quay về LoginViewModel sau khi đăng xuất");

        mainWindow.Close();
    }

    public void Dispose()
    {
        _fixture.Dispose();
        GC.SuppressFinalize(this);
    }
}
