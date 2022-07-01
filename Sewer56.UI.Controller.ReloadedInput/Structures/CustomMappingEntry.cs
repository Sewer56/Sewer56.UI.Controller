using Reloaded.Input.Structs;

namespace Sewer56.UI.Controller.ReloadedInput.Structures;

/// <summary>
/// Describes a custom mapping entry.  
/// That is an entry with custom post-process behaviour.  
/// </summary>
public struct CustomMappingEntry
{
    /// <summary>
    /// The name of the custom mapping.
    /// (Intended for people looking at library code, and UIs)
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The mapping index of the entry.
    /// </summary>
    public int MappingIndex { get; }

    /// <summary>
    /// The type of mapping used.
    /// </summary>
    public MappingType Type { get; }

    /// <summary>
    /// Description of the mapping.
    /// </summary>
    public string Description { get; }

    /// <summary/>
    /// <param name="name">The name of the custom mapping. (Intended for people looking at library code, and UIs)</param>
    /// <param name="mappingIndex">The mapping index of the entry.</param>
    /// <param name="type">The type of mapping used.</param>
    /// <param name="description">Description of the mapping.</param>
    public CustomMappingEntry(string name, int mappingIndex, MappingType type, string description = "")
    {
        Name = name;
        MappingIndex = mappingIndex;
        Type = type;
        Description = description;
    }
}