using System.Windows;
using HuntAndPeck.NativeMethods;
using HuntAndPeck.Services;
using HuntAndPeck.ViewModels;
using HuntAndPeck.Views;
using Application = System.Windows.Application;

namespace HuntAndPeck;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly SingleLaunchMutex _singleLaunchMutex = new SingleLaunchMutex();
    private readonly UiAutomationHintProviderService _hintProviderService = new UiAutomationHintProviderService();
    private readonly HintLabelService _hintLabelService = new HintLabelService();
    private KeyListenerService? _keyListenerService;

    private void ShowOverlay(OverlayViewModel vm)
    {
        var view = new OverlayView
        {
            DataContext = vm
        };
        vm.CloseOverlay = () => view.Close();
        view.ShowDialog();
    }

    private void ShowDebugOverlay(DebugOverlayViewModel vm)
    {
        var view = new DebugOverlayView
        {
            DataContext = vm
        };
        view.ShowDialog();
    }

    private void ShowOptions(OptionsViewModel vm)
    {
        var view = new OptionsView
        {
            DataContext = vm
        };
        view.ShowDialog();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        if (e.Args.Contains("/hint"))
        {
            // support headless mode
            var session = _hintProviderService.EnumHints();
            if (session != null)
            {
                var overlayWindow = new OverlayView()
                {
                    DataContext = new OverlayViewModel(session, _hintLabelService)
                };
                var vm = (OverlayViewModel)overlayWindow.DataContext;
                vm.CloseOverlay = () =>
                {
                    overlayWindow.Close();
                    Current.Shutdown();
                };
                overlayWindow.Closed += (s, args) => Current.Shutdown();
                overlayWindow.Show();
            }
            else
            {
                Current.Shutdown();
            }
        }
        else if (e.Args.Contains("/tray"))
        {
            // support headless tray mode
            var taskbarHWnd = User32.FindWindow("Shell_traywnd", "");
            var session = _hintProviderService.EnumHints(taskbarHWnd);
            if (session != null)
            {
                var overlayWindow = new OverlayView()
                {
                    DataContext = new OverlayViewModel(session, _hintLabelService)
                };
                var vm = (OverlayViewModel)overlayWindow.DataContext;
                vm.CloseOverlay = () =>
                {
                    overlayWindow.Close();
                    Current.Shutdown();
                };
                overlayWindow.Closed += (s, args) => Current.Shutdown();
                overlayWindow.Show();
            }
            else
            {
                Current.Shutdown();
            }
        }
        else
        {
            // Prevent multiple startup in non-headless mode
            if (_singleLaunchMutex.AlreadyRunning)
            {
                Current.Shutdown();
                return;
            }

            // Create this as late as possible as it has a window
            _keyListenerService = new KeyListenerService();

            var shellViewModel = new ShellViewModel(
                ShowOverlay,
                ShowDebugOverlay,
                ShowOptions,
                _hintLabelService,
                _hintProviderService,
                _hintProviderService,
                _keyListenerService);

            var shellView = new ShellView
            {
                DataContext = shellViewModel
            };
            shellView.Show();
        }
        base.OnStartup(e);
    }
}

