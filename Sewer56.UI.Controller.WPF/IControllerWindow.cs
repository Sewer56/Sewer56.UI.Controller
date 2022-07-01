using Sewer56.UI.Controller.Core.Structures;

namespace Sewer56.UI.Controller.WPF;

/// <summary>
/// Represents an individual WPF window with controller support. 
/// Inherit this class in your window to receive controller input notifications. 
/// </summary>
public interface IControllerWindow
{
    /// <summary>
    /// Processes any additional inputs not handled by library.
    /// </summary>
    void ProcessInputs(in ControllerState state);
}