using System.Reflection;
using System.Windows.Input;
using System.Windows;
#pragma warning disable CS1591

namespace Sewer56.UI.Controller.WPF;

public struct KeyboardNav
{
    private static PropertyInfo _alwaysShowFocusVisual = typeof(KeyboardNavigation).GetProperty("AlwaysShowFocusVisual", BindingFlags.NonPublic | BindingFlags.Static)!;
    private static MethodInfo _showFocusVisual = typeof(KeyboardNavigation).GetMethod("ShowFocusVisual", BindingFlags.NonPublic | BindingFlags.Static)!;
    
    public static bool AlwaysShowFocusVisual
    {
        get => (bool)_alwaysShowFocusVisual.GetValue(null, null)!;
        set => _alwaysShowFocusVisual.SetValue(null, value, null);
    }
    
    /// <summary>
    /// Focuses the specified element and shows the focus visual style.
    /// </summary>
    /// <param name="element">The element.</param>
    public static void Focus(UIElement element)
    {
        var alwaysShowFocusVisual = AlwaysShowFocusVisual;
        AlwaysShowFocusVisual = true;
        try
        {
            Keyboard.Focus(element);
            _showFocusVisual.Invoke(null, null);
        }
        finally
        {
            AlwaysShowFocusVisual = alwaysShowFocusVisual;
        }
    }
}