using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Kryptograf.Commands;
using Kryptograf.Enums;
using Kryptograf.Helpers;
using Kryptograf.Services;

namespace Kryptograf.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IEncryptionService _encryptionService;

    private bool _isDarkMode;
    private string _inputText;
    private string _encryptedText;
    private string _decryptedText;
    private string _password;
    private bool _isPasswordVisible;
    private string _passwordVisibilityText;

    public event PropertyChangedEventHandler PropertyChanged;

    public MainWindowViewModel(IEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
        EncryptCommand = new RelayCommand(Encrypt, CanEncrypt);
        DecryptCommand = new RelayCommand(Decrypt, CanDecrypt);
        CleanTextCommand = new CleanTextCommand(CleanText, IsTextPropertyContainsValue);
        TogglePasswordVisibilityCommand = new RelayCommand(TogglePasswordVisibility, CanTogglePasswordVisibility);
        CopyTextToClipboardCommand = new CopyTextToClipboardCommand(CopyTextToClipboard, IsTextPropertyContainsValue);

        IsDarkMode = false;
        IsPasswordVisible = false;
    }

    public bool IsDarkMode
    {
        set
        {
            _isDarkMode = value;
            ThemeHelper.ApplyTheme(_isDarkMode ? Theme.Dark : Theme.Light);
        }
    }

    public string InputText
    {
        get => _inputText;
        set
        {
            _inputText = value;
            OnPropertyChanged(nameof(InputText));
            (EncryptCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DecryptCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CleanTextCommand as CleanTextCommand)?.RaiseCanExecuteChanged(nameof(InputText));
        }
    }

    public string EncryptedText
    {
        get => _encryptedText;
        set
        {
            _encryptedText = value;
            OnPropertyChanged(nameof(EncryptedText));
            (CleanTextCommand as CleanTextCommand)?.RaiseCanExecuteChanged(nameof(DecryptedText));
            (CopyTextToClipboardCommand as CopyTextToClipboardCommand)?.RaiseCanExecuteChanged(nameof(EncryptedText));
        }
    }

    public string DecryptedText
    {
        get => _decryptedText;
        set
        {
            _decryptedText = value;
            OnPropertyChanged(nameof(DecryptedText));
            (CleanTextCommand as CleanTextCommand)?.RaiseCanExecuteChanged(nameof(DecryptedText));
            (CopyTextToClipboardCommand as CopyTextToClipboardCommand)?.RaiseCanExecuteChanged(nameof(DecryptedText));
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
            (EncryptCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DecryptCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (TogglePasswordVisibilityCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CleanTextCommand as CleanTextCommand)?.RaiseCanExecuteChanged(nameof(Password));
        }
    }

    public bool IsPasswordVisible
    {
        get => _isPasswordVisible;
        set
        {
            _isPasswordVisible = value;
            PasswordVisibilityText = _isPasswordVisible ? "Hide" : "Show";
            OnPropertyChanged(nameof(IsPasswordVisible));
        }
    }

    public string PasswordVisibilityText
    {
        get => _passwordVisibilityText;
        set
        {
            _passwordVisibilityText = value;
            OnPropertyChanged(nameof(PasswordVisibilityText));
        }
    }
    public ICommand EncryptCommand { get; }
    public ICommand DecryptCommand { get; }
    public ICommand CleanTextCommand { get; }
    public ICommand TogglePasswordVisibilityCommand { get; }
    public ICommand CopyTextToClipboardCommand { get; }

    private bool CanEncrypt() => !string.IsNullOrWhiteSpace(InputText) && !string.IsNullOrWhiteSpace(Password);

    private bool CanDecrypt() => !string.IsNullOrWhiteSpace(InputText) && !string.IsNullOrWhiteSpace(Password);

    private bool IsTextPropertyContainsValue(string propertyName)
    {
        var property = GetType().GetProperty(propertyName);
        return !string.IsNullOrEmpty(property?.GetValue(this) as string);
    }

    private bool CanTogglePasswordVisibility() => !string.IsNullOrWhiteSpace(Password);

    private void Encrypt()
    {
        EncryptedText = _encryptionService.Encrypt(InputText, Password);
    }

    private void Decrypt()
    {
        var decryptionResult = _encryptionService.Decrypt(InputText, Password);

        if (decryptionResult is null)
        {
            MessageBox.Show("Błąd deszyfrowania!", "Task failed succesfully", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }
        DecryptedText = decryptionResult;
    }

    private void CleanText(string propertyName)
    {
        var property = GetType().GetProperty(propertyName);
        property?.SetValue(this, string.Empty);
    }

    private void CopyTextToClipboard(string propertyName)
    {
        var property = GetType().GetProperty(propertyName);
        Clipboard.SetText(property?.GetValue(this) as string ?? string.Empty);
    }

    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
