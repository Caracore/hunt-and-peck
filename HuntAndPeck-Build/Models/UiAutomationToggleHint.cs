using System.Windows;
using System.Windows.Automation;

namespace HuntAndPeck.Models;

/// <summary>
/// Represents a Windows UI Automation based toggle hint
/// </summary>
internal class UiAutomationToggleHint : Hint
{
    private readonly TogglePattern _togglePattern;

    public UiAutomationToggleHint(IntPtr owningWindow, TogglePattern togglePattern, Rect boundingRectangle)
        : base(owningWindow, boundingRectangle)
    {
        _togglePattern = togglePattern;
    }

    public override void Invoke()
    {
        try
        {
            _togglePattern.Toggle();
        }
        catch
        {
            // Element may have gone away
        }
    }
}
