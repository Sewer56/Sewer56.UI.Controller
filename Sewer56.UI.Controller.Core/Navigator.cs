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
        if (state.PressedButtons.HasMovementButtons() && !state.IsButtonHeld(Button.Modifier))
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
        SelectableElement.Init(pos, direction, controlPositions, elements);

        // Sort by angle, select within X degrees and sort by distance.
        elements.Sort(new SelectableElement.CompareAngleAscending());
        elements = SelectableElement.SliceByMaxAngleDifference(elements);
        elements.Sort(new SelectableElement.CompareDistanceAscending());

        _platform.SelectControl(elements[0].Index);
    }

    /// <summary>
    /// Updates and polls inputs, returns true if there was an update.
    /// </summary>
    private bool PollInputs()
    {
        var inputs    = _controller.GetInputs();
        var stateCopy = _state;
        _state.Update(inputs);
        return !_state.Equals(stateCopy);
    }
}

internal struct SelectableElement
{
    public int Index;
    public Vector2 Position;
    public float Angle;
    public float DistanceSquared;

    /// <summary>
    /// Initializes a set of selectable elements given a list of positions.
    /// </summary>
    /// <param name="currentPosition">Current cursor position.</param>
    /// <param name="selectionDirection">Direction of user selection.</param>
    /// <param name="positions">The positions of the other elements.</param>
    /// <param name="result">Span that will hold result value.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static void Init(Vector2 currentPosition, Vector2 selectionDirection, Span<Vector2> positions, Span<SelectableElement> result)
    {
        for (int x = 0; x < positions.Length; x++)
        {
            ref SelectableElement res = ref result[x];
            res.Index = x;
            res.Position = positions[x];

            var vecCurrentToTarget = Mathematics.CalculateVector(res.Position, currentPosition);
            res.Angle = selectionDirection.CalcAngle(vecCurrentToTarget);
            res.DistanceSquared = vecCurrentToTarget.LengthSquared();
        }
    }

    /// <summary>
    /// Returns all selectable elements within the angle range `elements[0].Angle - elements[0].Angle + maxAngleDiff`.
    /// </summary>
    /// <param name="elements">All selectable elements, sorted by angle ascending.</param>
    /// <param name="maxAngleDiff">Maximum angle difference from lowest in degrees.</param>
    public static Span<SelectableElement> SliceByMaxAngleDifference(Span<SelectableElement> elements, float maxAngleDiff = 30.0f)
    {
        if (elements.Length == 1)
            return elements;

        var maxAngle    = elements[0].Angle + maxAngleDiff;
        int numElements = 1;

        while (numElements < elements.Length)
        {
            if (elements[numElements].Angle > maxAngle)
                break;

            numElements++;
        }

        return elements[..numElements];
    }

    /// <summary>
    /// Sorts a number of selectable elements by distance squared ascending
    /// </summary>
    public struct CompareDistanceAscending : IComparer<SelectableElement>
    {
        public int Compare(SelectableElement x, SelectableElement y)
        {
            if (x.DistanceSquared > y.DistanceSquared) return 1;
            if (x.DistanceSquared < y.DistanceSquared) return -1;
            return 0;
        }
    }

    /// <summary>
    /// Sorts a number of selectable elements by distance squared ascending
    /// </summary>
    public struct CompareAngleAscending : IComparer<SelectableElement>
    {
        public int Compare(SelectableElement x, SelectableElement y)
        {
            if (x.Angle > y.Angle) return 1;
            if (x.Angle < y.Angle) return -1;
            return 0;
        }
    }
}