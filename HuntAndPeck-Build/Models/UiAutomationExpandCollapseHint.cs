using System.Windows;
using System.Windows.Automation;

namespace HuntAndPeck.Models;

/// <summary>
/// Represents a Windows UI Automation based expand/collapse hint
/// </summary>
internal class UiAutomationExpandCollapseHint : Hint
{
    private readonly ExpandCollapsePattern _expandCollapsePattern;

    public UiAutomationExpandCollapseHint(IntPtr owningWindow, ExpandCollapsePattern expandCollapsePattern, Rect boundingRectangle)
        : base(owningWindow, boundingRectangle)
    {
        _expandCollapsePattern = expandCollapsePattern;
    }

    public override void Invoke()
    {
        try
        {
            switch (_expandCollapsePattern.Current.ExpandCollapseState)
            {
                case ExpandCollapseState.Collapsed:
                    _expandCollapsePattern.Expand();
                    break;
                case ExpandCollapseState.Expanded:
                case ExpandCollapseState.PartiallyExpanded:
                    _expandCollapsePattern.Collapse();
                    break;
            }
        }
        catch
        {
            // Element may have gone away
        }
    }
}
