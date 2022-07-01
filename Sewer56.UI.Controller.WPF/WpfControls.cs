using System.Runtime.CompilerServices;
using Sewer56.UI.Controller.Core.Structures;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using static Sewer56.UI.Controller.Core.Enums.Button;
using Button = System.Windows.Controls.Button;
using System;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Sewer56.UI.Controller.WPF;

/// <summary>
/// Code for handling WPF Control interactions.
/// </summary>
internal static class WpfControls
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void HandleButton(ControllerState state, Button btn)
    {
        var peer = new ButtonAutomationPeer(btn);
        if (!state.IsButtonPressed(Accept)) 
            return;

        var invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
        invokeProv!.Invoke();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void HandleComboBox(ControllerState state, ComboBox comboBox)
    {
        void DecrementValue() => comboBox.SelectedIndex = ModuloCycleItem(comboBox.SelectedIndex, -1, comboBox.Items.Count);
        void IncrementValue() => comboBox.SelectedIndex = ModuloCycleItem(comboBox.SelectedIndex, 1, comboBox.Items.Count);

        if (state.IsButtonPressed(Increment) || (state.IsButtonHeld(Modifier) && state.IsButtonPressed(Down | Right)))
            IncrementValue();

        if (state.IsButtonPressed(Decrement) || (state.IsButtonHeld(Modifier) && state.IsButtonPressed(Up | Left)))
            DecrementValue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void HandleCheckbox(in ControllerState state, CheckBox check)
    {
        void Tick() => check.IsChecked = true;
        void Untick() => check.IsChecked = false;
        void Toggle() => check.IsChecked = !check.IsChecked;

        if (state.IsButtonPressed(Increment))
            Tick();

        if (state.IsButtonPressed(Decrement))
            Untick();

        if (state.IsButtonPressed(Accept))
            Toggle();
    }

    internal static void HandleToggleButton(in ControllerState state, ToggleButton toggle)
    {
        void Enable() => toggle.IsChecked = true;
        void Disable() => toggle.IsChecked = false;
        void Toggle() => toggle.IsChecked = !toggle.IsChecked;

        if (state.IsButtonPressed(Increment))
            Enable();

        if (state.IsButtonPressed(Decrement))
            Disable();

        if (state.IsButtonPressed(Accept))
            Toggle();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int ModuloCycleItem(int value, int direction, int maxValue)
    {
        int result = (value + direction) % maxValue;
        if (result < 0)
            result = maxValue - (-direction);

        return result;
    }
}