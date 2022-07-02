using Reloaded.Input.Configurator.WPF;
using Reloaded.Input.Configurator;
using Reloaded.Input;
using Sewer56.UI.Controller.Core.Enums;
using System.Collections.ObjectModel;
using Reloaded.Input.Structs;
using Sewer56.UI.Controller.ReloadedInput.Enums;
using static Sewer56.UI.Controller.ReloadedInput.Configurator.CustomStrings;

namespace Sewer56.UI.Controller.ReloadedInput.Configurator;

/// <inheritdoc />
public class ReloadedInputControllerWithConfigurator : ReloadedInputController
{
    private DefaultLocalizationProvider? _provider;

    /// <inheritdoc />
    public ReloadedInputControllerWithConfigurator(string configPath) : base(configPath)
    {
        _provider = new DefaultLocalizationProvider(null);
    }

    /// <summary>
    /// Creates a Reloaded.Input with Configurator instance with localization support.
    /// </summary>
    /// <param name="configPath">Path to store the Reloaded.Input configuration.</param>
    /// <param name="provider">Provides support for localization.</param>
    public ReloadedInputControllerWithConfigurator(string configPath, ILocalizationProvider? provider) : base(configPath)
    {
        _provider = new DefaultLocalizationProvider(provider);
    }

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

            mappings.Add(new MappingEntry(_provider!.GetName(ButtonValues[x]), x, MappingType.Button, _provider.GetDescription(ButtonValues[x])));
        }

        // Add stick navigation
        for (var x = 0; x < CustomStickMappingEntryExtensions.StickCustomMapEntries.Length; x++)
        {
            var customEntry = CustomStickMappingEntryExtensions.StickCustomMapEntries[x];
            var entry = customEntry.Entry;
            mappings.Add(new MappingEntry(_provider!.GetName((CustomStickMappingEntry)x), entry.MappingIndex, entry.Type, _provider.GetDescription((CustomStickMappingEntry)x)));
        }

        // Map enum to triggers. 
        for (int x = 0; x < ButtonNames.Length; x++)
        {
            if (ButtonValues[x] <= 0)
                continue;

            var mappingIndex = x + IndexTriggerButtons;
            mappings.Add(new MappingEntry
            (
                $"{_provider!.GetName(ButtonValues[x])} [{_provider.GetCustomString(Trigger)}]", 
                mappingIndex, 
                MappingType.Axis, 
                $"[{_provider.GetCustomString(TriggerDescription)}] {_provider!.GetDescription(ButtonValues[x])}"
            ));
        }

        var window = new ConfiguratorWindow(new[]
        {
            new ConfiguratorInput(_provider!.GetCustomString(MainConfig), Controller.FilePath, mappings)
        }, _provider.InternalProvider);

        window.Title = _provider!.GetCustomString(WindowName);
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