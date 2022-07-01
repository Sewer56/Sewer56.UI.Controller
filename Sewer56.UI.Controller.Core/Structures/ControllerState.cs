﻿using System;
using System.Diagnostics;
using System.Numerics;
using Sewer56.UI.Controller.Core.Enums;

namespace Sewer56.UI.Controller.Core.Structures;

/// <summary>
/// The current state of the controller.
/// </summary>
public struct ControllerState
{
    /// <summary>
    /// Time spent holding a button until scrolling occurs.
    /// </summary>
    public const float TimeUntilScrollMs = 1000 / 4f;

    /// <summary>
    /// Time interval between successive menu scrolls.
    /// </summary>
    public const float ScrollSpeedMs = 1000 / 8f;

    /// <summary>
    /// The buttons currently held. 
    /// </summary>
    public Button HeldButtons { get; private set; }

    /// <summary>
    /// The buttons pressed. 
    /// </summary>
    public Button PressedButtons { get; private set; }

    /// <summary>
    /// The buttons released. 
    /// </summary>
    public Button ReleasedButtons { get; private set; }

    /// <summary>
    /// The state of the buttons used for navigation.
    /// </summary>
    public Button NavigationButtons { get; private set; }

    /// <summary>
    /// Time when buttons were last pressed or released.
    /// </summary>
    public float TimeSincePressOrRelease { get; private set; }
    private float _scrollingAccumulator;

    /// <summary>
    /// Updates the current state of the struct.
    /// </summary>
    /// <param name="inputs">The inputs.</param>
    /// <param name="deltaTime">Time since last function call in milliseconds.</param>
    public void Update(in FrameInputs inputs, float deltaTime)
    {
        PressedButtons = GetPressedButtons(HeldButtons, inputs.Buttons);
        ReleasedButtons = GetReleasedButtons(HeldButtons, inputs.Buttons);
        NavigationButtons = PressedButtons;
        HeldButtons = inputs.Buttons;

        // Scrolling
        if ((PressedButtons | ReleasedButtons) != Button.None)
        {
            TimeSincePressOrRelease = 0;
            _scrollingAccumulator = 0;
        }
        else
        {
            if (TimeSincePressOrRelease >= 0)
            {
                // Wait for scroll mode.
                TimeSincePressOrRelease += deltaTime;
                if (TimeSincePressOrRelease <= TimeUntilScrollMs) 
                    return;

                _scrollingAccumulator = TimeSincePressOrRelease - TimeUntilScrollMs;
                TimeSincePressOrRelease = float.MinValue;
            }
            else
            {
                // Scroll mode.
                _scrollingAccumulator += deltaTime;
                while (_scrollingAccumulator > ScrollSpeedMs)
                {
                    NavigationButtons |= HeldButtons;
                    _scrollingAccumulator -= ScrollSpeedMs;
                }
            }
        }
    }

    /// <summary>
    /// Gets the direction vector associated with the movement buttons pressed.
    /// </summary>
    /// <param name="flipX">Flips the direction in the X axis.</param>
    /// <param name="flipY">Flips the direction in the Y axis.</param>
    public Vector2 GetDirectionVector(bool flipX, bool flipY) => NavigationButtons.GetDirectionVector(flipX, flipY);

    /// <summary>
    /// Checks if the given button has been pressed.
    /// </summary>
    /// <param name="btn">The button to check.</param>
    public bool IsButtonPressed(Button btn) => PressedButtons.HasAnyFlag(btn);

    /// <summary>
    /// Checks if the given navigation button has been pressed.
    /// </summary>
    /// <param name="btn">The button to check.</param>
    public bool IsNavigationButtonPressed(Button btn) => NavigationButtons.HasAnyFlag(btn);

    /// <summary>
    /// Checks if the given button is held.
    /// </summary>
    /// <param name="btn">The button to check.</param>
    public bool IsButtonHeld(Button btn) => HeldButtons.HasAnyFlag(btn);

    /// <summary>
    /// Checks if the given button has been released.
    /// </summary>
    /// <param name="btn">The button to release.</param>
    public bool IsButtonReleased(Button btn) => ReleasedButtons.HasAnyFlag(btn);

    // Return B and NOT A "Return those before without the ones after"
    private Button GetReleasedButtons(Button before, Button after) => before & (~after);

    // Return A and NOT B "Return those after without the ones before"
    private Button GetPressedButtons(Button before, Button after) => after & (~before);

    #region Autogenerated by R#
    /// <summary>
    /// Verifies for equality against the other struct instance.
    /// </summary>
    public bool Equals(in ControllerState other)
    {
        return HeldButtons == other.HeldButtons && PressedButtons == other.PressedButtons &&
               ReleasedButtons == other.ReleasedButtons && NavigationButtons == other.NavigationButtons;
    }

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is ControllerState other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine((int)HeldButtons, (int)PressedButtons, (int)ReleasedButtons, NavigationButtons);
    #endregion
}