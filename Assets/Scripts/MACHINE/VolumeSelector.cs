using UnityEngine;
using TMPro;

public class VolumeSelector : MonoBehaviour
{
    [Header("Range in ml")]
    public float minMl = 50f;
    public float maxMl = 500f;

    [Header("UI")]
    public TextMeshPro volumeText; // формат: "200 ml"

    float currentMl = 200f;

    void Start()
    {
        currentMl = Mathf.Clamp(currentMl, minMl, maxMl);
        UpdateUI();
    }

    // Вызывайте из логики крутилки: normalized в [0..1]
    public void SetNormalizedVolume(float normalized)
    {
        normalized = Mathf.Clamp01(normalized);
        currentMl = Mathf.Lerp(minMl, maxMl, normalized);
        UpdateUI();
    }

    // Или установить конкретное значение в мл
    public void SetVolumeDirect(float ml)
    {
        currentMl = Mathf.Clamp(ml, minMl, maxMl);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (volumeText != null)
        {
            volumeText.text = $"{Mathf.RoundToInt(currentMl)} ml";
        }
    }

    public float CurrentVolumeMl => currentMl;
}
