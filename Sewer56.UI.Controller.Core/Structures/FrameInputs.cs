using Sewer56.UI.Controller.Core.Enums;

namespace Sewer56.UI.Controller.Core.Structures;

/// <summary>
/// Provides inputs for the current frame.
/// </summary>
public struct FrameInputs
{
    /// <summary>
    /// The buttons pressed. 
    /// </summary>
    public readonly Button Buttons;

    /// <summary/>
    public FrameInputs(Button buttons) => Buttons = buttons;
}