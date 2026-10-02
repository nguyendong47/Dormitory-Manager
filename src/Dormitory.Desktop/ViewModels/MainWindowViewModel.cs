using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel chính của ứng dụng Desktop, điều phối chuyển hướng giữa các màn hình chức năng
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly DashboardViewModel _dashboardVm;
    private readonly RoomListViewModel _roomListVm;
    private readonly StudentListViewModel _studentListVm;
    private readonly ContractListViewModel _contractListVm;
    private readonly BillListViewModel _billListVm;

    [ObservableProperty]
    private ViewModelBase _currentView;

    [ObservableProperty]
    private string _activeMenu = "Dashboard";

    public MainWindowViewModel(
        DashboardViewModel dashboardVm,
        RoomListViewModel roomListVm,
        StudentListViewModel studentListVm,
        ContractListViewModel contractListVm,
        BillListViewModel billListVm)
    {
        _dashboardVm = dashboardVm;
        _roomListVm = roomListVm;
        _studentListVm = studentListVm;
        _contractListVm = contractListVm;
        _billListVm = billListVm;

        // Khởi tạo màn hình mặc định là Dashboard
        _currentView = _dashboardVm;

        NavigateToDashboardCommand = new RelayCommand(NavigateToDashboard);
        NavigateToRoomsCommand = new RelayCommand(NavigateToRooms);
        NavigateToStudentsCommand = new RelayCommand(NavigateToStudents);
        NavigateToContractsCommand = new RelayCommand(NavigateToContracts);
        NavigateToBillsCommand = new RelayCommand(NavigateToBills);

        // Nạp số liệu Dashboard
        _ = _dashboardVm.LoadStatsAsync();
    }

    public IRelayCommand NavigateToDashboardCommand { get; }
    public IRelayCommand NavigateToRoomsCommand { get; }
    public IRelayCommand NavigateToStudentsCommand { get; }
    public IRelayCommand NavigateToContractsCommand { get; }
    public IRelayCommand NavigateToBillsCommand { get; }

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
}
