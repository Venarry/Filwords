using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class GridGenerator : MonoBehaviour
{
    public List<string> wordsToPlace; // ������ ���� ��� ����������
    public string[,] grid;             // �����
    public int gridSize;             // ������ �����
    public string secretWord;        // ������ �����
    private HashSet<(int, int)> protectedCells = new HashSet<(int, int)>(); // ������, ���������� �� ���������

    private (int dx, int dy)[] directions = new (int, int)[]
    {
        (1, 0), (0, 1), // ������, ����
        (-1, 0), (0, -1) // �����, �����
    };

    public void InitializeGrid()
    {
        grid = new string[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = "_"; // ������ ������
            }
        }
        protectedCells.Clear();
    }

    public IEnumerator GenerateGridCoroutine(System.Action<bool> onComplete)
    {
        InitializeGrid();

        // ��������� ��������� �����
        if (!SelectRandomWords())
        {
            Debug.LogError("�� ������� ��������� ����� ��� ���������� �����.");
            onComplete?.Invoke(false);
            yield break;
        }

        // �������� ������ ����� �� ������
        secretWord = wordsToPlace[Random.Range(0, wordsToPlace.Count)];
        wordsToPlace.Remove(secretWord);

        // ��������� ������ �����
        //if (!PlaceSecretWord(secretWord))
        //{
        //    Debug.LogError("�� ������� ���������� ������ �����.");
        //    onComplete?.Invoke(false);
        //    yield break;
        //}

        // ��������� ���������� ����� � �����
        foreach (var word in wordsToPlace)
        {
            Debug.Log(word);

            if (!TryPlaceWordWithTurns(word))
            {
                Debug.LogWarning($"�� ������� ���������� �����: {word}");
                onComplete?.Invoke(false);
                yield break;
                ///////////////////////////////////////////////////////////////////////////////
            }
        }

        Debug.LogWarning($"�������");
        // ��������� ���������� ������ ������ ������� �� ������� �����
        FillEmptyCellsWithSecretWord();

        onComplete?.Invoke(true);
        yield return null;
    }

    private bool SelectRandomWords()
    {
        int totalCells = gridSize * gridSize;

        List<string> randomWords = WordLoader.words
            .Where(word => word.Length <= totalCells && word.All(char.IsLetter)) // ��������� ������ ����� � �������
            .OrderBy(_ => Random.value)
            .ToList();

        List<string> selectedWords = new List<string>();
        int usedCells = 0;

        foreach (var word in randomWords)
        {
            if (usedCells + word.Length <= totalCells)
            {
                selectedWords.Add(word);
                usedCells += word.Length;

                if (usedCells == totalCells)
                {
                    wordsToPlace = selectedWords;
                    return true;
                }
            }
        }

        return false; // �� ������� ��������� ����� ��� ���������� �����
    }

    private bool TryPlaceWordWithTurns(string word)
    {
        List<(int, int)> possiblePositions = new List<(int, int)>();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y] == "_")
                {
                    possiblePositions.Add((x, y));
                }
            }
        }

        possiblePositions = possiblePositions.OrderBy(_ => Random.value).ToList();

        foreach (var (startX, startY) in possiblePositions)
        {
            foreach (var (dx, dy) in directions)
            {
                if (CanPlaceWordWithTurn(word, startX, startY, dx, dy))
                {
                    return true; // ����� ��������� �������
                }
            }
        }

        return false; // �� ������� ���������� �����
    }

    private bool CanPlaceWordWithTurn(string word, int x, int y, int dx, int dy)
    {
        int halfLength = word.Length / 2;
        for (int i = 0; i < word.Length; i++)
        {
            int newX = x + (i < halfLength ? i * dx : halfLength * dx + (i - halfLength) * dy);
            int newY = y + (i < halfLength ? i * dy : halfLength * dy + (i - halfLength) * -dx);

            if (!IsInsideGrid(newX, newY) || (grid[newX, newY] != "_" && !protectedCells.Contains((newX, newY))))
            {
                return false;
            }
        }

        PlaceWordWithTurn(word, x, y, dx, dy, halfLength);
        return true;
    }

    private void PlaceWordWithTurn(string word, int x, int y, int dx, int dy, int halfLength)
    {
        for (int i = 0; i < word.Length; i++)
        {
            int newX = x + (i < halfLength ? i * dx : halfLength * dx + (i - halfLength) * dy);
            int newY = y + (i < halfLength ? i * dy : halfLength * dy + (i - halfLength) * -dx);

            grid[newX, newY] = word[i].ToString();
        }
    }

    private bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < gridSize && y < gridSize;
    }

    private void FillEmptyCellsWithSecretWord()
    {
        int secretIndex = 0;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y] == "_")
                {
                    // ����������� ������, � �� ������
                    grid[x, y] = "_" + secretWord[secretIndex % secretWord.Length];
                    Debug.Log("����� � ����� "+grid[x, y]+" || "+ grid[x, y].ToString());
                    secretIndex++;
                }
            }
        }
    }
}