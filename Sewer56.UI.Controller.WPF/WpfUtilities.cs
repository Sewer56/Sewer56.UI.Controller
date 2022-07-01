using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace Sewer56.UI.Controller.WPF;

/// <summary>
/// Gets all visual children of a given element.
/// </summary>
public static class WpfUtilities
{
    /// <summary>
    /// Converts the given point to a vector.
    /// </summary>
    /// <param name="point">The point to convert to the vector.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 AsVector(this Point point) => new((float)point.X, (float)point.Y);

    /// <summary>
    /// Converts the given point to a vector.
    /// </summary>
    /// <param name="size">The size to convert to the vector.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 AsVector(this Size size) => new((float)size.Width, (float)size.Height);

    /// <summary>
    /// Finds all children of the current element using recursion.
    /// </summary>
    /// <param name="item">Current item in the recursion hierarchy.</param>
    /// <param name="exclude">Item to exclude (if present).</param>
    /// <param name="accumulator">The list that will receive all of the child UI items.</param>
    public static void FindSelectableChildren(DependencyObject item, UIElement exclude, ref SpanList<UIElement> accumulator)
    {
        var children = VisualTreeHelper.GetChildrenCount(item);
        for (int x = 0; x < children; x++)
        {
            var child = VisualTreeHelper.GetChild(item, x);
            if (child == null) 
                continue;

            if (child is UIElement { Focusable: true, IsVisible: true, IsEnabled: true } uiElement && uiElement != exclude)
                accumulator.Add(uiElement);

            FindSelectableChildren(child, exclude, ref accumulator);
        }
    }

    /// <summary>
    /// Tries to find a parent with a given type in the visual tree hierarchy.
    /// </summary>
    /// <param name="item">Current item in the visual tree hierarchy.</param>
    public static T? FindParent<T>(DependencyObject item)
    {
        var parent = VisualTreeHelper.GetParent(item);
        return parent switch
        {
            null => default,
            T value => value,
            _ => FindParent<T>(parent)
        };
    }

    /// <summary>
    /// Tries to get the currently focused element and its corresponding window.
    /// </summary>
    /// <param name="window">The currently selected window.</param>
    /// <param name="focused">The currently focused element.</param>
    [SkipLocalsInit]
    public static bool TryGetFocusedElementAndWindow(out Window? window, out UIElement? focused)
    {
        window = default;
        focused = Keyboard.FocusedElement as UIElement;

        if (focused == null)
            return false;

        window = Window.GetWindow(focused);
        return window != null;
    }
}

/// <summary>
/// Represents a span with list-like addition semantics.
/// </summary>
public ref struct SpanList<T>
{
    /// <summary>
    /// Items in this span list.
    /// </summary>
    public Span<T> Items;

    /// <summary>
    /// Current index of items in the list.
    /// </summary>
    public int Length;

    /// <summary>
    /// Creates a list-like wrapper for spans.
    /// </summary>
    /// <param name="items">The span of items to hold the results.</param>
    public SpanList(Span<T> items) : this() => Items = items;

    /// <summary>
    /// Adds an item onto this span list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public void Add(in T item) => Items[Length++] = item;
}