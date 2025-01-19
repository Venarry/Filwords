using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellController : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public int x, y; // Координаты клетки
    public string letter; // Буква в клетке
    public Color originalColor;
    public Image cellImage;
    public GameObject SectetPanel;
    public CellController cellParent;

    public bool SecretWord = false;

    public bool IsLocked { get; set; } // Свойство для блокировки клетки

    private void Awake()
    {
        cellImage = GetComponent<Image>();
        if (cellImage != null)
        {
            originalColor = cellImage.color; // Сохраняем оригинальный цвет
        }
    }

    public void SetData(int x, int y, string letter)
    {
        this.x = x;
        this.y = y;

        // Преобразуем строку в StringBuilder для манипуляции символами
        StringBuilder word = new StringBuilder(letter);

        if (word[0] == '_')
        {
            // Удаляем первый символ и сохраняем результат в word
            word.Remove(0, 1);
            //cellImage.color = Color.red;
            SecretWord = true;
        }

        GetComponentInChildren<TextMeshProUGUI>().text = word.ToString();
        // Преобразуем обратно в строку и присваиваем
        this.letter = word.ToString();
        IsLocked = false; // Изначально клетки не заблокированы

    }

    public void HighlightCell(Color color)
    {
        if (cellImage != null)
        {
            cellImage.color = color;
        }
    }

    public void ResetCell()
    {
        if (cellImage != null && !IsLocked) // Сбрасываем цвет только если клетка не заблокирована
        {
            cellImage.color = originalColor;
        }
    }

    public void LockCell()
    {
        IsLocked = true; // Устанавливаем блокировку
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GridVisualizer.instance != null && cellParent == null)
        {
            GridVisualizer.instance.StartSelection(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GridVisualizer.instance != null && GridVisualizer.instance.IsSelecting() && cellParent == null)
        {
            GridVisualizer.instance.ContinueSelection(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        if (GridVisualizer.instance != null && GridVisualizer.instance.IsSelecting() && cellParent == null)
        {
            GridVisualizer.instance.EndSelection();
        }
        if (SecretWord && cellParent == null)
        {
            CellController cell = Instantiate(gameObject, SectetPanel.transform).GetComponent<CellController>();
            cell.cellParent = this;
            cellImage.color = Color.green;
            SectetPanel.GetComponent<SecretContainer>().SectretWordPoisk();
        }
    }
}