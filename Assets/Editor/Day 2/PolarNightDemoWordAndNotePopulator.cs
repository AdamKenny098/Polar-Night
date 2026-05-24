using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PolarNightDemoWordAndNotePopulator
{
    private const string GeneratorFolder = "Assets/Generator";
    private const string WordDatabasePath = "Assets/Generator/GeneratorWordDatabase.asset";

    private const string NoteboardFolder = "Assets/Noteboard";
    private const string NoteboardEntriesFolder = "Assets/Noteboard/Entries";

    private struct NoteDefinition
    {
        public string AssetName;
        public string EntryId;
        public NoteboardEntryCategory Category;
        public string Title;
        public string Body;
        public int Priority;

        public NoteDefinition(
            string assetName,
            string entryId,
            NoteboardEntryCategory category,
            string title,
            string body,
            int priority
        )
        {
            AssetName = assetName;
            EntryId = entryId;
            Category = category;
            Title = title;
            Body = body;
            Priority = priority;
        }
    }

    [MenuItem("Tools/Polar Night/Populate Words And Notes")]
    public static void PopulateWordsAndNotes()
    {
        EnsureFolderExists("Assets", "Generator");
        EnsureFolderExists("Assets", "Noteboard");
        EnsureFolderExists(NoteboardFolder, "Entries");

        PopulateGeneratorWordDatabase();
        PopulateNoteboardEntries();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Polar Night words and noteboard entries populated.");
    }

    private static void PopulateGeneratorWordDatabase()
    {
        GeneratorWordDatabase database = AssetDatabase.LoadAssetAtPath<GeneratorWordDatabase>(WordDatabasePath);

        if (!database)
        {
            database = ScriptableObject.CreateInstance<GeneratorWordDatabase>();
            AssetDatabase.CreateAsset(database, WordDatabasePath);
        }

        database.wordSets.Clear();

        AddWordSet(database, GeneratorWordCategory.Mechanical, new List<string>
        {
            "FUSES", "VALVE", "MOTOR", "CABLE", "WIRES",
            "PANEL", "LEVER", "GEARS", "COILS", "SPARK",
            "CRANK", "BOLTS", "RIVET", "TIMER", "GAUGE",
            "PIPES", "VENTS", "ROTOR", "RELAY", "INPUT",
            "TANKS", "PLUGS", "DRIVE", "BRAKE", "HINGE",
            "SHAFT", "CLAMP", "FRAME", "GRATE", "LATCH",
            "METER", "FIBER", "CHAIN", "WHEEL", "SCREW",
            "DRILL", "TOOLS", "DUCTS", "JOINT", "SEALS"
        });

        AddWordSet(database, GeneratorWordCategory.ColdWeather, new List<string>
        {
            "FROST", "STORM", "CHILL", "SNOWS", "WINDS",
            "WHITE", "POLAR", "NORTH", "ARCTC", "GLACE",
            "FLOES", "DRIFT", "SLEET", "FROZE", "ICEST",
            "CRUST", "SHARD", "BRINE", "WASTE", "CAVES",
            "RIDGE", "CLIFF", "NIGHT", "DARKS", "CLOAK",
            "BITES", "NUMBS", "HAILS", "GUSTS", "COVER",
            "BURRY", "SHADE", "FRIGD", "BLAST", "TUNDR",
            "SNARE", "GLAZE", "MISTS", "CREEK", "BANKS"
        });

        AddWordSet(database, GeneratorWordCategory.Anomaly, new List<string>
        {
            "ECHO", "TRACE", "STATIC", "SIGNL", "HUMMS",
            "WOUND", "SIGIL", "RIFTS", "GHOST", "OTHER",
            "DREAM", "VOICE", "SHAPE", "THING", "FACET",
            "PULSE", "VEINS", "SPORE", "ABYSS", "FLESH",
            "WRAIT", "SHADE", "ORBIT", "CRYPT", "ALTAR",
            "HOLLOW", "GRASP", "STARE", "WATCH", "NOISE",
            "WHISP", "GLARE", "GLOOM", "VANTA", "CYCLE",
            "MARKS", "OMENS", "BLOOM", "SHIFT", "PHASE"
        });

        AddWordSet(database, GeneratorWordCategory.Emergency, new List<string>
        {
            "ALARM", "FAULT", "ERROR", "PANIC", "BREAK",
            "RELAY", "RESET", "POWER", "DRAIN", "SURGE",
            "FLOOD", "SMOKE", "FIREX", "DANGER", "ALERT",
            "SIREN", "LOCKS", "FAILS", "CRASH", "BURNS",
            "SHORT", "BLEED", "CLOGS", "LEAKS", "PRESS",
            "BLINK", "SHAKE", "CRACK", "BURST", "STUCK",
            "EMPTY", "LOWER", "FINAL", "SCARE", "FAULT",
            "WORST", "DELAY", "PATCH", "BREAK", "RETRY"
        });

        AddWordSet(database, GeneratorWordCategory.Containment, new List<string>
        {
            "CELLS", "CHAIN", "LOCKS", "DOORS", "GLASS",
            "VAULT", "SEALS", "FIELD", "ALPHA", "OMEGA",
            "BLOCK", "GUARD", "WATCH", "LEVEL", "ENTRY",
            "PANEL", "CAGES", "BOUND", "CAPTR", "SNARE",
            "TRAPS", "BAITS", "ALLOY", "LASER", "REACT",
            "STUDY", "TESTS", "NOTES", "SHIFT", "RISKS",
            "BREACH", "INNER", "OUTER", "CLEAN", "QUIET",
            "SCOPE", "TOWER", "MOTEL", "STARE", "HINGE"
        });

        EditorUtility.SetDirty(database);

        Debug.Log($"Generator word database populated at: {WordDatabasePath}");
    }

    private static void AddWordSet(
        GeneratorWordDatabase database,
        GeneratorWordCategory category,
        List<string> words
    )
    {
        GeneratorWordSet set = new GeneratorWordSet
        {
            category = category,
            words = new List<string>()
        };

        HashSet<string> seenWords = new HashSet<string>();

        for (int i = 0; i < words.Count; i++)
        {
            string word = NormalizeWord(words[i]);

            if (string.IsNullOrWhiteSpace(word))
            {
                continue;
            }

            if (word.Length != 5)
            {
                Debug.LogWarning($"Skipped generator word '{word}' because it is not 5 letters.");
                continue;
            }

            if (seenWords.Contains(word))
            {
                continue;
            }

            seenWords.Add(word);
            set.words.Add(word);
        }

        database.wordSets.Add(set);
    }

    private static void PopulateNoteboardEntries()
    {
        List<NoteDefinition> notes = new List<NoteDefinition>
        {
            new NoteDefinition(
                "Generator Log - Pressure Valve",
                "generator_pressure_valve",
                NoteboardEntryCategory.Generator,
                "Generator Log",
                "Pressure valve is unstable. If the generator stalls again, check the valve before checking the fuel line.",
                10
            ),

            new NoteDefinition(
                "Generator Log - Fuel Draw",
                "generator_fuel_draw",
                NoteboardEntryCategory.Generator,
                "Fuel Draw Warning",
                "Fuel consumption has increased during the evening cycle. Something may be pulling more power from the shelter grid.",
                20
            ),

            new NoteDefinition(
                "Field Note - Northern Tracks",
                "field_northern_tracks",
                NoteboardEntryCategory.Field,
                "Northern Tracks",
                "Tracks found north of the shelter. They do not match boot marks, animal prints, or anything from the supply manifest.",
                30
            ),

            new NoteDefinition(
                "Field Note - Broken Cache",
                "field_broken_cache",
                NoteboardEntryCategory.Field,
                "Broken Cache",
                "One of the supply caches was torn open from the inside. The metal bent outward.",
                40
            ),

            new NoteDefinition(
                "Containment Note - Empty Chamber",
                "containment_empty_chamber",
                NoteboardEntryCategory.Containment,
                "Containment Chamber",
                "The annex chamber is still empty. Keep it powered, sealed, and ready. Do not wait until something is already at the door.",
                50
            ),

            new NoteDefinition(
                "Containment Note - Power Requirement",
                "containment_power_requirement",
                NoteboardEntryCategory.Containment,
                "Power Requirement",
                "Containment systems cannot run from emergency lighting alone. Generator maintenance now affects more than heat.",
                60
            ),

            new NoteDefinition(
                "Warning - Do Not Follow Voices",
                "warning_do_not_follow_voices",
                NoteboardEntryCategory.Warning,
                "Do Not Follow Voices",
                "If you hear someone calling from outside, verify their location visually before opening the door. Do not answer unknown voices.",
                70
            ),

            new NoteDefinition(
                "Warning - Whiteout Movement",
                "warning_whiteout_movement",
                NoteboardEntryCategory.Warning,
                "Whiteout Movement",
                "Movement has been reported during heavy snow. Visibility loss may be hiding approach patterns.",
                80
            ),

            new NoteDefinition(
                "Research - Residue Sample",
                "research_residue_sample",
                NoteboardEntryCategory.Research,
                "Residue Sample",
                "The residue reacts faintly to battery current. It may be useful in bait, containment tools, or tracking devices.",
                90
            ),

            new NoteDefinition(
                "Research - Bait Hypothesis",
                "research_bait_hypothesis",
                NoteboardEntryCategory.Research,
                "Bait Hypothesis",
                "Organic material mixed with residue could act as a lure. The idea is ugly, but it may work.",
                100
            )
        };

        int created = 0;
        int updated = 0;

        for (int i = 0; i < notes.Count; i++)
        {
            NoteDefinition definition = notes[i];

            NoteboardEntryData entry = FindExistingNote(definition);

            if (!entry)
            {
                entry = ScriptableObject.CreateInstance<NoteboardEntryData>();

                string path = $"{NoteboardEntriesFolder}/{SanitizeFileName(definition.AssetName)}.asset";
                path = AssetDatabase.GenerateUniqueAssetPath(path);

                AssetDatabase.CreateAsset(entry, path);
                created++;
            }
            else
            {
                updated++;
            }

            entry.entryId = definition.EntryId;
            entry.category = definition.Category;
            entry.title = definition.Title;
            entry.body = definition.Body;
            entry.priority = definition.Priority;

            EditorUtility.SetDirty(entry);
        }

        Debug.Log($"Noteboard entries populated. Created: {created}. Updated: {updated}.");
    }

    private static NoteboardEntryData FindExistingNote(NoteDefinition definition)
    {
        string[] guids = AssetDatabase.FindAssets("t:NoteboardEntryData", new[] { NoteboardEntriesFolder });

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            NoteboardEntryData entry = AssetDatabase.LoadAssetAtPath<NoteboardEntryData>(path);

            if (!entry)
            {
                continue;
            }

            if (entry.entryId == definition.EntryId)
            {
                return entry;
            }

            if (entry.title == definition.Title)
            {
                return entry;
            }

            string fileName = Path.GetFileNameWithoutExtension(path);

            if (fileName == definition.AssetName)
            {
                return entry;
            }
        }

        return null;
    }

    private static void EnsureFolderExists(string parentFolder, string childFolder)
    {
        string fullPath = $"{parentFolder}/{childFolder}";

        if (!AssetDatabase.IsValidFolder(fullPath))
        {
            AssetDatabase.CreateFolder(parentFolder, childFolder);
        }
    }

    private static string NormalizeWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return "";
        }

        return word.Trim().ToUpperInvariant();
    }

    private static string SanitizeFileName(string fileName)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar.ToString(), "");
        }

        return fileName.Trim();
    }
}