using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel chính của ứng dụng Desktop, điều phối chuyển hướng giữa các màn hình chức năng và quản lý phiên đăng nhập
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly DashboardViewModel _dashboardVm;
    private readonly RoomListViewModel _roomListVm;
    private readonly EquipmentListViewModel _equipmentListVm;
    private readonly StudentListViewModel _studentListVm;
    private readonly ContractListViewModel _contractListVm;
    private readonly BillListViewModel _billListVm;
    private readonly EmployeeListViewModel _employeeListVm;
    private readonly ViolationListViewModel _violationListVm;
    private readonly SystemSettingsViewModel _systemSettingsVm;
    private readonly IUserSession _userSession;

    public LoginViewModel LoginVm { get; }

    [ObservableProperty]
    private bool _isLoggedIn;

    [ObservableProperty]
    private bool _isAdmin;

    [ObservableProperty]
    private string _currentUserName = string.Empty;

    [ObservableProperty]
    private string _currentUserRole = string.Empty;

    [ObservableProperty]
    private ViewModelBase _currentView;

    [ObservableProperty]
    private string _activeMenu = "Dashboard";

    public MainWindowViewModel(
        DashboardViewModel dashboardVm,
        RoomListViewModel roomListVm,
        EquipmentListViewModel equipmentListVm,
        StudentListViewModel studentListVm,
        ContractListViewModel contractListVm,
        BillListViewModel billListVm,
        EmployeeListViewModel employeeListVm,
        ViolationListViewModel violationListVm,
        SystemSettingsViewModel systemSettingsVm,
        LoginViewModel loginVm,
        IUserSession userSession)
    {
        _dashboardVm = dashboardVm;
        _roomListVm = roomListVm;
        _equipmentListVm = equipmentListVm;
        _studentListVm = studentListVm;
        _contractListVm = contractListVm;
        _billListVm = billListVm;
        _employeeListVm = employeeListVm;
        _violationListVm = violationListVm;
        _systemSettingsVm = systemSettingsVm;
        LoginVm = loginVm;
        _userSession = userSession;

        // Khởi tạo màn hình mặc định ban đầu dựa vào trạng thái xác thực
        if (_userSession.IsAuthenticated)
        {
            _currentView = _dashboardVm;
            UpdateUserInfo();
            IsLoggedIn = true;
            IsAdmin = _userSession.IsAdmin;
            _ = _dashboardVm.LoadStatsAsync();
        }
        else
        {
            _currentView = LoginVm;
            IsLoggedIn = false;
            IsAdmin = false;
        }

        // Lắng nghe sự kiện đăng nhập thành công từ LoginViewModel
        LoginVm.LoginSuccess += OnLoginSuccess;

        NavigateToDashboardCommand = new RelayCommand(NavigateToDashboard);
        NavigateToRoomsCommand = new RelayCommand(NavigateToRooms);
        NavigateToEquipmentsCommand = new RelayCommand(NavigateToEquipments);
        NavigateToStudentsCommand = new RelayCommand(NavigateToStudents);
        NavigateToContractsCommand = new RelayCommand(NavigateToContracts);
        NavigateToBillsCommand = new RelayCommand(NavigateToBills);
        NavigateToEmployeesCommand = new RelayCommand(NavigateToEmployees);
        NavigateToViolationsCommand = new RelayCommand(NavigateToViolations);
        NavigateToSettingsCommand = new AsyncRelayCommand(NavigateToSettingsAsync);
        LogoutCommand = new RelayCommand(Logout);
    }

    public IRelayCommand NavigateToDashboardCommand { get; }
    public IRelayCommand NavigateToRoomsCommand { get; }
    public IRelayCommand NavigateToEquipmentsCommand { get; }
    public IRelayCommand NavigateToStudentsCommand { get; }
    public IRelayCommand NavigateToContractsCommand { get; }
    public IRelayCommand NavigateToBillsCommand { get; }
    public IRelayCommand NavigateToEmployeesCommand { get; }
    public IRelayCommand NavigateToViolationsCommand { get; }
    public IAsyncRelayCommand NavigateToSettingsCommand { get; }
    public IRelayCommand LogoutCommand { get; }

    /// <summary>
    /// Xử lý khi người dùng đăng nhập thành công
    /// </summary>
    private void OnLoginSuccess()
    {
        UpdateUserInfo();
        IsLoggedIn = true;
        NavigateToDashboard();
    }

    /// <summary>
    /// Cập nhật tên và vai trò người dùng từ phiên hiện tại
    /// </summary>
    private void UpdateUserInfo()
    {
        if (_userSession.CurrentUser != null)
        {
            CurrentUserName = _userSession.CurrentUser.FullName;
            CurrentUserRole = _userSession.CurrentUser.Role switch
            {
                UserRole.Admin => "Quản trị viên (Admin)",
                UserRole.Manager => "Quản lý KTX (Manager)",
                UserRole.Staff => "Nhân viên (Staff)",
                _ => _userSession.CurrentUser.Role.ToString()
            };
            IsAdmin = _userSession.IsAdmin;
        }
        else
        {
            CurrentUserName = string.Empty;
            CurrentUserRole = string.Empty;
            IsAdmin = false;
        }
    }

    /// <summary>
    /// Đăng xuất khỏi hệ thống và xóa phiên làm việc
    /// </summary>
    public void Logout()
    {
        _userSession.ClearSession();
        IsLoggedIn = false;
        IsAdmin = false;
        CurrentUserName = string.Empty;
        CurrentUserRole = string.Empty;
        LoginVm.Password = string.Empty;
        LoginVm.ErrorMessage = null;
        CurrentView = LoginVm;
    }

    public void NavigateToDashboard()
    {
        CurrentView = _dashboardVm;
        ActiveMenu = "Dashboard";
        _ = _dashboardVm.LoadStatsAsync();
    }

    public void NavigateToRooms()
    {
        CurrentView = _roomListVm;
        ActiveMenu = "Rooms";
        _ = _roomListVm.LoadRoomsAsync();
    }

    public void NavigateToEquipments()
    {
        CurrentView = _equipmentListVm;
        ActiveMenu = "Equipments";
        _ = _equipmentListVm.LoadEquipmentsAsync();
    }

    public void NavigateToStudents()
    {
        CurrentView = _studentListVm;
        ActiveMenu = "Students";
        _ = _studentListVm.LoadStudentsAsync();
    }

    public void NavigateToContracts()
    {
        CurrentView = _contractListVm;
        ActiveMenu = "Contracts";
        _ = _contractListVm.LoadContractsAsync();
    }

    public void NavigateToBills()
    {
        CurrentView = _billListVm;
        ActiveMenu = "Bills";
        _ = _billListVm.LoadBillsAsync();
    }

    public void NavigateToEmployees()
    {
        CurrentView = _employeeListVm;
        ActiveMenu = "Employees";
        _ = _employeeListVm.LoadEmployeesAsync();
    }

    public void NavigateToViolations()
    {
        CurrentView = _violationListVm;
        ActiveMenu = "Violations";
        _ = _violationListVm.LoadViolationsAsync();
    }

    public async Task NavigateToSettingsAsync()
    {
        CurrentView = _systemSettingsVm;
        ActiveMenu = "Settings";
        await _systemSettingsVm.LoadDatabaseInfoCommand.ExecuteAsync(null);
    }
}
