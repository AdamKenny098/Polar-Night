using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GeneratorStickyNoteBoard : MonoBehaviour
{
    public static GeneratorStickyNoteBoard Instance;

    [Header("Sticky Note Texts")]
    public TMP_Text[] noteTexts = new TMP_Text[6];

    [Header("Display")]
    public bool clearOnStart = true;
    public string emptyText = "";

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (clearOnStart)
        {
            ClearBoard();
        }
    }

    public void FillBoard(string correctWord, List<string> falseWords)
    {
        if (noteTexts == null || noteTexts.Length == 0)
        {
            Debug.LogWarning("GeneratorStickyNoteBoard has no note texts assigned.");
            return;
        }

        List<string> boardWords = new List<string>();

        string cleanedCorrectWord = CleanWord(correctWord);

        if (!string.IsNullOrWhiteSpace(cleanedCorrectWord))
        {
            boardWords.Add(cleanedCorrectWord);
        }

        if (falseWords != null)
        {
            for (int i = 0; i < falseWords.Count; i++)
            {
                string falseWord = CleanWord(falseWords[i]);

                if (string.IsNullOrWhiteSpace(falseWord))
                {
                    continue;
                }

                if (boardWords.Contains(falseWord))
                {
                    continue;
                }

                boardWords.Add(falseWord);

                if (boardWords.Count >= noteTexts.Length)
                {
                    break;
                }
            }
        }

        Shuffle(boardWords);

        for (int i = 0; i < noteTexts.Length; i++)
        {
            if (!noteTexts[i])
            {
                continue;
            }

            if (i < boardWords.Count)
            {
                noteTexts[i].text = boardWords[i];
            }
            else
            {
                noteTexts[i].text = emptyText;
            }
        }
    }

    public void ClearBoard()
    {
        if (noteTexts == null)
        {
            return;
        }

        for (int i = 0; i < noteTexts.Length; i++)
        {
            if (noteTexts[i])
            {
                noteTexts[i].text = emptyText;
            }
        }
    }

    private string CleanWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            return "";
        }

        return word.Trim().ToUpperInvariant();
    }

    private void Shuffle(List<string> words)
    {
        for (int i = 0; i < words.Count; i++)
        {
            int randomIndex = Random.Range(i, words.Count);

            string temp = words[i];
            words[i] = words[randomIndex];
            words[randomIndex] = temp;
        }
    }
}