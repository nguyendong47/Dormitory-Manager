using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý danh sách hợp đồng thuê phòng KTX
/// </summary>
public partial class ContractListViewModel : ViewModelBase
{
    private readonly IContractService _contractService;

    [ObservableProperty]
    private ObservableCollection<ContractDto> _contracts = new();

    [ObservableProperty]
    private ContractDto? _selectedContract;

    [ObservableProperty]
    private ContractStatus? _selectedStatus;

    [ObservableProperty]
    private bool _isLoading;

    public ContractListViewModel(IContractService contractService)
    {
        _contractService = contractService;
        LoadContractsCommand = new AsyncRelayCommand(LoadContractsAsync);
        TerminateContractCommand = new AsyncRelayCommand(TerminateSelectedContractAsync, () => SelectedContract != null && SelectedContract.Status == ContractStatus.Active);
    }

    public IAsyncRelayCommand LoadContractsCommand { get; }
    public IAsyncRelayCommand TerminateContractCommand { get; }

    /// <summary>
    /// Tải danh sách hợp đồng từ cơ sở dữ liệu
    /// </summary>
    public async Task LoadContractsAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _contractService.GetAllContractsAsync(SelectedStatus);
            Contracts = new ObservableCollection<ContractDto>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Thanh lý hợp đồng đang được chọn
    /// </summary>
    private async Task TerminateSelectedContractAsync()
    {
        if (SelectedContract == null) return;

        var success = await _contractService.TerminateContractAsync(SelectedContract.Id, "Thanh lý bởi người dùng");
        if (success)
        {
            await LoadContractsAsync();
        }
    }
}
