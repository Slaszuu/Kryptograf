using System.Windows;
using Kryptograf.Services;
using Kryptograf.ViewModels;
using Kryptograf.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Kryptograf;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static ServiceProvider ServiceProvider { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddSingleton<IEncryptionService, AesEncryptionService>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        ServiceProvider = services.BuildServiceProvider();

        var mainWindow = ServiceProvider.GetService<MainWindow>();
        mainWindow.Show();
    }
}
