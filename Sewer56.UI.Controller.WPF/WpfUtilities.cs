using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

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
    public static void FindSelectableChildren(DependencyObject item, UIElement? exclude, ref SpanList<UIElement> accumulator) => FindSelectableChildrenEx(item, item, exclude, ref accumulator);

    /// <summary>
    /// Finds all children of the current element using recursion.
    /// </summary>
    /// <param name="parent">The parent/root from wihch we are searching from.</param>
    /// <param name="item">Current item in the recursion hierarchy.</param>
    /// <param name="exclude">Item to exclude (if present).</param>
    /// <param name="accumulator">The list that will receive all of the child UI items.</param>
    public static void FindSelectableChildrenEx(DependencyObject parent, DependencyObject item, UIElement? exclude, ref SpanList<UIElement> accumulator)
    {
        var children = VisualTreeHelper.GetChildrenCount(item);
        for (int x = 0; x < children; x++)
        {
            var child = VisualTreeHelper.GetChild(item, x);
            if (child == null!)
                continue;

            if (child is UIElement { Focusable: true, IsVisible: true, IsEnabled: true } uiElement && uiElement != exclude 
                && HitTester.IsElementClickable((UIElement)parent, uiElement))
                accumulator.Add(uiElement);

            FindSelectableChildrenEx(parent, child, exclude, ref accumulator);
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
    /// Tries to find a parent with a given type in the visual tree hierarchy.
    /// </summary>
    /// <param name="item">Current item in the visual tree hierarchy.</param>
    /// <param name="type">The type of parent to find.</param>
    public static object? FindParent(DependencyObject item, Type type)
    {
        try
        {
            var parent = VisualTreeHelper.GetParent(item);
            if (parent == null)
                return default;

            return parent.GetType() == type ? parent
                                            : FindParent(parent, type);
        }
        catch (Exception)
        {
            // FlowDocument, etc.
            return default;
        }
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

    /// <summary>
    /// Tries to get the currently focused element and its corresponding window.
    /// </summary>
    /// <param name="window">The currently selected window.</param>
    [SkipLocalsInit]
    public static bool TryGetFocusedWindow(out Window? window)
    {
        window = default;

        // Can only get focused window.
        foreach (var candidate in Application.Current.Windows)
        {
            var windo = candidate as Window;
            if (windo is { IsActive: true })
            {
                window = windo;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines if an item is a child of a given parent.
    /// </summary>
    /// <param name="child">The child to check if within parent.</param>
    /// <param name="parent">The parent element.</param>
    public static bool IsElementChild(DependencyObject parent, DependencyObject child)
    {
        if (child.GetHashCode() == parent.GetHashCode())
            return true;

        foreach (DependencyObject parentChild in EnumerateChildren<DependencyObject>(parent))
        {
            if (parentChild.GetHashCode() == child.GetHashCode())
                return true;
        }

        return false;
    }

    /// <summary>
    /// Enumerates list of all the children of the given object.
    /// </summary>
    /// <typeparam name="T">The type of child to return.</typeparam>
    /// <param name="parent">The parent to check children of.</param>
    public static IEnumerable<T> EnumerateChildren<T>(DependencyObject? parent) where T : DependencyObject
    {
        if (parent == null)
            yield break;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject? child = VisualTreeHelper.GetChild(parent, i);
            if (child != null! && child is T)
                yield return (T)child;

            foreach (T childOfChild in EnumerateChildren<T>(child))
                yield return childOfChild;
        }
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