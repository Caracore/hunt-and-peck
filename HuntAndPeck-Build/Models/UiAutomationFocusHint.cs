using System.Windows;
using System.Windows.Automation;

namespace HuntAndPeck.Models;

/// <summary>
/// Represents a Windows UI Automation based focus hint
/// </summary>
internal class UiAutomationFocusHint : Hint
{
    private readonly AutomationElement _automationElement;

    public UiAutomationFocusHint(IntPtr owningWindow, AutomationElement automationElement, Rect boundingRectangle)
        : base(owningWindow, boundingRectangle)
    {
        _automationElement = automationElement;
    }

    public override void Invoke()
    {
        try
        {
            _automationElement.SetFocus();
        }
        catch
        {
            // Element may have gone away
        }
    }
}
