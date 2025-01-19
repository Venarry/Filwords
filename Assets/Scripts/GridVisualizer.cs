using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class GridVisualizer : MonoBehaviour
{
    public static GridVisualizer instance;

    [Header("Grid Configuration")]
    public GridLayoutGroup gridLayout;
    public GameObject SectretContainer;
    public GameObject cellPrefab;
    public GridGenerator gridGenerator;

    private List<CellController> selectedCells = new List<CellController>();
    private bool isSelecting = false;
    public GameObject LoadPanel;

    private Queue<Color> recentColors = new Queue<Color>(); // Очередь для уникальных цветов

    void Awake()
    {
        instance = this;
        LoadPanel.SetActive(true);
    }

    public IEnumerator VisualizeGridCoroutine()
    {
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridGenerator.gridSize;

        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        string[,] grid = gridGenerator.grid;

        for (int x = 0; x < gridGenerator.gridSize; x++)
        {
            for (int y = 0; y < gridGenerator.gridSize; y++)
            {
                GameObject cell = Instantiate(cellPrefab, gridLayout.transform);
                cell.GetComponent<CellController>().SectetPanel = SectretContainer;
                TextMeshProUGUI cellText = cell.GetComponentInChildren<TextMeshProUGUI>();
                if (cellText != null)
                {
                    cellText.text = grid[x, y].ToString();
                }

                CellController cellController = cell.GetComponent<CellController>();
                if (cellController != null)
                {
                    cellController.SetData(x, y, grid[x, y]);
                    cellController.IsLocked = false; // Инициализация состояния клетки
                }

                yield return null;
            }
        }
        LoadPanel.SetActive(false);
        Debug.Log("Сетка визуализирована и готова.");
    }

    public void StartSelection(CellController cell)
    {
        if (cell.IsLocked)
        {
            Debug.LogWarning("Эта клетка уже заблокирована.");
            return;
        }

        if (selectedCells.Count > 0 && !IsAdjacent(cell, selectedCells[selectedCells.Count - 1]))
        {
            ResetSelection(); // Сбрасываем выбор, если клетка не является соседней
        }

        isSelecting = true;
        AddCellToSelection(cell);
    }

    public void ContinueSelection(CellController cell)
    {
        if (isSelecting && !cell.IsLocked && IsAdjacent(cell, selectedCells[selectedCells.Count - 1]))
        {
            AddCellToSelection(cell);
        }
    }

    public void EndSelection()
    {
        isSelecting = false;

        string word = string.Concat(selectedCells.ConvertAll(c => c.letter));
        if (gridGenerator.wordsToPlace.Contains(word))
        {
            Debug.Log("Правильное слово выбрано!");

            // Применяем случайный цвет к выбранным клеткам
            ApplyRandomColorToSelection();

            // Блокируем выбранные клетки
            LockSelectedCells();

            selectedCells.Clear();
        }
        else
        {
            Debug.LogWarning("Выбрано неправильное слово.");
            ResetSelection();
        }
    }

    public bool IsSelecting()
    {
        return isSelecting;
    }

    private void AddCellToSelection(CellController cell)
    {
        if (!selectedCells.Contains(cell))
        {
            selectedCells.Add(cell);
            cell.HighlightCell(Color.yellow); // Выделяем клетку жёлтым цветом
        }
    }

    public void ResetSelection()
    {
        foreach (var cell in selectedCells)
        {
            cell.ResetCell();
        }
        selectedCells.Clear();
        isSelecting = false;
    }

    private void ApplyRandomColorToSelection()
    {
        Color uniqueColor = GenerateUniqueColor();
        foreach (var cell in selectedCells)
        {
            cell.HighlightCell(uniqueColor);
        }
    }

    private void LockSelectedCells()
    {
        foreach (var cell in selectedCells)
        {
            cell.IsLocked = true;
        }
    }

    private Color GenerateUniqueColor()
    {
        Color randomColor = new Color(Random.Range(0.2f, 1f), Random.Range(0.2f, 1f), Random.Range(0.2f, 1f));
        while (recentColors.Contains(randomColor))
        {
            randomColor = new Color(Random.Range(0.2f, 1f), Random.Range(0.2f, 1f), Random.Range(0.2f, 1f));
        }

        if (recentColors.Count >= 5)
        {
            recentColors.Dequeue();
        }
        recentColors.Enqueue(randomColor);

        return randomColor;
    }

    private bool IsAdjacent(CellController cell1, CellController cell2)
    {
        int dx = Mathf.Abs(cell1.x - cell2.x);
        int dy = Mathf.Abs(cell1.y - cell2.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    public void ClearGrid()
    {
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Сетка очищена.");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}