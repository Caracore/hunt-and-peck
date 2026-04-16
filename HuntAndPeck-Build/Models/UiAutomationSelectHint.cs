using System.Windows;
using System.Windows.Automation;

namespace HuntAndPeck.Models;

/// <summary>
/// Represents a Windows UI Automation based select hint
/// </summary>
internal class UiAutomationSelectHint : Hint
{
    private readonly SelectionItemPattern _selectPattern;

    public UiAutomationSelectHint(IntPtr owningWindow, SelectionItemPattern selectPattern, Rect boundingRectangle)
        : base(owningWindow, boundingRectangle)
    {
        _selectPattern = selectPattern;
    }

    public override void Invoke()
    {
        try
        {
            _selectPattern.Select();
        }
        catch
        {
            // Element may have gone away
        }
    }
}
