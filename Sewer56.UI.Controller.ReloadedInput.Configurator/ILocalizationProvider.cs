using Sewer56.UI.Controller.Core.Enums;
using Sewer56.UI.Controller.ReloadedInput.Enums;

namespace Sewer56.UI.Controller.ReloadedInput.Configurator;

/// <summary>
/// This class provides localization support for a given. 
/// </summary>
public interface ILocalizationProvider
{
    /// <summary>
    /// Tries to get the localized name for the given button.
    /// </summary>
    /// <param name="button">The button to get name for.</param>
    /// <returns>The name, or null if not available.</returns>
    public string? GetName(Button button);

    /// <summary>
    /// Tries to get the description for the given button.
    /// </summary>
    /// <param name="button">The button to get description for.</param>
    /// <returns>The description, or null if not available.</returns>
    public string? GetDescription(Button button);

    /// <summary>
    /// Tries to get the localized name for the given stick mapping.
    /// </summary>
    /// <param name="entry">The mapping to get name for.</param>
    /// <returns>The name, or null if not available.</returns>
    public string? GetName(CustomStickMappingEntry entry);

    /// <summary>
    /// Tries to get the description for the given stick mapping.
    /// </summary>
    /// <param name="entry">The mapping to get name for.</param>
    /// <returns>The description, or null if not available.</returns>
    public string? GetDescription(CustomStickMappingEntry entry);

    /// <summary>
    /// Tries to get the localized name for the given string.
    /// </summary>
    /// <param name="customString">The string to get name for.</param>
    /// <returns>The string, or null if not available.</returns>
    public string? GetCustomString(CustomStrings customString);
}

/// <summary>
/// Custom strings specific to this package.
/// </summary>
public enum CustomStrings
{
    /// <summary>
    /// "Main Config"
    /// </summary>
    MainConfig,

    /// <summary>
    /// "Trigger", as in, a trigger on a controller (L2/R2, LT/RT) etc.
    /// </summary>
    Trigger,

    /// <summary>
    /// "Use This if Mapping to Controller Trigger"
    /// </summary>
    TriggerDescription,

    /// <summary>
    /// "Reloaded.Input Configurator" Name of the configurator window.
    /// </summary>
    WindowName,
}