using UnityEngine;

public enum NoteboardEntryCategory
{
    General,
    Generator,
    Field,
    Containment,
    Warning,
    Research
}

[CreateAssetMenu(menuName = "Data/Noteboard Entry")]
public class NoteboardEntryData : ScriptableObject
{
    public string entryId;
    public NoteboardEntryCategory category = NoteboardEntryCategory.General;

    [Header("Content")]
    public string title;

    [TextArea(3, 8)]
    public string body;

    [Header("Sorting")]
    public int priority;

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(entryId))
        {
            entryId = name.ToLowerInvariant().Replace(" ", "_");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            title = name;
        }
    }
}