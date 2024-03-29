﻿using Sewer56.UI.Controller.Core.Enums;
using Sewer56.UI.Controller.ReloadedInput.Enums;

namespace Sewer56.UI.Controller.ReloadedInput.Configurator;

/// <summary>
/// The default implementation for providing localization.
/// </summary>
internal class DefaultLocalizationProvider
{
    internal readonly ILocalizationProvider? InternalProvider;

    public DefaultLocalizationProvider(ILocalizationProvider? internalProvider)
    {
        InternalProvider = internalProvider;
    }

    public string GetName(Button button)
    {
        var text = InternalProvider?.GetName(button);
        return text ?? ReloadedInputController.ButtonNames[(int)button];
    }

    public string GetDescription(Button button)
    {
        var text = InternalProvider?.GetDescription(button);
        return text ?? ReloadedInputController.ButtonValues[(int)button].GetDescription();
    }

    public string GetName(CustomStickMappingEntry entry)
    {
        var text = InternalProvider?.GetName(entry);
        return text ?? CustomStickMappingEntryExtensions.StickCustomMapEntries[(int)entry].Entry.Name;
    }

    public string GetDescription(CustomStickMappingEntry entry)
    {
        var text = InternalProvider?.GetDescription(entry);
        return text ?? CustomStickMappingEntryExtensions.StickCustomMapEntries[(int)entry].Entry.Description;
    }

    public string GetCustomString(CustomStrings customString)
    {
        var text = InternalProvider?.GetCustomString(customString);
        if (text != null)
            return text;

        return customString switch
        {
            CustomStrings.MainConfig => "Main Config",
            CustomStrings.Trigger => "Trigger",
            CustomStrings.TriggerDescription => "Use This if Mapping to Controller Trigger",
            CustomStrings.WindowName => "Reloaded.Input Configurator",
            _ => ""
        };
    }
}