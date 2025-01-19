using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public WordLoader wordLoader;
    public WordGame wordGame;
    public GridGenerator gridGenerator;
    public GridVisualizer gridVisualizer;

    void Start()
    {
        //StartCoroutine(RunGameFlow());
    }

    public IEnumerator RunGameFlow()
    {
        Debug.Log("Загрузка слов...");
        wordLoader.LoadWords();

        if (WordLoader.words == null || WordLoader.words.Count == 0)
        {
            Debug.LogError("Список слов пуст! Проверьте JSON-файл.");
            yield break;
        }

        while (true)
        {
            Debug.Log("Подбор слов для заполнения сетки...");

            gridGenerator.wordsToPlace = new List<string>(WordLoader.words);

            Debug.Log("Генерация сетки...");
            bool success = false;
            yield return StartCoroutine(gridGenerator.GenerateGridCoroutine(result => success = result));

            if (success)
            {
                Debug.Log("Сетка успешно сгенерирована!");
                break;
            }
            else
            {
                Debug.LogWarning("Не удалось сгенерировать сетку. Повторная попытка...");

                yield return StartCoroutine(gridGenerator.GenerateGridCoroutine(result => success = result));
            }
        }

        Debug.Log("Визуализация...");
        gridVisualizer.gridGenerator = gridGenerator;
        yield return StartCoroutine(gridVisualizer.VisualizeGridCoroutine());

        Debug.Log("Игра успешно запущена!");
    }
}