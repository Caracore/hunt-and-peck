using System.Windows;
using HuntAndPeck.Models;

namespace HuntAndPeck.ViewModels;

public class DebugOverlayViewModel : NotifyPropertyChanged
{
    private Rect _bounds;

    public DebugOverlayViewModel(HintSession session)
    {
        Bounds = session.OwningWindowBounds;
        Hints = session.Hints.OfType<DebugHint>().Select(x => new DebugHintViewModel(x)).ToList();
    }

    public List<DebugHintViewModel> Hints { get; set; }

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
}
