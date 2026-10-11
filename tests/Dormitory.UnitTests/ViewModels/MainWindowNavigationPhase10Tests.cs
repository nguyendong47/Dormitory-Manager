using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

/// <summary>
/// Bộ kiểm thử đơn vị cho điều hướng Sidebar, RBAC và DI Container trong Phase 10
/// </summary>
public class MainWindowNavigationPhase10Tests
{
    private readonly IUserSession _userSession;
    private readonly IAuthService _authService;
    private readonly IDashboardService _dashboardService;
    private readonly IRoomService _roomService;
    private readonly IEquipmentService _equipmentService;
    private readonly IStudentService _studentService;
    private readonly IContractService _contractService;
    private readonly IBillService _billService;
    private readonly IEmployeeService _employeeService;
    private readonly IViolationService _violationService;
    private readonly IExportService _exportService;
    private readonly IPdfExportService _pdfExportService;
    private readonly IEmailService _emailService;
    private readonly IDatabaseService _databaseService;
    private readonly IReportService _reportService;
    private readonly IBankSettingsService _bankSettingsService;
    private readonly IVietQrService _vietQrService;
    private readonly IPaymentReconciliationService _reconciliationService;
    private readonly IVisitorService _visitorService;
    private readonly IAttendanceService _attendanceService;
    private readonly IDialogService _dialogService;
    private readonly IFileService _fileService;

    public MainWindowNavigationPhase10Tests()
    {
        _userSession = Substitute.For<IUserSession>();
        _authService = Substitute.For<IAuthService>();
        _dashboardService = Substitute.For<IDashboardService>();
        _roomService = Substitute.For<IRoomService>();
        _equipmentService = Substitute.For<IEquipmentService>();
        _studentService = Substitute.For<IStudentService>();
        _contractService = Substitute.For<IContractService>();
        _billService = Substitute.For<IBillService>();
        _employeeService = Substitute.For<IEmployeeService>();
        _violationService = Substitute.For<IViolationService>();
        _exportService = Substitute.For<IExportService>();
        _pdfExportService = Substitute.For<IPdfExportService>();
        _emailService = Substitute.For<IEmailService>();
        _databaseService = Substitute.For<IDatabaseService>();
        _reportService = Substitute.For<IReportService>();
        _bankSettingsService = Substitute.For<IBankSettingsService>();
        _vietQrService = Substitute.For<IVietQrService>();
        _reconciliationService = Substitute.For<IPaymentReconciliationService>();
        _visitorService = Substitute.For<IVisitorService>();
        _attendanceService = Substitute.For<IAttendanceService>();
        _dialogService = Substitute.For<IDialogService>();
        _fileService = Substitute.For<IFileService>();

        // Cấu hình kết quả mặc định cho các dịch vụ
        _roomService.GetAllRoomsAsync(Arg.Any<string?>(), Arg.Any<RoomStatus?>())
            .Returns(Task.FromResult(new List<RoomDto>
            {
                new() { Id = 1, RoomNumber = "P101", Building = "A", Capacity = 4, CurrentOccupancy = 2 }
            }));

        _studentService.GetAllStudentsAsync(Arg.Any<string?>(), Arg.Any<int?>())
            .Returns(Task.FromResult(new List<StudentDto>()));

        _visitorService.GetVisitorsAsync(Arg.Any<VisitorFilterDto>(), default)
            .Returns(Task.FromResult<IReadOnlyList<VisitorLogDto>>(new List<VisitorLogDto>()));

        _visitorService.GetVisitorSummaryAsync(Arg.Any<DateTime?>(), default)
            .Returns(Task.FromResult(new VisitorSummaryDto()));

        _attendanceService.GetRoomAttendanceAsync(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<AttendanceRecordDto>>(new List<AttendanceRecordDto>()));
    }

    /// <summary>
    /// Tạo ServiceProvider chứa đầy đủ các đăng ký DI tương tự App.axaml.cs
    /// </summary>
    private IServiceProvider CreateServiceProvider(IUserSession? customSession = null)
    {
        var services = new ServiceCollection();

        services.AddSingleton(customSession ?? _userSession);
        services.AddSingleton(_authService);
        services.AddSingleton(_dashboardService);
        services.AddSingleton(_roomService);
        services.AddSingleton(_equipmentService);
        services.AddSingleton(_studentService);
        services.AddSingleton(_contractService);
        services.AddSingleton(_billService);
        services.AddSingleton(_employeeService);
        services.AddSingleton(_violationService);
        services.AddSingleton(_exportService);
        services.AddSingleton(_pdfExportService);
        services.AddSingleton(_emailService);
        services.AddSingleton(_databaseService);
        services.AddSingleton(_reportService);
        services.AddSingleton(_bankSettingsService);
        services.AddSingleton(_vietQrService);
        services.AddSingleton(_reconciliationService);
        services.AddSingleton(_visitorService);
        services.AddSingleton(_attendanceService);
        services.AddSingleton(_dialogService);
        services.AddSingleton(_fileService);

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<RoomListViewModel>();
        services.AddTransient<EquipmentListViewModel>();
        services.AddTransient<StudentListViewModel>();
        services.AddTransient<ContractListViewModel>();
        services.AddTransient<BillListViewModel>();
        services.AddTransient<EmployeeListViewModel>();
        services.AddTransient<ViolationListViewModel>();
        services.AddTransient<ReportListViewModel>();
        services.AddTransient<SystemSettingsViewModel>();
        services.AddTransient<PaymentTransactionListViewModel>();
        services.AddTransient<VisitorListViewModel>();
        services.AddTransient<AttendanceTrackingViewModel>();
        services.AddTransient<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Khởi tạo MainWindowViewModel với các ViewModel phụ trợ cụ thể
    /// </summary>
    private (MainWindowViewModel MainWindowVm, VisitorListViewModel VisitorVm, AttendanceTrackingViewModel AttendanceVm, LoginViewModel LoginVm)
        CreateMainWindowViewModelDirect(IUserSession session)
    {
        var provider = CreateServiceProvider(session);
        var visitorVm = provider.GetRequiredService<VisitorListViewModel>();
        var attendanceVm = provider.GetRequiredService<AttendanceTrackingViewModel>();
        var loginVm = provider.GetRequiredService<LoginViewModel>();

        var mainWindowVm = new MainWindowViewModel(
            provider.GetRequiredService<DashboardViewModel>(),
            provider.GetRequiredService<RoomListViewModel>(),
            provider.GetRequiredService<EquipmentListViewModel>(),
            provider.GetRequiredService<StudentListViewModel>(),
            provider.GetRequiredService<ContractListViewModel>(),
            provider.GetRequiredService<BillListViewModel>(),
            provider.GetRequiredService<EmployeeListViewModel>(),
            provider.GetRequiredService<ViolationListViewModel>(),
            provider.GetRequiredService<ReportListViewModel>(),
            provider.GetRequiredService<SystemSettingsViewModel>(),
            provider.GetRequiredService<PaymentTransactionListViewModel>(),
            visitorVm,
            attendanceVm,
            loginVm,
            session
        );

        return (mainWindowVm, visitorVm, attendanceVm, loginVm);
    }

    [Fact]
    public void Should_Initialize_MainWindowViewModel_With_Dependencies_When_Unauthenticated()
    {
        // Arrange
        _userSession.IsAuthenticated.Returns(false);
        _userSession.IsAdmin.Returns(false);
        _userSession.CurrentUser.Returns((UserDto?)null);

        // Act
        var (mainWindowVm, visitorVm, attendanceVm, loginVm) = CreateMainWindowViewModelDirect(_userSession);

        // Assert
        mainWindowVm.IsLoggedIn.Should().BeFalse("Người dùng chưa đăng nhập");
        mainWindowVm.IsAdmin.Should().BeFalse("Không có quyền admin khi chưa đăng nhập");
        mainWindowVm.CurrentView.Should().BeSameAs(loginVm, "Màn hình ban đầu phải là đăng nhập");
        mainWindowVm.NavigateToVisitorsCommand.Should().NotBeNull("Lệnh điều hướng khách phải được khởi tạo");
        mainWindowVm.NavigateToAttendanceCommand.Should().NotBeNull("Lệnh điều hướng điểm danh phải được khởi tạo");
    }

    [Fact]
    public void Should_Initialize_MainWindowViewModel_With_Dashboard_When_Authenticated()
    {
        // Arrange
        var user = new UserDto
        {
            Id = 1,
            Username = "admin",
            FullName = "Quản Trị Viên",
            Role = UserRole.Admin
        };
        _userSession.IsAuthenticated.Returns(true);
        _userSession.IsAdmin.Returns(true);
        _userSession.CurrentUser.Returns(user);

        // Act
        var (mainWindowVm, _, _, _) = CreateMainWindowViewModelDirect(_userSession);

        // Assert
        mainWindowVm.IsLoggedIn.Should().BeTrue();
        mainWindowVm.IsAdmin.Should().BeTrue();
        mainWindowVm.CurrentUserName.Should().Be("Quản Trị Viên");
        mainWindowVm.CurrentUserRole.Should().Be("Quản trị viên (Admin)");
        mainWindowVm.CurrentView.Should().BeOfType<DashboardViewModel>();
        mainWindowVm.ActiveMenu.Should().Be("Dashboard");
    }

    [Fact]
    public void Should_Navigate_To_Visitors_When_NavigateToVisitorsCommand_Executed()
    {
        // Arrange
        var user = new UserDto { Id = 1, Username = "staff", FullName = "Nhân Viên", Role = UserRole.Staff };
        _userSession.IsAuthenticated.Returns(true);
        _userSession.IsAdmin.Returns(false);
        _userSession.CurrentUser.Returns(user);

        var (mainWindowVm, visitorVm, _, _) = CreateMainWindowViewModelDirect(_userSession);

        // Act
        mainWindowVm.NavigateToVisitorsCommand.Execute(null);

        // Assert
        mainWindowVm.CurrentView.Should().BeSameAs(visitorVm, "CurrentView phải trỏ về VisitorListViewModel");
        mainWindowVm.ActiveMenu.Should().Be("Visitors", "ActiveMenu phải là 'Visitors'");

        // Kiểm tra phương thức tải dữ liệu khách được kích hoạt
        _visitorService.Received().GetVisitorsAsync(Arg.Any<VisitorFilterDto>(), default);
    }

    [Fact]
    public void Should_Navigate_To_Attendance_When_NavigateToAttendanceCommand_Executed()
    {
        // Arrange
        var user = new UserDto { Id = 2, Username = "manager", FullName = "Quản Lý", Role = UserRole.Manager };
        _userSession.IsAuthenticated.Returns(true);
        _userSession.IsAdmin.Returns(false);
        _userSession.CurrentUser.Returns(user);

        var (mainWindowVm, _, attendanceVm, _) = CreateMainWindowViewModelDirect(_userSession);

        // Act
        mainWindowVm.NavigateToAttendanceCommand.Execute(null);

        // Assert
        mainWindowVm.CurrentView.Should().BeSameAs(attendanceVm, "CurrentView phải trỏ về AttendanceTrackingViewModel");
        mainWindowVm.ActiveMenu.Should().Be("Attendance", "ActiveMenu phải là 'Attendance'");

        // Kiểm tra phương thức tải danh sách phòng điểm danh được kích hoạt
        _roomService.Received().GetAllRoomsAsync(Arg.Any<string?>(), Arg.Any<RoomStatus?>());
    }

    [Theory]
    [InlineData(UserRole.Admin, true, "Quản trị viên (Admin)")]
    [InlineData(UserRole.Manager, false, "Quản lý KTX (Manager)")]
    [InlineData(UserRole.Staff, false, "Nhân viên (Staff)")]
    public async Task Should_Handle_RBAC_Roles_And_Allow_Navigation_To_Visitors_And_Attendance(
        UserRole role, bool expectedIsAdmin, string expectedRoleDisplay)
    {
        // Arrange
        var realSession = new UserSession();
        var (mainWindowVm, visitorVm, attendanceVm, loginVm) = CreateMainWindowViewModelDirect(realSession);

        mainWindowVm.IsLoggedIn.Should().BeFalse();

        // Act: Giả lập đăng nhập thành công với vai trò tương ứng
        var user = new UserDto
        {
            Id = 10,
            Username = "user_" + role.ToString().ToLower(),
            FullName = "Người dùng " + role,
            Role = role
        };

        // Kích hoạt sự kiện LoginSuccess
        _authService.LoginAsync(Arg.Any<LoginRequest>()).Returns(Task.FromResult(new LoginResponse
        {
            Success = true,
            User = user
        }));
        loginVm.Username = user.Username;
        loginVm.Password = "Password123";
        await loginVm.LoginCommand.ExecuteAsync(null);

        // Assert: Trạng thái RBAC phải được cập nhật chính xác
        mainWindowVm.IsLoggedIn.Should().BeTrue();
        mainWindowVm.IsAdmin.Should().Be(expectedIsAdmin);
        mainWindowVm.CurrentUserName.Should().Be("Người dùng " + role);
        mainWindowVm.CurrentUserRole.Should().Be(expectedRoleDisplay);

        // Cả 3 vai trò (Admin, Manager, Staff) đều có quyền điều hướng tới Khách ra vào
        mainWindowVm.NavigateToVisitorsCommand.CanExecute(null).Should().BeTrue();
        mainWindowVm.NavigateToVisitorsCommand.Execute(null);
        mainWindowVm.CurrentView.Should().BeSameAs(visitorVm);
        mainWindowVm.ActiveMenu.Should().Be("Visitors");

        // Cả 3 vai trò đều có quyền điều hướng tới Điểm danh KTX
        mainWindowVm.NavigateToAttendanceCommand.CanExecute(null).Should().BeTrue();
        mainWindowVm.NavigateToAttendanceCommand.Execute(null);
        mainWindowVm.CurrentView.Should().BeSameAs(attendanceVm);
        mainWindowVm.ActiveMenu.Should().Be("Attendance");
    }

    [Fact]
    public async Task Should_Reset_State_And_Navigate_To_Login_On_Logout()
    {
        // Arrange
        var realSession = new UserSession();
        var (mainWindowVm, _, _, loginVm) = CreateMainWindowViewModelDirect(realSession);

        var user = new UserDto { Id = 1, Username = "admin", FullName = "Admin", Role = UserRole.Admin };
        _authService.LoginAsync(Arg.Any<LoginRequest>()).Returns(Task.FromResult(new LoginResponse
        {
            Success = true,
            User = user
        }));
        loginVm.Username = "admin";
        loginVm.Password = "Admin@123456";
        await loginVm.LoginCommand.ExecuteAsync(null);

        mainWindowVm.IsLoggedIn.Should().BeTrue();

        // Act
        mainWindowVm.LogoutCommand.Execute(null);

        // Assert
        mainWindowVm.IsLoggedIn.Should().BeFalse();
        mainWindowVm.IsAdmin.Should().BeFalse();
        mainWindowVm.CurrentUserName.Should().BeEmpty();
        mainWindowVm.CurrentUserRole.Should().BeEmpty();
        mainWindowVm.CurrentView.Should().BeSameAs(loginVm);
        realSession.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void Should_Resolve_MainWindowViewModel_And_All_Dependencies_From_DI_Container()
    {
        // Arrange
        var provider = CreateServiceProvider();

        // Act
        var mainWindowVm = provider.GetRequiredService<MainWindowViewModel>();

        // Assert
        mainWindowVm.Should().NotBeNull();
        mainWindowVm.NavigateToVisitorsCommand.Should().NotBeNull();
        mainWindowVm.NavigateToAttendanceCommand.Should().NotBeNull();
        mainWindowVm.NavigateToDashboardCommand.Should().NotBeNull();
        mainWindowVm.NavigateToRoomsCommand.Should().NotBeNull();
        mainWindowVm.NavigateToEquipmentsCommand.Should().NotBeNull();
        mainWindowVm.NavigateToStudentsCommand.Should().NotBeNull();
        mainWindowVm.NavigateToContractsCommand.Should().NotBeNull();
        mainWindowVm.NavigateToBillsCommand.Should().NotBeNull();
        mainWindowVm.NavigateToTransactionsCommand.Should().NotBeNull();
        mainWindowVm.NavigateToEmployeesCommand.Should().NotBeNull();
        mainWindowVm.NavigateToViolationsCommand.Should().NotBeNull();
        mainWindowVm.NavigateToReportsCommand.Should().NotBeNull();
        mainWindowVm.NavigateToSettingsCommand.Should().NotBeNull();
        mainWindowVm.LogoutCommand.Should().NotBeNull();
    }
}
