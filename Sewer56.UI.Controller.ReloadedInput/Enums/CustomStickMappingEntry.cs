using Reloaded.Input.Structs;
using Sewer56.UI.Controller.Core.Enums;
using Sewer56.UI.Controller.ReloadedInput.Structures;
using static Sewer56.UI.Controller.ReloadedInput.Enums.CustomStickMappingEntry;

namespace Sewer56.UI.Controller.ReloadedInput.Enums;

/// <summary>
/// Represents pre-programmed custom stick mapping entries.
/// </summary>
public enum CustomStickMappingEntry
{
    /// <summary>
    /// Maps horizontal analog stick movement to left/right (<see cref="Button.Left"/>/<see cref="Button.Right"/>) menu movement.
    /// </summary>
    HorizontalStickToLeftRight = 0,

    /// <summary>
    /// Maps vertical analog stick movement to left/right (<see cref="Button.Up"/>/<see cref="Button.Down"/>) menu movement.
    /// </summary>
    VerticalStickToUpDown = 1
}

/// <summary>
/// Extensions for <see cref="CustomStickMappingEntry"/> enum.
/// </summary>
public static class CustomStickMappingEntryExtensions
{
    /// <summary>
    /// Contains mappings for mapping sticks to predetermined buttons.
    /// </summary>
    public static readonly StickMappingEntry[] StickCustomMapEntries =
    {
        new()
        {
            Entry = new CustomMappingEntry("Menu Left/Right [Stick]", ReloadedInputController.IndexCustomStickMapping + (int)HorizontalStickToLeftRight, MappingType.Axis, "Navigates the Menu Left/Right"),
            ValueOnNegative = Button.Left,
            ValueOnPositive = Button.Right
        },
        new()
        {
            Entry = new CustomMappingEntry("Menu Up/Down [Stick]", ReloadedInputController.IndexCustomStickMapping + (int)VerticalStickToUpDown, MappingType.Axis, "Navigates the Menu Up/Down"),
            ValueOnNegative = Button.Down,
            ValueOnPositive = Button.Up
        }
    };
}