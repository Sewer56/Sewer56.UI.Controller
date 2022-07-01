using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using NetEscapades.EnumGenerators;

namespace Sewer56.UI.Controller.Core.Enums;

/// <summary>
/// Contains a list of all available buttons.
/// </summary>
[EnumExtensions]
[Flags]
public enum Button : int
{
    /// <summary>
    /// No buttons
    /// </summary>
    None = 0,

    /// <summary>
    /// Navigates the menu up.
    /// </summary>
    [Description("Navigates the menu up.")]
    Up = 1 << 0,

    /// <summary>
    /// Navigates the menu down.
    /// </summary>
    [Description("Navigates the menu down.")]
    Down = 1 << 1,

    /// <summary>
    /// Navigates the menu left.
    /// </summary>
    [Description("Navigates the menu left.")]
    Left = 1 << 2,

    /// <summary>
    /// Navigates the menu right.
    /// </summary>
    [Description("Navigates the menu right.")]
    Right = 1 << 3,

    /// <summary>
    /// Click a button, etc.
    /// </summary>
    [Description("Clicks a button.")]
    Accept = 1 << 4,

    /// <summary>
    /// Go back etc.
    /// </summary>
    [Description("Closes the current non-main window.")]
    Decline = 1 << 5,

    /// <summary>
    /// Navigates to the next page, on supported applications.
    /// </summary>
    [Description("Navigates to the next page, on supported applications.")]
    NextPage = 1 << 6,

    /// <summary>
    /// Navigates to the last page, on supported applications.
    /// </summary>
    [Description("Navigates to the last page, on supported applications.")]
    LastPage = 1 << 7,

    /// <summary>
    /// Adds to the current value.
    /// </summary>
    [Description("+1 to the current value.")]
    Increment = 1 << 8,

    /// <summary>
    /// Decreases from the current value.
    /// </summary>
    [Description("-1 to the current value.")]
    Decrement = 1 << 9,

    /// <summary>
    /// Modifier key that allows for alternate behaviour on controls,
    /// e.g. increasing values using up/down/left/right.
    /// </summary>
    [Description("Allows for alternate behaviour on some element.\n" +
                 "e.g. Mod+Up increases the value by 1.")]
    Modifier = 1 << 10,
}

/// <summary/>
public static partial class ButtonExtensions
{
    /// <summary>
    /// Set of buttons that move the cursor.
    /// </summary>
    public const Button MovementButtons = Button.Left | Button.Up | Button.Down | Button.Right;

    /// <summary>
    /// Returns true if any of the movement buttons are present.
    /// </summary>
    /// <param name="buttons">The current pressed buttons.</param>
    public static bool HasMovementButtons(this Button buttons) => (buttons & MovementButtons) > 0;

    /// <summary>
    /// Returns true if any of the flags.
    /// </summary>
    /// <param name="buttons">The buttons to check.</param>
    /// <param name="flags">The flags.</param>
    public static bool HasAnyFlag(this Button buttons, Button flags) => (buttons & flags) > 0;

    /// <summary>
    /// Gets the description for an individual button.
    /// </summary>
    /// <param name="button">The button for which to get description.</param>
    /// <returns>null if not found, else description.</returns>
    public static string GetDescription(this Button button)
    {
        var fi = typeof(Button).GetField(button.ToStringFast());

        if (fi!.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes)
            return attributes.FirstOrDefault()?.Description;

        return null;
    }

    /// <summary>
    /// Gets the direction vector associated with the movement buttons used.
    /// </summary>
    /// <param name="buttons">The current pressed buttons.</param>
    /// <param name="flipX">Flips the direction in the X axis.</param>
    /// <param name="flipY">Flips the direction in the Y axis.</param>
    public static Vector2 GetDirectionVector(this Button buttons, bool flipX, bool flipY)
    {
        var result = new Vector2(0, 0);

        if ((buttons & Button.Left) == Button.Left)
            result.X -= 1.0f;

        if ((buttons & Button.Right) == Button.Right)
            result.X += 1.0f;

        if ((buttons & Button.Up) == Button.Up)
            result.Y += 1.0f;

        if ((buttons & Button.Down) == Button.Down)
            result.Y -= 1.0f;

        if (flipX)
            result.X = -result.X;

        if (flipY)
            result.Y = -result.Y;

        return result;
    }
}