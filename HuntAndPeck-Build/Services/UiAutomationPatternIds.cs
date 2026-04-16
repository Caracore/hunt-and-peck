using System.Windows.Automation;

namespace HuntAndPeck.Services;

internal class UiAutomationPatternIds
{
    /// <summary>
    /// All possible patterns (excluding custom patterns) -> debug description
    /// </summary>
    private static readonly Dictionary<AutomationPattern, string> s_patternNames = new Dictionary<AutomationPattern, string>()
    {
        { InvokePattern.Pattern, "Invoke" },
        { SelectionPattern.Pattern, "Selection" },
        { ValuePattern.Pattern, "Value" },
        { RangeValuePattern.Pattern, "RangeValue" },
        { ScrollPattern.Pattern, "Scroll" },
        { ExpandCollapsePattern.Pattern, "ExpandCollapse" },
        { GridPattern.Pattern, "Grid" },
        { GridItemPattern.Pattern, "GridItem" },
        { MultipleViewPattern.Pattern, "MultipleView" },
        { WindowPattern.Pattern, "Window" },
        { SelectionItemPattern.Pattern, "SelectionItem" },
        { DockPattern.Pattern, "Dock" },
        { TablePattern.Pattern, "Table" },
        { TableItemPattern.Pattern, "TableItem" },
        { TextPattern.Pattern, "Text" },
        { TogglePattern.Pattern, "Toggle" },
        { TransformPattern.Pattern, "Transform" },
        { ScrollItemPattern.Pattern, "ScrollItem" },
        { ItemContainerPattern.Pattern, "ItemContainer" },
        { VirtualizedItemPattern.Pattern, "VirtualizedItem" },
        { SynchronizedInputPattern.Pattern, "SynchronizedInput" },
    };

    public static Dictionary<AutomationPattern, string> PatternNames => s_patternNames;
}
