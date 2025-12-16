using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PourTrigger : MonoBehaviour
{
    public PouringSystem pouringSystem;

    // В триггере может быть несколько кружек — будем хранить список.
    List<GlassInventory> glassesInTrigger = new List<GlassInventory>();

    void Reset()
    {
        Collider c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Glass")) return;

        var gi = other.GetComponent<GlassInventory>();
        if (gi == null) return;

        glassesInTrigger.Add(gi);
        UpdateCurrentGlass();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Glass")) return;

        var gi = other.GetComponent<GlassInventory>();
        if (gi == null) return;

        glassesInTrigger.Remove(gi);
        UpdateCurrentGlass();
    }

    void UpdateCurrentGlass()
    {
        if (pouringSystem == null) return;

        // логика выбора: если несколько кружек — используем последнюю попавшуюся в список
        if (glassesInTrigger.Count > 0)
            pouringSystem.currentGlass = glassesInTrigger[glassesInTrigger.Count - 1];
        else
            pouringSystem.currentGlass = null;
    }
}
