using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class WordList
{
    public List<string> words;
}

public class WordLoader : MonoBehaviour
{
    public string jsonFileName = "words.json";
    static public List<string> words;
    private bool load = false;

    public void LoadWords()
    {
        if (load)
            return;

        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            WordList wordList = JsonUtility.FromJson<WordList>(json);
            words = wordList.words;
        }
        else
        {
            //Debug.LogError("Файл не найден: " + filePath);
        }
    }
}
