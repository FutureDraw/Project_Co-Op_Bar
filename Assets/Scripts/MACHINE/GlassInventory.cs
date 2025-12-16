using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrinkRecord
{
    public string id;
    public string displayName;
    public float volumeMl;
}

public class GlassInventory : MonoBehaviour
{
    public List<DrinkRecord> contents = new List<DrinkRecord>();

    // Добавляет напиток: если последний элемент имеет тот же id -> суммируем, иначе добавляем новый элемент.
    public void AddDrink(string id, string displayName, float volumeMl)
    {
        if (volumeMl <= 0f) return;

        if (contents.Count > 0)
        {
            var last = contents[contents.Count - 1];
            if (last.id == id)
            {
                last.volumeMl += volumeMl;
                return;
            }
        }

        DrinkRecord rec = new DrinkRecord { id = id, displayName = displayName, volumeMl = volumeMl };
        contents.Add(rec);
    }

    // Удобные методы для отладки / UI
    public string GetContentsSummary()
    {
        if (contents.Count == 0) return "Empty";
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var r in contents)
        {
            sb.AppendLine($"{r.displayName}: {Mathf.RoundToInt(r.volumeMl)} ml");
        }
        return sb.ToString();
    }
}
