using UnityEngine;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour
{
    [Header("Настройки")]
    public Transform[] teleportTargets; // Возможные точки
    public string teleportableTag = "Teleportable"; // Тег телепортируемых объектов
    public Collider teleportZone; // Зона телепортации

    [SerializeField] private TeleportSyncSystem syncSystem;

    private int selectedTargetIndex = 0;

    private List<GameObject> objectsInZone = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger enter");
        if (other.CompareTag(teleportableTag))
        {
            Debug.Log("Nice tag");
            if (!objectsInZone.Contains(other.gameObject)) {
                objectsInZone.Add(other.gameObject);
                Debug.Log("add");
            }
            Debug.Log("trig");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectsInZone.Contains(other.gameObject)) { 
            objectsInZone.Remove(other.gameObject);
            Debug.Log("Remove");
        }
    }

    public void SelectTarget(int index)
    {
        if (index >= 0 && index < teleportTargets.Length)
            selectedTargetIndex = index;
        else
            Debug.LogWarning("Индекс точки телепортации вне диапазона");
    }

    public void TeleportObjects()
    {
        if (!syncSystem.IsTeleportAllowed())
            return;

        int index = syncSystem.GetSyncedIndex();

        Debug.Log($"tp try {objectsInZone}");
        foreach (var obj in objectsInZone)
        {
            obj.transform.position = teleportTargets[selectedTargetIndex].position;
            Debug.Log("tp");
        }
    }
}
