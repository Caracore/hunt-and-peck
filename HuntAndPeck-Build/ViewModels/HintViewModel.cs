using HuntAndPeck.Models;

namespace HuntAndPeck.ViewModels;

public class HintViewModel : NotifyPropertyChanged
{
    private string _label = string.Empty;
    private bool _active;
    private string _fontSizeReadValue = "14";

    public HintViewModel(Hint hint)
    {
        Hint = hint;
        FontSizeReadValue = AppSettings.FontSize;
    }

    public Hint Hint { get; set; }

    public bool Active
    {
        get => _active;
        set { _active = value; NotifyOfPropertyChange(); }
    }

    public string Label
    {
        get => _label;
        set { _label = value; NotifyOfPropertyChange(); }
    }

    public string FontSizeReadValue
    {
        get => _fontSizeReadValue;
        set { _fontSizeReadValue = value; NotifyOfPropertyChange(); }
    }
}
