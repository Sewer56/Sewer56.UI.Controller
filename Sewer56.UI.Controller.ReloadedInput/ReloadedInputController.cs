using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Reloaded.Input;
using Reloaded.Input.Configurator;
using Reloaded.Input.Configurator.WPF;
using Reloaded.Input.Structs;
using Sewer56.UI.Controller.Core.Enums;
using Sewer56.UI.Controller.Core.Interfaces;
using Sewer56.UI.Controller.Core.Structures;

namespace Sewer56.UI.Controller.ReloadedInput;

/// <summary>
/// A controller that abstracts the Reloaded.Hooks library.
/// </summary>
public class ReloadedInputController : IController, IDisposable
{
    private static readonly Button[] _buttonValues = ButtonExtensions.GetValues();
    private static readonly int _triggerMinMappingIndex = 256;
    private static readonly int _customMappingIndex_Stick = 512;

    private static readonly StickMappingEntry[] _customMapEntries_Stick = new StickMappingEntry[]
    {
        new StickMappingEntry()
        {
            Entry = new MappingEntry("Menu Left/Right [Stick]", _customMappingIndex_Stick + 0, MappingType.Axis, "Navigates the Menu Left/Right"),
            ValueOnNegative = Button.Left,
            ValueOnPositive = Button.Right
        },
        new StickMappingEntry()
        {
            Entry = new MappingEntry("Menu Up/Down [Stick]", _customMappingIndex_Stick + 1, MappingType.Axis, "Navigates the Menu Up/Down"),
            ValueOnNegative = Button.Down,
            ValueOnPositive = Button.Up
        }
    };

    private VirtualController _controller;

    /// <summary>
    /// Creates a Reloaded.Hooks controller from a given instance.
    /// </summary>
    /// <param name="configPath"></param>
    public ReloadedInputController(string configPath) => _controller = new VirtualController(configPath);

    /// <summary/>
    ~ReloadedInputController() => Dispose();

    /// <inheritdoc />
    public void Dispose()
    {
        _controller?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Launches the Reloaded.Input configuration window.
    /// </summary>
    public void Configure(bool alreadyInWpfApp = false)
    {
        var buttonNames  = ButtonExtensions.GetNames();
        var mappings     = new ObservableCollection<MappingEntry>();

        // Map enum to buttons. 
        for (int x = 0; x < buttonNames.Length; x++)
        {
            if (_buttonValues[x] <= 0)
                continue;

            mappings.Add(new MappingEntry(buttonNames[x], x, MappingType.Button, _buttonValues[x].GetDescription()));
        }

        // Add stick navigation
        foreach (var customEntry in _customMapEntries_Stick)
            mappings.Add(customEntry.Entry);

        // Map enum to triggers. 
        for (int x = 0; x < buttonNames.Length; x++)
        {
            if (_buttonValues[x] <= 0)
                continue;

            var mappingIndex = x + _triggerMinMappingIndex;
            mappings.Add(new MappingEntry($"{buttonNames[x]} [Trigger]", mappingIndex, MappingType.Axis, $"[Use This if Mapping to Controller Trigger] {_buttonValues[x].GetDescription()}"));
        }

        var window = new ConfiguratorWindow(new[]
        {
            new ConfiguratorInput("Main Config", _controller.FilePath, mappings)
        });

        if (!alreadyInWpfApp)
        {
            var configurator = new Configurator();
            configurator.Run(window);
        }
        else
            window.Show();

        _controller?.Dispose();
        _controller = new VirtualController(_controller.FilePath);
    }

    /// <inheritdoc />
    public FrameInputs GetInputs()
    {
        _controller.PollAll();
        var button = Button.None;

        // Map returned buttons to the enum
        for (int x = 0; x < _buttonValues.Length; x++)
        {
            bool isPressed = _controller.GetButton(x);
            button |= (Button)((int)_buttonValues[x] * Unsafe.As<bool, byte>(ref isPressed));
        }

        // Map triggers to the enum.
        for (int x = 0; x < _buttonValues.Length; x++)
        {
            float axisValue = _controller.GetAxis(_triggerMinMappingIndex + x);
            bool isPressed  = axisValue > 0.0f;
            button |= (Button)((int)_buttonValues[x] * Unsafe.As<bool, byte>(ref isPressed));
        }

        // Map stick movement
        foreach (var customEntry in _customMapEntries_Stick)
        {
            float axisValue = _controller.GetAxis(customEntry.Entry.MappingIndex);
            button |= customEntry.GetValue(axisValue);
        }

        return new FrameInputs(button);
    }
}

internal struct StickMappingEntry
{
    public const float MinAxis = AxisSet.MaxValue * 0.75f;

    public MappingEntry Entry;
    public Button ValueOnNegative;
    public Button ValueOnPositive;

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