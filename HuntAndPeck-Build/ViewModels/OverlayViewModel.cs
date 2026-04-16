using System.Collections.ObjectModel;
using System.Windows;
using HuntAndPeck.Models;
using HuntAndPeck.Services.Interfaces;

namespace HuntAndPeck.ViewModels;

internal class OverlayViewModel : NotifyPropertyChanged
{
    private Rect _bounds;
    private ObservableCollection<HintViewModel> _hints = new ObservableCollection<HintViewModel>();

    public OverlayViewModel(
        HintSession session,
        IHintLabelService hintLabelService)
    {
        _bounds = session.OwningWindowBounds;

        var labels = hintLabelService.GetHintStrings(session.Hints.Count);
        for (int i = 0; i < labels.Count; ++i)
        {
            var hint = session.Hints[i];
            _hints.Add(new HintViewModel(hint)
            {
                Label = labels[i],
                Active = false
            });
        }
    }

    /// <summary>
    /// Bounds in logical screen coordinates
    /// </summary>
    public Rect Bounds
    {
        get => _bounds;
        set
        {
            _bounds = value;
            NotifyOfPropertyChange();
        }
    }

    public ObservableCollection<HintViewModel> Hints
    {
        get => _hints;
        set
        {
            _hints = value;
            NotifyOfPropertyChange();
        }
    }

    public Action? CloseOverlay { get; set; }

    public string MatchString
    {
        set
        {
            foreach (var x in Hints)
            {
                x.Active = false;
            }

            var matching = Hints.Where(x => x.Label.StartsWith(value, StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (var x in matching)
            {
                x.Active = true;
            }

            if (matching.Length == 1)
            {
                matching.First().Hint.Invoke();
                CloseOverlay?.Invoke();
            }
        }
    }
}
