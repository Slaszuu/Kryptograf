using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Kryptograf.Commands;

public class CleanTextCommand : ICommand
{
    private readonly Action<string> _execute;
    private readonly Func<string, bool> _canExecute;

    public CleanTextCommand(Action<string> execute, Func<string, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute is null || _canExecute(parameter as string);
    }

    public void Execute(object parameter)
    {
        _execute(parameter as string);
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged(string propertyName)
    {
        CanExecuteChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
