using UnityEngine;
using TMPro;

public class PouringSystem : MonoBehaviour
{
    [Header("References")]
    public DrinkSelector drinkSelector;
    public VolumeSelector volumeSelector;

    [Header("Current glass in trigger (устанавливает PourTrigger)")]
    [HideInInspector] public GlassInventory currentGlass;

    [Header("Optional UI")]
    public TextMeshProUGUI pourLogText; // краткий лог последнего налива

    // Вызывать при срабатывании рычага
    public void PourCurrentSelection()
    {
        if (drinkSelector == null || volumeSelector == null)
        {
            Debug.LogWarning("DrinkSelector или VolumeSelector не назначены в PouringSystem.");
            return;
        }

        if (currentGlass == null)
        {
            Debug.Log("Нет кружки в триггере — наливать нельзя.");
            return;
        }

        var drink = drinkSelector.CurrentDrink;
        if (drink == null)
        {
            Debug.Log("Выбранного напитка нет.");
            return;
        }

        float vol = volumeSelector.CurrentVolumeMl;
        if (vol <= 0f)
        {
            Debug.Log("Объём нулевой.");
            return;
        }

        currentGlass.AddDrink(drink.id, drink.displayName, vol);
        if (pourLogText != null) pourLogText.text = $"Налито: {drink.displayName} — {Mathf.RoundToInt(vol)} ml";

        Debug.Log($"Налили в кружку: {drink.displayName} {Mathf.RoundToInt(vol)} ml");
    }
}
