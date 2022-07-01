using Sewer56.UI.Controller.Core.Structures;

namespace Sewer56.UI.Controller.Core.Interfaces;

/// <summary>
/// Interface used to add controller support.
/// </summary>
public interface IController
{
    /// <summary>
    /// Returns the inputs for the current frame.
    /// </summary>
    FrameInputs GetInputs();
}