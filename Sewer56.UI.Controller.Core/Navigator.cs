using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sewer56.UI.Controller.Core.Enums;
using Sewer56.UI.Controller.Core.Interfaces;
using Sewer56.UI.Controller.Core.Structures;

namespace Sewer56.UI.Controller.Core;

/// <summary>
/// Main entry point for the controller library.
/// </summary>
public class Navigator
{
    private readonly IPlatform _platform;
    private readonly IController _controller;
    private ControllerState _state;
    private DateTime _lastDateTime = DateTime.UtcNow;

    /// <summary>
    /// Specifies the amount of bias towards picking elements based on distance.
    /// Higher value means selected elements will more likely be based on distance, lower value means less likely.
    /// Note: Library already has implicit large distance bias as it internally uses distance squared, you can minimize it using this.
    /// </summary>
    public float DistanceBias = 1f;

    /// <summary>
    /// Max selection angle in degrees between current control and new control
    /// before a control is disqualified as not selectable.
    /// </summary>
    public float MaxSelectionAngle = 89f;

    /// <summary>
    /// Creates an instance of the navigator.
    /// </summary>
    /// <param name="platform">Abstracts the platform, e.g. WPF.</param>
    /// <param name="controller">Abstracts the controller input, e.g. Reloaded.Input</param>
    public Navigator(IPlatform platform, IController controller)
    {
        _platform = platform;
        _controller = controller;
        platform.OnUpdate += OnUpdate;
    }

    [SkipLocalsInit]
    private void OnUpdate()
    {
        if (!PollInputs())
            return;

        ref var state = ref _state;
        if (state.NavigationButtons.HasMovementButtons() && !state.IsButtonHeld(Button.Modifier))
            OnUpdate_Movement(ref state);

        _platform.ProcessInputs(state);
    }

    private void OnUpdate_Movement(ref ControllerState state)
    {
        if (!_platform.HasFocus())
            return;

        // Get current and possible positions.
        var pos = _platform.GetCurrentPosition();
        Span<Vector2> controlPositions = stackalloc Vector2[IPlatform.MaxControls];
        controlPositions = _platform.GetSelectableControls(controlPositions);

        if (controlPositions.Length == 0)
            return;

        // Get button info.
        var direction = state.GetDirectionVector(_platform.FlipX, _platform.FlipY);
        Span<SelectableElement> elements = stackalloc SelectableElement[controlPositions.Length];
        SelectableElement.Init(pos, direction, controlPositions, elements, DistanceBias);

        // Select element with highest score.
        var lowest = SelectableElement.GetLowestScore(elements, MaxSelectionAngle);
        if (lowest.HasValue)
            _platform.SelectControl(lowest.Value.Index);
    }

    /// <summary>
    /// Updates and polls inputs, returns true if there was an update.
    /// </summary>
    private bool PollInputs()
    {
        // Normally we'd set lastDateTime here if never set but it's okay not to
        // due to implementation details of State Update.
        var currentTime = DateTime.UtcNow;
        var deltaTime = currentTime - _lastDateTime;
        _lastDateTime = currentTime;
        var inputs    = _controller.GetInputs();
        var stateCopy = _state;
        _state.Update(inputs, (float)deltaTime.TotalMilliseconds);
        return !_state.Equals(stateCopy);
    }
}

internal struct SelectableElement
{
    public int Index;
    public float Angle;
    public float Score;

    /// <summary>
    /// Initializes a set of selectable elements given a list of positions.
    /// </summary>
    /// <param name="currentPosition">Current cursor position.</param>
    /// <param name="selectionDirection">Direction of user selection.</param>
    /// <param name="positions">The positions of the other elements.</param>
    /// <param name="result">Span that will hold result value.</param>
    /// <param name="distanceBias">Bias to apply for distance. Default 1.0f. Higher values means controls more likely picked based on distance.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static void Init(Vector2 currentPosition, Vector2 selectionDirection, Span<Vector2> positions,
        Span<SelectableElement> result, float distanceBias = 1.0f)
    {
        for (int x = 0; x < positions.Length; x++)
        {
            ref SelectableElement res = ref result[x];
            res.Index = x;

            var vecCurrentToTarget = Mathematics.CalculateVector(positions[x], currentPosition);
            var angle    = selectionDirection.CalcAngle(vecCurrentToTarget);
            var distance = vecCurrentToTarget.LengthSquared();
            
            // Just in case.
            res.Angle = angle;
            angle += 0.000001f; // This might skew the results a bit, but solves problem with multiplying values too small.
            distance *= distanceBias;
            if (distance == 0)
                distance = float.MinValue;

            res.Score = angle * distance;
        }
    }

    /// <summary>
    /// Returns the element with the lowest score.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static SelectableElement? GetLowestScore(Span<SelectableElement> elements, float maxAngle = 89f)
    {
        var result = new SelectableElement()
        {
            Score = float.MaxValue
        };

        for (int x = 0; x < elements.Length; x++)
        {
            var element = elements[x];
            if (element.Score < result.Score && element.Angle < maxAngle)
                result = element;
        }

        if (result.Score != float.MaxValue)
            return result;

        return null;
    }
}