using System.Windows;
using System.Windows.Media;
using HuntAndPeck.ViewModels;

namespace HuntAndPeck.Views;

/// <summary>
/// Interaction logic for DebugOverlayView.xaml
/// </summary>
public partial class DebugOverlayView
{
    public DebugOverlayView()
    {
        InitializeComponent();
    }

    private void DebugOverlayView_OnLoaded(object sender, RoutedEventArgs e)
    {
        var presentationSource = PresentationSource.FromVisual(this);
        if (presentationSource?.CompositionTarget == null) return;
        
        var m = presentationSource.CompositionTarget.TransformToDevice;
        var scaleX = m.M11;
        var scaleY = m.M22;

        // scale the items for non-96 DPIs
        layoutGrid.LayoutTransform = new ScaleTransform(1 / scaleX, 1 / scaleY);

        // resize the window for non-96 DPIs
        if (DataContext is DebugOverlayViewModel vm)
        {
            Left = vm.Bounds.Left / scaleX;
            Top = vm.Bounds.Top / scaleY;
            Width = vm.Bounds.Width / scaleX;
            Height = vm.Bounds.Height / scaleY;
        }
    }
}
