using System.Windows.Forms;
using HuntAndPeck.NativeMethods;
using HuntAndPeck.Services.Interfaces;

namespace HuntAndPeck.Services;

internal class KeyListenerService : Form, IKeyListenerService, IDisposable
{
    public event EventHandler? OnHotKeyActivated;
    public event EventHandler? OnTaskbarHotKeyActivated;
    public event EventHandler? OnDebugHotKeyActivated;

    /// <summary>
    /// Global counter for assigning ids to identify the hot key registration
    /// </summary>
    private int _hotkeyIdCounter = 0;

    private HotKey? _hotKey;
    private HotKey? _taskbarHotKey;
    private HotKey? _debugHotKey;

    /// <summary>
    /// Re-registers the current hotkey, unregistering any previous key
    /// </summary>
    private void ReRegisterHotKey(HotKey hotKey)
    {
        // Already registered, have to unregister first
        if (hotKey.RegistrationId > 0)
        {
            User32.UnregisterHotKey(Handle, hotKey.RegistrationId);
        }

        hotKey.RegistrationId = _hotkeyIdCounter++;
        var result = User32.RegisterHotKey(Handle, hotKey.RegistrationId, (uint)hotKey.Modifier, (uint)hotKey.Keys);
        if (!result)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to register hotkey: {hotKey.Keys} with modifier {hotKey.Modifier}");
        }
    }

    /// <summary>
    /// Gets/sets the current hotkey
    /// </summary>
    /// <remarks>Changing this will cause the current hotkey to be unregistered</remarks>
    public HotKey? HotKey
    {
        get => _hotKey;
        set
        {
            _hotKey = value;
            if (_hotKey != null) ReRegisterHotKey(_hotKey);
        }
    }

    /// <summary>
    /// Gets/sets the current task bar hotkey
    /// </summary>
    /// <remarks>Changing this will cause the current hotkey to be unregistered</remarks>
    public HotKey? TaskbarHotKey
    {
        get => _taskbarHotKey;
        set
        {
            _taskbarHotKey = value;
            if (_taskbarHotKey != null) ReRegisterHotKey(_taskbarHotKey);
        }
    }

    public HotKey? DebugHotKey
    {
        get => _debugHotKey;
        set
        {
            _debugHotKey = value;
            if (_debugHotKey != null) ReRegisterHotKey(_debugHotKey);
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == Constants.WM_HOTKEY)
        {
            var e = new HotKeyEventArgs(m.LParam);

            // Normal hotkey
            if (_hotKey != null &&
                e.Key == _hotKey.Keys &&
                e.Modifiers == _hotKey.Modifier)
            {
                OnHotKeyActivated?.Invoke(this, EventArgs.Empty);
            }

            // Task bar hotkey
            if (_taskbarHotKey != null &&
                e.Key == _taskbarHotKey.Keys &&
                e.Modifiers == _taskbarHotKey.Modifier)
            {
                OnTaskbarHotKeyActivated?.Invoke(this, EventArgs.Empty);
            }

            // Debug hotkey
            if (_debugHotKey != null &&
                e.Key == _debugHotKey.Keys &&
                e.Modifiers == _debugHotKey.Modifier)
            {
                OnDebugHotKeyActivated?.Invoke(this, EventArgs.Empty);
            }
        }

        base.WndProc(ref m);
    }

    protected override void SetVisibleCore(bool value)
    {
        // Ensures that the window will never be displayed
        base.SetVisibleCore(false);
    }
}
