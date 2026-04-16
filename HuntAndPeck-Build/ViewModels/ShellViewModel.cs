using System.Windows.Forms;
using HuntAndPeck.NativeMethods;
using HuntAndPeck.Services.Interfaces;
using Application = System.Windows.Application;

namespace HuntAndPeck.ViewModels;

internal class ShellViewModel
{
    private readonly Action<OverlayViewModel> _showOverlay;
    private readonly Action<DebugOverlayViewModel> _showDebugOverlay;
    private readonly Action<OptionsViewModel> _showOptions;
    private readonly IHintLabelService _hintLabelService;
    private readonly IHintProviderService _hintProviderService;
    private readonly IDebugHintProviderService _debugHintProviderService;

    public ShellViewModel(
        Action<OverlayViewModel> showOverlay,
        Action<DebugOverlayViewModel> showDebugOverlay,
        Action<OptionsViewModel> showOptions,
        IHintLabelService hintLabelService,
        IHintProviderService hintProviderService,
        IDebugHintProviderService debugHintProviderService,
        IKeyListenerService keyListener)
    {
        _showOverlay = showOverlay;
        _showDebugOverlay = showDebugOverlay;
        _showOptions = showOptions;
        _hintLabelService = hintLabelService;
        _hintProviderService = hintProviderService;
        _debugHintProviderService = debugHintProviderService;

        // Alt+H for hints (works on all keyboard layouts including AZERTY)
        keyListener.HotKey = new HotKey
        {
            Keys = Keys.H,
            Modifier = KeyModifier.Alt
        };

        // Ctrl+H for taskbar hints
        keyListener.TaskbarHotKey = new HotKey
        {
            Keys = Keys.H,
            Modifier = KeyModifier.Control
        };

#if DEBUG
        keyListener.DebugHotKey = new HotKey
        {
            Keys = Keys.H,
            Modifier = KeyModifier.Alt | KeyModifier.Shift
        };
#endif

        keyListener.OnHotKeyActivated += _keyListener_OnHotKeyActivated;
        keyListener.OnTaskbarHotKeyActivated += _keyListener_OnTaskbarHotKeyActivated;
        keyListener.OnDebugHotKeyActivated += _keyListener_OnDebugHotKeyActivated;

        ShowOptionsCommand = new DelegateCommand(ShowOptions);
        ExitCommand = new DelegateCommand(Exit);
    }

    public DelegateCommand ShowOptionsCommand { get; }
    public DelegateCommand ExitCommand { get; }

    private void _keyListener_OnHotKeyActivated(object? sender, EventArgs e)
    {
        var session = _hintProviderService.EnumHints();
        if (session != null)
        {
            var vm = new OverlayViewModel(session, _hintLabelService);
            _showOverlay(vm);
        }
    }

    private void _keyListener_OnTaskbarHotKeyActivated(object? sender, EventArgs e)
    {
        var taskbarHWnd = User32.FindWindow("Shell_traywnd", "");
        var session = _hintProviderService.EnumHints(taskbarHWnd);
        if (session != null)
        {
            var vm = new OverlayViewModel(session, _hintLabelService);
            _showOverlay(vm);
        }
    }

    private void _keyListener_OnDebugHotKeyActivated(object? sender, EventArgs e)
    {
        var session = _debugHintProviderService.EnumDebugHints();
        if (session != null)
        {
            var vm = new DebugOverlayViewModel(session);
            _showDebugOverlay(vm);
        }
    }

    public void Exit()
    {
        Application.Current.Shutdown();
    }

    public void ShowOptions()
    {
        var vm = new OptionsViewModel();
        _showOptions(vm);
    }
}
