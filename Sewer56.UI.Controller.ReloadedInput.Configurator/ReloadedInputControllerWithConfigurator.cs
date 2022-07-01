using System;
using Reloaded.Input.Configurator.WPF;
using Reloaded.Input.Configurator;
using Reloaded.Input;
using Sewer56.UI.Controller.Core.Enums;
using System.Collections.ObjectModel;
using Reloaded.Input.Structs;
using Sewer56.UI.Controller.ReloadedInput.Enums;

namespace Sewer56.UI.Controller.ReloadedInput.Configurator;

/// <inheritdoc />
public class ReloadedInputControllerWithConfigurator : ReloadedInputController
{
    /// <inheritdoc />
    public ReloadedInputControllerWithConfigurator(string configPath) : base(configPath) { }

    /// <summary>
    /// Launches the Reloaded.Input configuration window.
    /// </summary>
    public void Configure(bool alreadyInWpfApp = false)
    {
        var mappings = new ObservableCollection<MappingEntry>();

        // Map enum to buttons. 
        for (int x = 0; x < ButtonNames.Length; x++)
        {
            if (ButtonValues[x] <= 0)
                continue;

            mappings.Add(new MappingEntry(ButtonNames[x], x, MappingType.Button, ButtonValues[x].GetDescription()));
        }

        // Add stick navigation
        foreach (var customEntry in CustomStickMappingEntryExtensions.StickCustomMapEntries)
        {
            var entry = customEntry.Entry;
            mappings.Add(new MappingEntry(entry.Name, entry.MappingIndex, entry.Type, entry.Description));
        }

        // Map enum to triggers. 
        for (int x = 0; x < ButtonNames.Length; x++)
        {
            if (ButtonValues[x] <= 0)
                continue;

            var mappingIndex = x + IndexTriggerButtons;
            mappings.Add(new MappingEntry($"{ButtonNames[x]} [Trigger]", mappingIndex, MappingType.Axis, $"[Use This if Mapping to Controller Trigger] {ButtonValues[x].GetDescription()}"));
        }

        var window = new ConfiguratorWindow(new[]
        {
            new ConfiguratorInput("Main Config", Controller.FilePath, mappings)
        });

        if (!alreadyInWpfApp)
        {
            var configurator = new Reloaded.Input.Configurator.WPF.Configurator();
            configurator.Run(window);
            RecreateController();
        }
        else
        {
            window.Show();
            window.Closed += (sender, args) => { RecreateController(); };
        }
    }
    
    private void RecreateController()
    {
        Controller?.Dispose();
        Controller = new VirtualController(Controller!.FilePath);
    }
}