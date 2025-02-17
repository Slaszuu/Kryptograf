using System.Windows;
using Kryptograf.ViewModels;

namespace Kryptograf.Views;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(EncryptionViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
