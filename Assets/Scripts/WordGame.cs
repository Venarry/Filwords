using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WordGame : MonoBehaviour
{
    public string secretWord;
    public List<string> selectedWords = new List<string>();


    public int GetSecretWordLength()
    {
        if (string.IsNullOrEmpty(secretWord)) return 0;
        return secretWord.Length;
    }

    public void clear()
    {
        selectedWords.Clear();
        secretWord = null;
    }
}
