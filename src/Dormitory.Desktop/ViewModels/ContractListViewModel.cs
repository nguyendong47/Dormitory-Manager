using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.Views;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý danh sách hợp đồng thuê phòng KTX kèm các thao tác Lập hợp đồng mới, Gia hạn và Chấm dứt
/// </summary>
public partial class ContractListViewModel : ViewModelBase
{
    private readonly IContractService _contractService;
    private readonly IStudentService _studentService;
    private readonly IRoomService _roomService;
    private readonly IDialogService _dialogService;

    public ObservableCollection<string> FilterOptions { get; } = new()
    {
        "Tất cả",
        "Đang hiệu lực",
        "Sắp hết hạn (30 ngày)",
        "Đã kết thúc"
    };

    [ObservableProperty]
    private string _selectedFilter = "Tất cả";

    [ObservableProperty]
    private ObservableCollection<ContractDto> _contracts = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(TerminateContractCommand))]
    [NotifyCanExecuteChangedFor(nameof(RenewContractCommand))]
    private ContractDto? _selectedContract;

    [ObservableProperty]
    private ContractStatus? _selectedStatus;

    [ObservableProperty]
    private bool _isLoading;

    public ContractListViewModel(
        IContractService contractService,
        IStudentService studentService,
        IRoomService roomService,
        IDialogService dialogService)
    {
        _contractService = contractService;
        _studentService = studentService;
        _roomService = roomService;
        _dialogService = dialogService;
    }

    /// <summary>
    /// Xử lý khi bộ lọc hợp đồng thay đổi: tự động nạp lại danh sách hợp đồng
    /// </summary>
    partial void OnSelectedFilterChanged(string value)
    {
        _ = LoadContractsAsync();
    }

    /// <summary>
    /// Điều kiện để kích hoạt thao tác chấm dứt hợp đồng (chỉ áp dụng khi hợp đồng đang hiệu lực)
    /// </summary>
    private bool CanTerminateContract => SelectedContract != null && SelectedContract.Status == ContractStatus.Active;

    /// <summary>
    /// Điều kiện để kích hoạt thao tác gia hạn hợp đồng (áp dụng khi đã chọn một hợp đồng chưa bị chấm dứt)
    /// </summary>
    private bool CanRenewContract => SelectedContract != null && SelectedContract.Status != ContractStatus.Terminated;

    /// <summary>
    /// Tải danh sách hợp đồng từ cơ sở dữ liệu dựa trên bộ lọc đã chọn
    /// </summary>
    [RelayCommand]
    public async Task LoadContractsAsync()
    {
        IsLoading = true;
        try
        {
            List<ContractDto> list;
            var today = DateTime.Today;
            var maxDate = today.AddDays(30);

            switch (SelectedFilter)
            {
                case "Đang hiệu lực":
                    SelectedStatus = ContractStatus.Active;
                    list = await _contractService.GetAllContractsAsync(ContractStatus.Active);
                    break;
                case "Sắp hết hạn (30 ngày)":
                    SelectedStatus = ContractStatus.Active;
                    var activeList = await _contractService.GetAllContractsAsync(ContractStatus.Active);
                    list = activeList
                        .Where(c => c.Status == ContractStatus.Active && c.EndDate <= maxDate && c.EndDate >= today)
                        .ToList();
                    break;
                case "Đã kết thúc":
                    SelectedStatus = null;
                    var allForEnd = await _contractService.GetAllContractsAsync();
                    list = allForEnd
                        .Where(c => c.Status == ContractStatus.Terminated || c.Status == ContractStatus.Expired)
                        .ToList();
                    break;
                case "Tất cả":
                default:
                    SelectedStatus = null;
                    list = await _contractService.GetAllContractsAsync();
                    break;
            }

            Contracts = new ObservableCollection<ContractDto>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Mở hộp thoại lập hợp đồng thuê phòng mới cho sinh viên
    /// </summary>
    [RelayCommand]
    public async Task CreateContractAsync()
    {
        var dialogVm = new ContractDialogViewModel(_contractService, _studentService, _roomService);
        await dialogVm.LoadInitialDataAsync();
        var dialog = new ContractDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadContractsAsync();
        }
    }

    /// <summary>
    /// Chấm dứt / Thanh lý hợp đồng đang được chọn
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanTerminateContract))]
    public async Task TerminateContractAsync()
    {
        if (SelectedContract == null || SelectedContract.Status != ContractStatus.Active) return;

        var confirmed = await _dialogService.ShowConfirmAsync(
            "Xác nhận chấm dứt hợp đồng",
            $"Bạn có chắc chắn muốn chấm dứt hợp đồng {SelectedContract.ContractNumber} của sinh viên {SelectedContract.StudentName} trước hạn không?");

        if (!confirmed) return;

        try
        {
            var success = await _contractService.TerminateContractAsync(SelectedContract.Id, "Chấm dứt trước hạn");
            if (success)
            {
                await LoadContractsAsync();
            }
            else
            {
                await _dialogService.ShowMessageAsync("Lỗi", "Không thể chấm dứt hợp đồng.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", ex.Message);
        }
    }

    /// <summary>
    /// Gia hạn thời gian thuê của hợp đồng thêm 6 tháng
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRenewContract))]
    public async Task RenewContractAsync()
    {
        if (SelectedContract == null || SelectedContract.Status == ContractStatus.Terminated) return;

        var newEndDate = SelectedContract.EndDate.AddMonths(6);
        var confirmed = await _dialogService.ShowConfirmAsync(
            "Xác nhận gia hạn hợp đồng",
            $"Bạn có chắc chắn muốn gia hạn hợp đồng {SelectedContract.ContractNumber} thêm 6 tháng (đến ngày {newEndDate:dd/MM/yyyy}) không?");

        if (!confirmed) return;

        try
        {
            var success = await _contractService.RenewContractAsync(SelectedContract.Id, newEndDate);
            if (success)
            {
                await LoadContractsAsync();
            }
            else
            {
                await _dialogService.ShowMessageAsync("Lỗi", "Không thể gia hạn hợp đồng.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", ex.Message);
        }
    }
}
