using System;
using System.Windows.Media;
using System.Windows;

namespace Sewer56.UI.Controller.WPF;

/// <summary>
/// Utility for hit testing objects to determine if they are clickable.
/// </summary>
internal static class HitTester
{
    /// <summary>
    /// Checks if a given element of Type T is at least partially clickable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="container">The container to check if the control is hittable within.</param>
    /// <param name="element">The element to check if it can be hit.</param>
    /// <returns>True if the element can be clicked with the mouse.</returns>
    internal static bool IsElementClickable(UIElement container, UIElement? element)
    {
        if (element == null)
            return false;

        var absolutePos = GetRelativePlacement((FrameworkElement)container, (FrameworkElement)element);
        return IsPointClickable(container, element, new Point(absolutePos.TopLeft.X + 1, absolutePos.TopLeft.Y + 1)) || // Top left
               IsPointClickable(container, element, new Point(absolutePos.BottomLeft.X + 1, absolutePos.BottomLeft.Y - 1)) || // Bottom left
               IsPointClickable(container, element, new Point(absolutePos.TopRight.X - 1, absolutePos.TopRight.Y + 1)) || // Top right
               IsPointClickable(container, element, new Point(absolutePos.BottomRight.X - 1, absolutePos.BottomRight.Y - 1)) || // Bottom right
               IsPointClickable(container, element, new Point(absolutePos.TopLeft.X + absolutePos.Width / 2, absolutePos.TopLeft.Y + absolutePos.Height / 2)); // Middle
    }

    /// <summary>
    /// Determines if a given point can be clicked by the mouse.
    /// </summary>
    /// <param name="container">The container relative to which the position of the element should be determined..</param>
    /// <param name="element">The element whose position to get.</param>
    /// <param name="position">The position to test in screen coordinates.</param>
    /// <returns>Whether clicking at given point will hit the item.</returns>
    private static bool IsPointClickable(UIElement container, UIElement element, Point position)
    {
        var hitTestResult = HitTest(position, container, element.GetType());
        if (hitTestResult != null)
            return WpfUtilities.IsElementChild(element, hitTestResult);

        return false;
    }

    /// <summary>
    /// Performs a hit test to determine the element hit if clicking at a position.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="position">The position to check.</param>
    /// <param name="container">The container to check.</param>
    /// <param name="desiredType">Type of item to hit test for.</param>
    /// <returns>The object hit.</returns>
    private static DependencyObject? HitTest(Point position, UIElement container, Type desiredType)
    {
        var hitTestResult = container.InputHitTest(position);
        if (hitTestResult is DependencyObject obj && (hitTestResult.GetType() == desiredType || WpfUtilities.FindParent(obj, desiredType) != null))
            return obj;

        return null;
    }

    /// <summary>
    /// Retrieves the placement of the element relative to the container.
    /// </summary>
    /// <param name="container">The container relative to which the position of the element should be determined..</param>
    /// <param name="element">The element whose position to get.</param>
    private static Rect GetRelativePlacement(FrameworkElement container, FrameworkElement element)
    {
        var elementPos   = element.PointToScreen(new Point(0, 0));
        var containerPos = container.PointToScreen(new Point(0, 0));
        var relativePos  = new Point(elementPos.X - containerPos.X, elementPos.Y - containerPos.Y);
        return new Rect(relativePos.X, relativePos.Y, element.ActualWidth, element.ActualHeight);
    }
}