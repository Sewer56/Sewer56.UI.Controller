using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Reloaded.Input;
using Reloaded.Input.Structs;
using Sewer56.UI.Controller.Core.Enums;
using Sewer56.UI.Controller.Core.Interfaces;
using Sewer56.UI.Controller.Core.Structures;
using Sewer56.UI.Controller.ReloadedInput.Enums;

namespace Sewer56.UI.Controller.ReloadedInput;

/// <summary>
/// A controller that abstracts the Reloaded.Input library.
/// </summary>
public class ReloadedInputController : IController, IDisposable
{
    /// <summary>
    /// All possible button values.
    /// </summary>
    public static readonly Button[] ButtonValues = ButtonExtensions.GetValues();

    /// <summary>
    /// All possible button names.
    /// </summary>
    public static readonly string[] ButtonNames = ButtonExtensions.GetNames();

    /// <summary>
    /// The mapping index where the entries for mapping buttons to buttons start.
    /// </summary>
    public const int IndexButtonButtons = 0;

    /// <summary>
    /// The mapping index where the entries for mapping triggers to buttons (<see cref="Button"/>) start.
    /// </summary>
    public const int IndexTriggerButtons = 256;

    /// <summary>
    /// The mapping index where the custom stick mappings (<see cref="Enums.CustomStickMappingEntry"/>) start.
    /// </summary>
    public const int IndexCustomStickMapping = 512;

    /// <summary>
    /// The underlying controller instance.
    /// </summary>
    public VirtualController Controller;

    /// <summary>
    /// Creates a Reloaded.Hooks controller from a given instance.
    /// </summary>
    /// <param name="configPath"></param>
    public ReloadedInputController(string configPath) => Controller = new VirtualController(configPath);

    /// <summary/>
    ~ReloadedInputController() => Dispose();

    /// <inheritdoc />
    public void Dispose()
    {
        Controller?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Starts an operation to map a specified button using the Reloaded.Input library.
    /// </summary>
    /// <param name="mappingNo">The unique number. e.g. 0 for button 1, 1 for button 2 corresponding to same action.</param>
    /// <param name="token">Allows for cancelling the mapping process.</param>
    /// <param name="callback">Executed after every poll attempt for a key or axis.</param>
    /// <param name="button">The button to map in the underlying library.</param>
    public Task<bool> MapButton(Button button, int mappingNo, CancellationToken token = default, Action? callback = null)
    {
        return Controller.Map(IndexButtonButtons + (int)button, MappingType.Button, mappingNo, token, callback);
    }

    /// <summary>
    /// Unmaps a button mapped in the underlying library.
    /// </summary>
    /// <param name="button">The button to unmap.</param>
    /// <param name="mappingNo">The mapping number of the button.</param>
    public void UnMapButton(Button button, int mappingNo) => Controller.UnMap(IndexButtonButtons + (int)button, mappingNo);

    /// <summary>
    /// Starts an operation to map a specified trigger to a button using the Reloaded.Input library.
    /// </summary>
    /// <param name="mappingNo">The unique number. e.g. 0 for button 1, 1 for button 2 corresponding to same action.</param>
    /// <param name="token">Allows for cancelling the mapping process.</param>
    /// <param name="callback">Executed after every poll attempt for a key or axis.</param>
    /// <param name="button">The button to map in the underlying library.</param>
    public Task<bool> MapTriggerToButton(Button button, int mappingNo, CancellationToken token = default, Action? callback = null)
    {
        return Controller.Map(IndexTriggerButtons + (int)button, MappingType.Axis, mappingNo, token, callback);
    }

    /// <summary>
    /// Unmaps a button mapped in the underlying library.
    /// </summary>
    /// <param name="button">The button to unmap.</param>
    /// <param name="mappingNo">The mapping number of the button.</param>
    public void UnMapTriggerToButton(Button button, int mappingNo) => Controller.UnMap(IndexTriggerButtons + (int)button, mappingNo);

    /// <summary>
    /// Starts an operation to map a specified trigger to a button using the Reloaded.Input library.
    /// </summary>
    /// <param name="mappingNo">The unique number. e.g. 0 for button 1, 1 for button 2 corresponding to same action.</param>
    /// <param name="token">Allows for cancelling the mapping process.</param>
    /// <param name="callback">Executed after every poll attempt for a key or axis.</param>
    /// <param name="stickEntry">The custom stick entry to map in the underlying library.</param>
    public Task<bool> MapCustomStickBehaviour(CustomStickMappingEntry stickEntry, int mappingNo, CancellationToken token = default, Action? callback = null)
    {
        return Controller.Map(IndexCustomStickMapping + (int)stickEntry, MappingType.Axis, mappingNo, token, callback);
    }

    /// <summary>
    /// Unmaps a button mapped in the underlying library.
    /// </summary>
    /// <param name="stickEntry">The behaviour to unmap.</param>
    /// <param name="mappingNo">The mapping number of the button.</param>
    public void UnMapCustomStickBehaviour(CustomStickMappingEntry stickEntry, int mappingNo) => Controller.UnMap(IndexCustomStickMapping + (int)stickEntry, mappingNo);

    /// <inheritdoc />
    public virtual FrameInputs GetInputs()
    {
        Controller.PollAll();
        var button = Button.None;

        // Map returned buttons to the enum
        for (int x = 0; x < ButtonValues.Length; x++)
        {
            bool isPressed = Controller.GetButton(x);
            button |= (Button)((int)ButtonValues[x] * Unsafe.As<bool, byte>(ref isPressed));
        }

        // Map triggers to the enum.
        for (int x = 0; x < ButtonValues.Length; x++)
        {
            float axisValue = Controller.GetAxis(IndexTriggerButtons + x);
            bool isPressed  = axisValue > 0.0f;
            button |= (Button)((int)ButtonValues[x] * Unsafe.As<bool, byte>(ref isPressed));
        }

        // Map stick movement
        foreach (var customEntry in CustomStickMappingEntryExtensions.StickCustomMapEntries)
        {
            float axisValue = Controller.GetAxis(customEntry.Entry.MappingIndex);
            button |= customEntry.GetValue(axisValue);
        }

        return new FrameInputs(button);
    }
}