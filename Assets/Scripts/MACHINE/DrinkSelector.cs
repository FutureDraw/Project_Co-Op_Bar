using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DrinkData
{
    public string id;           // уникальный идентификатор (например "coffee", "beer")
    public string displayName;  // имя для UI
}

public class DrinkSelector : MonoBehaviour
{
    [Header("Slots")]
    public List<Transform> indicatorSlots = new List<Transform>(); // 7 transforms (позиции слотов)
    public Transform indicatorCube; // сам куб-индексатор

    [Header("Drinks")]
    public List<DrinkData> drinks = new List<DrinkData>(); // должен соответствовать числу слотов

    [Header("Movement")]
    public float moveSpeed = 8f; // скорость плавного перемещения

    [Header("UI (опционально)")]
    public TextMeshProUGUI drinkNameText;

    int currentIndex = 0;
    Vector3 targetPosition;

    void Start()
    {
        if (indicatorSlots.Count == 0) Debug.LogError("Indicator slots not set.");
        if (drinks.Count != indicatorSlots.Count) Debug.LogWarning("Drinks count != slots count. Лучше сделать одинаковыми.");
        currentIndex = Mathf.Clamp(currentIndex, 0, Mathf.Max(0, indicatorSlots.Count - 1));
        targetPosition = indicatorSlots[currentIndex].position;
        if (indicatorCube) indicatorCube.position = targetPosition;
        UpdateUI();
    }

    void Update()
    {
        if (indicatorCube != null)
        {
            indicatorCube.position = Vector3.Lerp(indicatorCube.position, targetPosition, Time.deltaTime * moveSpeed);
        }
    }

    public void NextDrink()
    {
        if (indicatorSlots.Count == 0) return;
        currentIndex = Mathf.Clamp(currentIndex + 1, 0, indicatorSlots.Count - 1);
        targetPosition = indicatorSlots[currentIndex].position;
        UpdateUI();
    }

    public void PrevDrink()
    {
        if (indicatorSlots.Count == 0) return;
        currentIndex = Mathf.Clamp(currentIndex - 1, 0, indicatorSlots.Count - 1);
        targetPosition = indicatorSlots[currentIndex].position;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (drinkNameText != null && drinks.Count > currentIndex)
            drinkNameText.text = drinks[currentIndex].displayName;
    }

    public int CurrentIndex => currentIndex;
    public DrinkData CurrentDrink => (drinks != null && drinks.Count > currentIndex) ? drinks[currentIndex] : null;
}
