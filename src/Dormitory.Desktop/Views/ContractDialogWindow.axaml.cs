using System;
using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Cửa sổ modal lập hợp đồng thuê phòng ký túc xá mới.
/// </summary>
public partial class ContractDialogWindow : Window
{
    public ContractDialogWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    public ContractDialogWindow(ContractDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = result => Close(result);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is ContractDialogViewModel vm)
        {
            vm.CloseAction = result => Close(result);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape)
        {
            Close(false);
        }
    }
}
