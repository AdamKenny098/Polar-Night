using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PolarNightDay5BPopulateWordDatabase
{
    private const string GeneratorFolder = "Assets/Generator";
    private const string WordDatabasePath = "Assets/Generator/GeneratorWordDatabase.asset";

    [MenuItem("Tools/Polar Night/Day 5/5B Populate Clean Generator Words")]
    public static void PopulateCleanGeneratorWords()
    {
        EnsureFolderExists("Assets", "Generator");

        GeneratorWordDatabase database =
            AssetDatabase.LoadAssetAtPath<GeneratorWordDatabase>(WordDatabasePath);

        if (!database)
        {
            database = ScriptableObject.CreateInstance<GeneratorWordDatabase>();
            AssetDatabase.CreateAsset(database, WordDatabasePath);
        }

        database.wordSets.Clear();

        AddSet(database, GeneratorWordCategory.Mechanical, new[]
        {
            "VALVE", "FUSES", "MOTOR", "CABLE", "WIRES",
            "PANEL", "LEVER", "GEARS", "COILS", "SPARK",
            "CRANK", "BOLTS", "RIVET", "TIMER", "GAUGE",
            "PIPES", "VENTS", "ROTOR", "RELAY", "INPUT",
            "PLUGS", "DRIVE", "BRAKE", "HINGE", "SHAFT",
            "CLAMP", "FRAME", "GRATE", "LATCH", "METER",
            "CHAIN", "WHEEL", "SCREW", "DRILL", "TOOLS",
            "DUCTS", "JOINT", "SEALS", "PUMPX", "COGSA"
        });

        AddSet(database, GeneratorWordCategory.ColdWeather, new[]
        {
            "FROST", "STORM", "CHILL", "SNOWY", "WINDS",
            "WHITE", "POLAR", "NORTH", "GLAZE", "FLOES",
            "DRIFT", "SLEET", "FROZE", "CRUST", "SHARD",
            "BRINE", "WASTE", "CAVES", "RIDGE", "CLIFF",
            "NIGHT", "DARKS", "CLOAK", "BITES", "NUMBS",
            "HAILS", "GUSTS", "COVER", "SHADE", "BLAST",
            "MISTS", "CREEK", "BANKS", "ICING", "SNARE"
        });

        AddSet(database, GeneratorWordCategory.Anomaly, new[]
        {
            "ECHOX", "TRACE", "STAIR", "HUMMS", "WOUND",
            "SIGIL", "RIFTS", "GHOST", "OTHER", "DREAM",
            "VOICE", "SHAPE", "THING", "FACET", "PULSE",
            "VEINS", "SPORE", "ABYSS", "FLESH", "WRATH",
            "SHADE", "ORBIT", "CRYPT", "ALTAR", "GRASP",
            "STARE", "WATCH", "NOISE", "GLARE", "GLOOM",
            "CYCLE", "MARKS", "OMENS", "BLOOM", "PHASE"
        });

        AddSet(database, GeneratorWordCategory.Emergency, new[]
        {
            "ALARM", "FAULT", "ERROR", "PANIC", "BREAK",
            "RESET", "POWER", "DRAIN", "SURGE", "SMOKE",
            "ALERT", "SIREN", "LOCKS", "FAILS",
            "CRASH", "BURNS", "SHORT", "BLEED", "CLOGS",
            "LEAKS", "PRESS", "BLINK", "SHAKE", "CRACK",
            "BURST", "STUCK", "EMPTY", "FINAL", "RETRY",
            "PATCH", "SHOCK", "FLARE", "CHAOS", "RUPTR"
        });

        AddSet(database, GeneratorWordCategory.Containment, new[]
        {
            "CELLS", "CHAIN", "LOCKS", "DOORS", "GLASS",
            "VAULT", "SEALS", "FIELD", "ALPHA", "OMEGA",
            "BLOCK", "GUARD", "WATCH", "LEVEL", "ENTRY",
            "CAGES", "BOUND", "SNARE", "TRAPS", "BAITS",
            "ALLOY", "LASER", "STUDY", "TESTS", "NOTES",
            "RISKS", "INNER", "OUTER", "CLEAN", "QUIET",
            "SCOPE", "TOWER", "HINGE", "LATCH", "PANEL"
        });

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Day 5B complete. Clean generator word database populated at {WordDatabasePath}");
    }

    private static void AddSet(
        GeneratorWordDatabase database,
        GeneratorWordCategory category,
        string[] words
    )
    {
        GeneratorWordSet set = new GeneratorWordSet
        {
            category = category,
            words = new List<string>()
        };

        HashSet<string> seen = new HashSet<string>();

        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i].Trim().ToUpperInvariant();

            if (word.Length != 5)
            {
                Debug.LogWarning($"Skipped '{word}' in {category}. Word must be exactly 5 letters.");
                continue;
            }

            if (seen.Contains(word))
            {
                continue;
            }

            seen.Add(word);
            set.words.Add(word);
        }

        database.wordSets.Add(set);
    }

    private static void EnsureFolderExists(string parentFolder, string childFolder)
    {
        string fullPath = $"{parentFolder}/{childFolder}";

        if (!AssetDatabase.IsValidFolder(fullPath))
        {
            AssetDatabase.CreateFolder(parentFolder, childFolder);
        }
    }
}