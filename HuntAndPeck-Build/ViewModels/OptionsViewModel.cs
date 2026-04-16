using System.ComponentModel;

namespace HuntAndPeck.ViewModels;

internal class OptionsViewModel : INotifyPropertyChanged
{
    public OptionsViewModel()
    {
        DisplayName = "Options";
        FontSize = AppSettings.FontSize;
    }

    public string DisplayName { get; set; }

    private string _fontSize = "14";
    public string FontSize
    {
        get => _fontSize;
        set
        {
            if (_fontSize != value)
            {
                _fontSize = value;
                OnPropertyChanged(nameof(FontSize));
                AppSettings.FontSize = value;
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
