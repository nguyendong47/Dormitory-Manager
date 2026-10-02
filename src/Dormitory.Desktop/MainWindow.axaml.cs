using Avalonia.Controls;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainWindowViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}