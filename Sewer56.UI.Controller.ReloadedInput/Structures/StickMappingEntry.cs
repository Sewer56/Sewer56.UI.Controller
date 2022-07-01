using Reloaded.Input.Structs;
using Sewer56.UI.Controller.Core.Enums;

namespace Sewer56.UI.Controller.ReloadedInput.Structures;

/// <summary>
/// Represents an individual entry that maps a stick movement to a button.
/// </summary>
public struct StickMappingEntry
{
    /// <summary>
    /// Minimum value before the stick is considered to press the button.
    /// </summary>
    public const float MinAxis = AxisSet.MaxValue * 0.75f;

    /// <summary>
    /// Details about the custom stick mapping.
    /// </summary>
    public CustomMappingEntry Entry;

    /// <summary>
    /// The button pressed if the stick has a negative value.
    /// (value &lt; <see cref="MinAxis"/>)
    /// </summary>
    public Button ValueOnNegative;

    /// <summary>
    /// The button pressed if the stick has a positive value.
    /// (value &gt; <see cref="MinAxis"/>)
    /// </summary>
    public Button ValueOnPositive;

    /// <summary>
    /// Gets the pressed button for a given float value.
    /// </summary>
    /// <param name="value">The float value in question.</param>
    public Button GetValue(float value)
    {
        return value switch
        {
            > MinAxis => ValueOnPositive,
            < -MinAxis => ValueOnNegative,
            _ => Button.None
        };
    }
}