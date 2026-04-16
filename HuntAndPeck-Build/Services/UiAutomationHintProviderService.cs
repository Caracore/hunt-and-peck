using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using HuntAndPeck.Extensions;
using HuntAndPeck.Models;
using HuntAndPeck.NativeMethods;
using HuntAndPeck.Services.Interfaces;

namespace HuntAndPeck.Services;

internal class UiAutomationHintProviderService : IHintProviderService, IDebugHintProviderService
{
    public HintSession? EnumHints()
    {
        var foregroundWindow = User32.GetForegroundWindow();
        if (foregroundWindow == IntPtr.Zero)
        {
            return null;
        }
        return EnumHints(foregroundWindow);
    }

    public HintSession? EnumHints(IntPtr hWnd)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var session = EnumWindowHints(hWnd, CreateHint);
        sw.Stop();

        Debug.WriteLine("Enumeration of hints took {0} ms", sw.ElapsedMilliseconds);
        return session;
    }

    public HintSession? EnumDebugHints()
    {
        var foregroundWindow = User32.GetForegroundWindow();
        if (foregroundWindow == IntPtr.Zero)
        {
            return null;
        }
        return EnumDebugHints(foregroundWindow);
    }

    public HintSession? EnumDebugHints(IntPtr hWnd)
    {
        return EnumWindowHints(hWnd, CreateDebugHint);
    }

    /// <summary>
    /// Enumerates all the hints from the given window
    /// </summary>
    /// <param name="hWnd">The window to get hints from</param>
    /// <param name="hintFactory">The factory to use to create each hint in the session</param>
    /// <returns>A hint session</returns>
    private HintSession EnumWindowHints(IntPtr hWnd, Func<IntPtr, Rect, AutomationElement, Hint?> hintFactory)
    {
        var result = new List<Hint>();
        var elements = EnumElements(hWnd);

        // Window bounds
        var rawWindowBounds = new RECT();
        User32.GetWindowRect(hWnd, ref rawWindowBounds);
        Rect windowBounds = rawWindowBounds;

        foreach (var element in elements)
        {
            try
            {
                var boundingRect = element.Current.BoundingRectangle;
                if (!boundingRect.IsEmpty && boundingRect.Width > 0 && boundingRect.Height > 0)
                {
                    // Convert the bounding rect to logical coords
                    var logicalRect = boundingRect.PhysicalToLogicalRect(hWnd);
                    if (!logicalRect.IsEmpty)
                    {
                        var windowCoords = boundingRect.ScreenToWindowCoordinates(windowBounds);
                        var hint = hintFactory(hWnd, windowCoords, element);
                        if (hint != null)
                        {
                            result.Add(hint);
                        }
                    }
                }
            }
            catch
            {
                // Element may have gone away
            }
        }

        return new HintSession
        {
            Hints = result,
            OwningWindow = hWnd,
            OwningWindowBounds = windowBounds,
        };
    }

    /// <summary>
    /// Enumerates the automation elements from the given window
    /// </summary>
    /// <param name="hWnd">The window handle</param>
    /// <returns>All of the automation elements found</returns>
    private List<AutomationElement> EnumElements(IntPtr hWnd)
    {
        var result = new List<AutomationElement>();
        
        try
        {
            var automationElement = AutomationElement.FromHandle(hWnd);

            var conditionEnabled = new PropertyCondition(AutomationElement.IsEnabledProperty, true);
            var conditionOnScreen = new PropertyCondition(AutomationElement.IsOffscreenProperty, false);
            var condition = new AndCondition(conditionEnabled, conditionOnScreen);

            var elementCollection = automationElement.FindAll(TreeScope.Descendants, condition);
            
            foreach (AutomationElement element in elementCollection)
            {
                result.Add(element);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error enumerating elements: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Creates a UI Automation element from the given automation element
    /// </summary>
    /// <param name="owningWindow">The owning window</param>
    /// <param name="hintBounds">The hint bounds</param>
    /// <param name="automationElement">The associated automation element</param>
    /// <returns>The created hint, else null if the hint could not be created</returns>
    private Hint? CreateHint(IntPtr owningWindow, Rect hintBounds, AutomationElement automationElement)
    {
        try
        {
            if (automationElement.TryGetCurrentPattern(InvokePattern.Pattern, out var invokePatternObj))
            {
                return new UiAutomationInvokeHint(owningWindow, (InvokePattern)invokePatternObj, hintBounds);
            }

            if (automationElement.TryGetCurrentPattern(TogglePattern.Pattern, out var togglePatternObj))
            {
                return new UiAutomationToggleHint(owningWindow, (TogglePattern)togglePatternObj, hintBounds);
            }

            if (automationElement.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var selectPatternObj))
            {
                return new UiAutomationSelectHint(owningWindow, (SelectionItemPattern)selectPatternObj, hintBounds);
            }

            if (automationElement.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out var expandCollapsePatternObj))
            {
                return new UiAutomationExpandCollapseHint(owningWindow, (ExpandCollapsePattern)expandCollapsePatternObj, hintBounds);
            }

            if (automationElement.TryGetCurrentPattern(ValuePattern.Pattern, out var valuePatternObj))
            {
                var valuePattern = (ValuePattern)valuePatternObj;
                if (!valuePattern.Current.IsReadOnly)
                {
                    return new UiAutomationFocusHint(owningWindow, automationElement, hintBounds);
                }
            }

            if (automationElement.TryGetCurrentPattern(RangeValuePattern.Pattern, out var rangeValuePatternObj))
            {
                var rangeValuePattern = (RangeValuePattern)rangeValuePatternObj;
                if (!rangeValuePattern.Current.IsReadOnly)
                {
                    return new UiAutomationFocusHint(owningWindow, automationElement, hintBounds);
                }
            }

            return null;
        }
        catch (Exception)
        {
            // May have gone
            return null;
        }
    }

    /// <summary>
    /// Creates a debug hint
    /// </summary>
    /// <param name="owningWindow">The window that owns the hint</param>
    /// <param name="hintBounds">The hint bounds</param>
    /// <param name="automationElement">The automation element</param>
    /// <returns>A debug hint</returns>
    private Hint? CreateDebugHint(IntPtr owningWindow, Rect hintBounds, AutomationElement automationElement)
    {
        // Enumerate all possible patterns. Note that the performance of this is *very* bad -- hence debug only.
        var programmaticNames = new List<string>();

        foreach (var pn in UiAutomationPatternIds.PatternNames)
        {
            try
            {
                if (automationElement.TryGetCurrentPattern(pn.Key, out _))
                {
                    programmaticNames.Add(pn.Value);
                }
            }
            catch (Exception)
            {
            }
        }

        if (programmaticNames.Any())
        {
            return new DebugHint(owningWindow, hintBounds, programmaticNames.ToList());
        }

        return null;
    }
}
