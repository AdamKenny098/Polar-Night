using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PolarNightDay5CValidateGeneratorWords
{
    private const string WordDatabasePath = "Assets/Generator/GeneratorWordDatabase.asset";
    private const int MinimumWordsPerCategory = 20;

    private static readonly HashSet<string> BannedWords = new HashSet<string>
    {
        "SIGNL",
        "CAPTR",
        "ARCTC",
        "FRIGD",
        "TUNDR",
        "ECHOX",
        "PUMPX",
        "COGSA",
        "RUPTR",
        "FIREX",
        "HUMMS",
        "VANTA"
    };

    [MenuItem("Tools/Polar Night/Day 5/5C Validate Generator Words")]
    public static void ValidateGeneratorWords()
    {
        int errors = 0;
        int warnings = 0;
        int removed = 0;

        GeneratorWordDatabase database =
            AssetDatabase.LoadAssetAtPath<GeneratorWordDatabase>(WordDatabasePath);

        if (!database)
        {
            Debug.LogError($"Missing GeneratorWordDatabase at: {WordDatabasePath}");
            return;
        }

        if (database.wordSets == null)
        {
            Debug.LogError("GeneratorWordDatabase wordSets list is null.");
            return;
        }

        ValidateAndCleanDatabase(database, ref errors, ref warnings, ref removed);
        ValidateRequiredCategories(database, ref errors, ref warnings);
        ValidateSceneGenerator(database, ref errors, ref warnings);

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        if (errors == 0 && warnings == 0)
        {
            Debug.Log($"Day 5C passed. Generator word database is clean. Removed entries: {removed}.");
        }
        else if (errors == 0)
        {
            Debug.LogWarning($"Day 5C passed with {warnings} warning(s). Removed entries: {removed}.");
        }
        else
        {
            Debug.LogError($"Day 5C failed with {errors} error(s), {warnings} warning(s). Removed entries: {removed}.");
        }
    }

    private static void ValidateAndCleanDatabase(
        GeneratorWordDatabase database,
        ref int errors,
        ref int warnings,
        ref int removed
    )
    {
        HashSet<string> globalWords = new HashSet<string>();

        for (int i = 0; i < database.wordSets.Count; i++)
        {
            GeneratorWordSet set = database.wordSets[i];

            if (set == null)
            {
                Debug.LogError($"Word set at index {i} is null.");
                errors++;
                continue;
            }

            if (set.words == null)
            {
                Debug.LogError($"Word set '{set.category}' has a null word list.");
                errors++;
                continue;
            }

            List<string> cleanedWords = new List<string>();
            HashSet<string> localWords = new HashSet<string>();

            for (int j = 0; j < set.words.Count; j++)
            {
                string cleaned = CleanWord(set.words[j]);

                if (string.IsNullOrWhiteSpace(cleaned))
                {
                    removed++;
                    continue;
                }

                if (cleaned.Length != 5)
                {
                    Debug.LogWarning($"Removed '{cleaned}' from {set.category}. Generator words must be exactly 5 letters.");
                    removed++;
                    warnings++;
                    continue;
                }

                if (BannedWords.Contains(cleaned))
                {
                    Debug.LogWarning($"Removed banned placeholder word '{cleaned}' from {set.category}.");
                    removed++;
                    warnings++;
                    continue;
                }

                if (localWords.Contains(cleaned))
                {
                    Debug.LogWarning($"Removed duplicate word '{cleaned}' inside category {set.category}.");
                    removed++;
                    warnings++;
                    continue;
                }

                localWords.Add(cleaned);
                cleanedWords.Add(cleaned);

                if (globalWords.Contains(cleaned))
                {
                    Debug.LogWarning($"Word '{cleaned}' appears in more than one category. This is allowed, but may reduce category identity.");
                    warnings++;
                }
                else
                {
                    globalWords.Add(cleaned);
                }
            }

            set.words = cleanedWords;

            if (set.words.Count < MinimumWordsPerCategory)
            {
                Debug.LogWarning($"Category {set.category} only has {set.words.Count} word(s). Recommended minimum: {MinimumWordsPerCategory}.");
                warnings++;
            }

            Debug.Log($"Validated {set.category}: {set.words.Count} word(s).");
        }
    }

    private static void ValidateRequiredCategories(
        GeneratorWordDatabase database,
        ref int errors,
        ref int warnings
    )
    {
        HashSet<GeneratorWordCategory> foundCategories = new HashSet<GeneratorWordCategory>();

        for (int i = 0; i < database.wordSets.Count; i++)
        {
            GeneratorWordSet set = database.wordSets[i];

            if (set != null)
            {
                foundCategories.Add(set.category);
            }
        }

        foreach (GeneratorWordCategory category in System.Enum.GetValues(typeof(GeneratorWordCategory)))
        {
            if (!foundCategories.Contains(category))
            {
                Debug.LogError($"GeneratorWordDatabase is missing category: {category}");
                errors++;
            }
        }
    }

    private static void ValidateSceneGenerator(
        GeneratorWordDatabase database,
        ref int errors,
        ref int warnings
    )
    {
        GeneratorMinigame generator = Object.FindObjectOfType<GeneratorMinigame>();

        if (!generator)
        {
            Debug.LogWarning("No GeneratorMinigame found in the current scene. Database validation still completed.");
            warnings++;
            return;
        }

        if (generator.wordDatabase != database)
        {
            Debug.LogWarning("GeneratorMinigame does not reference the main GeneratorWordDatabase. Assigning it now.");
            generator.wordDatabase = database;
            EditorUtility.SetDirty(generator);
            warnings++;
        }

        if (!generator.useContextCategories)
        {
            Debug.LogWarning("GeneratorMinigame has useContextCategories disabled. Enabling it now.");
            generator.useContextCategories = true;
            EditorUtility.SetDirty(generator);
            warnings++;
        }

        if (!generator.autoResolveContext)
        {
            Debug.LogWarning("GeneratorMinigame has autoResolveContext disabled. Enabling it now.");
            generator.autoResolveContext = true;
            EditorUtility.SetDirty(generator);
            warnings++;
        }

        Debug.Log("Scene GeneratorMinigame validated.");
    }

    private static string CleanWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return "";
        }

        return word.Trim().ToUpperInvariant();
    }
}