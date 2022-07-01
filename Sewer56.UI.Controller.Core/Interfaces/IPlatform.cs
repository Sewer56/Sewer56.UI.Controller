using System;
using System.Numerics;
using Sewer56.UI.Controller.Core.Structures;

namespace Sewer56.UI.Controller.Core.Interfaces;

/// <summary>
/// Abstracts a GUI platform such as WPF, Avalonia or WinForms.
/// </summary>
public interface IPlatform
{
    /// <summary>
    /// Maximum number of controls supported.
    /// </summary>
    public const int MaxControls = 2048;

    // Events with platform-specific custom logic

    /// <summary>
    /// An event that gets called on every frame/when the UI is to be updated.
    /// Runs the internal library logic.
    /// </summary>
    event Action OnUpdate;

    /// <summary>
    /// Set to true to flip movement in X axis.
    /// </summary>
    bool FlipX { get; }

    /// <summary>
    /// Set to true to flip movement in Y axis.
    /// </summary>
    bool FlipY { get; }

    /// <summary>
    /// Processes any additional inputs not handled by library.
    /// </summary>
    void ProcessInputs(in ControllerState state);

    // Events querying data from the platform.
    
    /// <summary>
    /// Gets the positions of all currently selectable on-screen elements.
    /// This excludes the currently selected element.
    /// </summary>
    /// <param name="buffer">
    ///     The buffer to place your controls in.
    ///     Fill it in, then slice it with the number of elements added.
    /// </param>
    /// <returns>Sliced buffer with all selectable element positions.</returns>
    Span<Vector2> GetSelectableControls(Span<Vector2> buffer);

    /// <summary>
    /// Retrieves the current cursor position.
    /// </summary>
    Vector2 GetCurrentPosition();

    /// <summary>
    /// Returns true if the window containing the current element has focus.
    /// </summary>
    bool HasFocus();

    /// <summary>
    /// Selects a control with the specified index.
    /// </summary>
    /// <param name="index">
    ///     The index of the object as returned from the last call to <see cref="GetSelectableControls"/>
    /// </param>
    void SelectControl(int index);
}