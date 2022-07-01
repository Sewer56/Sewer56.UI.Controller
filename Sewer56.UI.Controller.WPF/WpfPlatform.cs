using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Sewer56.UI.Controller.Core.Interfaces;
using Sewer56.UI.Controller.Core.Structures;
using static Sewer56.UI.Controller.Core.Enums.Button;
using static Sewer56.UI.Controller.WPF.WpfControls;
using static Sewer56.UI.Controller.WPF.WpfUtilities;

namespace Sewer56.UI.Controller.WPF;

/// <summary>
/// Allows for basic support of WPF/Windows Presentation Foundation projects.
/// </summary>
public class WpfPlatform : IPlatform, IDisposable
{
    /// <summary>
    /// Contains all of the children.
    /// </summary>
    private UIElement[] _childrenBuffer = new UIElement[IPlatform.MaxControls];

    /// <summary>
    /// Subscribe to this event from your application for custom input processing.
    /// </summary>
    public ProcessCustomInputsDelegate ProcessCustomInputs = (in ControllerState state) => { };

    /// <inheritdoc />
    public event Action OnUpdate = () => {};

    /// <inheritdoc />
    public bool FlipX { get; } = false;

    /// <inheritdoc />
    public bool FlipY { get; } = true;

    /// <summary>
    /// Adds support to a WPF application.
    /// </summary>
    public WpfPlatform() { CompositionTarget.Rendering += OnRendering; }

    /// <summary/>
    ~WpfPlatform() => Dispose();

    /// <inheritdoc />
    public void Dispose()
    {
        CompositionTarget.Rendering -= OnRendering;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public void ProcessInputs(in ControllerState state)
    {
        // Try get element and window.
        if (!TryGetFocusedElementAndWindow(out var window, out var currentControl))
        {
            ProcessCustomInputs(state);
            return;
        }

        // Close window if non-main and decline
        if (state.IsButtonPressed(Decline))
        {
            if (window != null && window != Application.Current.MainWindow)
                window.Close();
        }

        // Handle per-control actions
        switch (currentControl)
        {
            case Button btn:
                HandleButton(state, btn);
                break;
            case ComboBox combo:
                HandleComboBox(state, combo);
                break;

            case CheckBox check:
                HandleCheckbox(state, check);
                break;

            case ToggleButton toggle:
                HandleToggleButton(state, toggle);
                break;

            case DataGridCell cell:
                var row = FindParent<DataGridRow>(cell);
                if (row == null)
                    break;
                
                var grid = FindParent<DataGrid>(row);
                if (grid == null)
                    break;

                grid.SelectedIndex = row.GetIndex();
                break;

            case ListViewItem listViewItem:

                var listView = FindParent<ListView>(listViewItem);
                if (listView == null)
                    break;

                listView.SelectedIndex = listView.ItemContainerGenerator.IndexFromContainer(listViewItem);
                break;
        }

        // Call custom
        ProcessCustomInputs(state);
    }

    /// <inheritdoc />
    public Span<Vector2> GetSelectableControls(Span<Vector2> buffer)
    {
        if (!TryGetFocusedElementAndWindow(out var window, out var element))
            return buffer[..];

        var spanList = new SpanList<UIElement>(_childrenBuffer);
        FindSelectableChildren(window!, element, ref spanList);

        // Extract positions.
        ref var items = ref spanList.Items;
        int itemsInBuffer = 0;
        for (int x = 0; x < spanList.Length; x++)
        {
            buffer[itemsInBuffer++] = items[x].TransformToAncestor(window!)
                .Transform(new Point(0, 0))
                .AsVector() + (element.RenderSize.AsVector() / 2); // Take element center
        }

        return buffer[..itemsInBuffer];
    }

    /// <inheritdoc />
    public Vector2 GetCurrentPosition()
    {
        if (!TryGetFocusedElementAndWindow(out var window, out var element))
            return new Vector2(0,0);

        return element!.TransformToAncestor(window!)
            .Transform(new Point(0, 0))
            .AsVector() + (element.RenderSize.AsVector() / 2); // Take element center
    }

    /// <inheritdoc />
    public bool HasFocus()
    {
        if (TryGetFocusedElementAndWindow(out var window, out var element))
            return window!.IsActive;

        return false;
    }

    /// <inheritdoc />
    public void SelectControl(int index)
    {
        KeyboardNav.Focus(_childrenBuffer[index]);
    }

    private void OnRendering(object? sender, EventArgs e) => OnUpdate.Invoke();

    /// <summary>
    /// Delegate which 
    /// </summary>
    /// <param name="state">The current state of the controller.</param>
    public delegate void ProcessCustomInputsDelegate(in ControllerState state);
}