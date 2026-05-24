using System.Collections.Generic;
using UnityEngine;

public enum GeneratorWordCategory
{
    Mechanical,
    ColdWeather,
    Anomaly,
    Emergency,
    Containment
}

[System.Serializable]
public class GeneratorWordSet
{
    public GeneratorWordCategory category;
    public List<string> words = new List<string>();
}

[CreateAssetMenu(menuName = "Data/Generator Word Database")]
public class GeneratorWordDatabase : ScriptableObject
{
    public List<GeneratorWordSet> wordSets = new List<GeneratorWordSet>();

    public string GetRandomWord(int length, params GeneratorWordCategory[] categories)
    {
        List<string> validWords = GetWords(length, categories);

        if (validWords.Count == 0)
        {
            return "";
        }

        return validWords[Random.Range(0, validWords.Count)];
    }

    public List<string> GetWords(int length, params GeneratorWordCategory[] categories)
    {
        List<string> validWords = new List<string>();
        HashSet<string> seenWords = new HashSet<string>();

        for (int i = 0; i < wordSets.Count; i++)
        {
            GeneratorWordSet set = wordSets[i];

            if (set == null || !CategoryAllowed(set.category, categories))
            {
                continue;
            }

            for (int j = 0; j < set.words.Count; j++)
            {
                string word = NormalizeWord(set.words[j]);

                if (word.Length == length && !seenWords.Contains(word))
                {
                    seenWords.Add(word);
                    validWords.Add(word);
                }
            }
        }

        return validWords;
    }

    private bool CategoryAllowed(GeneratorWordCategory category, GeneratorWordCategory[] categories)
    {
        if (categories == null || categories.Length == 0)
        {
            return true;
        }

        for (int i = 0; i < categories.Length; i++)
        {
            if (category == categories[i])
            {
                return true;
            }
        }

        return false;
    }

    private string NormalizeWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return "";
        }

        return word.Trim().ToUpperInvariant();
    }
}