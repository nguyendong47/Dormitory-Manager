using System;
using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Cửa sổ modal hỗ trợ kế toán gán thủ công một hóa đơn chưa thanh toán cho giao dịch ngân hàng
/// </summary>
public partial class AssignBillDialogWindow : Window
{
    public AssignBillDialogWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    public AssignBillDialogWindow(AssignBillDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;
        SetupViewModel(viewModel);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is AssignBillDialogViewModel vm)
        {
            SetupViewModel(vm);
        }
    }

    private void SetupViewModel(AssignBillDialogViewModel vm)
    {
        vm.CloseAction = result => Close(result);
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
