using System.Windows;
using System.Windows.Automation;

namespace HuntAndPeck.Models;

/// <summary>
/// Represents a Windows UI Automation based invoke hint
/// </summary>
internal class UiAutomationInvokeHint : Hint
{
    private readonly InvokePattern _invokePattern;

    public UiAutomationInvokeHint(IntPtr owningWindow, InvokePattern invokePattern, Rect boundingRectangle)
        : base(owningWindow, boundingRectangle)
    {
        _invokePattern = invokePattern;
    }

    public override void Invoke()
    {
        try
        {
            _invokePattern.Invoke();
        }
        catch
        {
            // Element may have gone away
        }
    }
}
