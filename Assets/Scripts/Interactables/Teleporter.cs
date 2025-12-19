using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Common teleport")]
    [SerializeField] private Transform[] teleportTargets;

    [Header("Ticket teleport")]
    [SerializeField] private Transform[] tableTeleportPoints;
    [SerializeField] private int ticketModeIndex = 3;

    [Header("Systems")]
    [SerializeField] public TeleportSyncSystem syncSystem;
    [SerializeField] public CdReaderMachine reader;

    [Header("Rules")]
    [SerializeField] private string consumeTicketTag = "Glass";

    private readonly List<GameObject> objectsInZone = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!objectsInZone.Contains(other.gameObject))
            objectsInZone.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        objectsInZone.Remove(other.gameObject);
    }

    public void TeleportObjects()
    {
        if (!syncSystem.IsTeleportAllowed())
            return;

        int mode = syncSystem.GetSyncedIndex();

        foreach (var obj in objectsInZone)
        {
            if (mode == ticketModeIndex)
                TeleportByTicket(obj);
            else
                TeleportNormally(obj, mode);
        }
    }

    private void TeleportNormally(GameObject obj, int index)
    {
        if (index < 0 || index >= teleportTargets.Length)
            return;

        obj.transform.position = teleportTargets[index].position;
        obj.transform.rotation = teleportTargets[index].rotation;
    }

    private void TeleportByTicket(GameObject obj)
    {
        // режим 3 без тикета
        if (!reader.HasTicket())
            return;

        int tableNumber = reader.GetTableNumber();
        int tableIndex = tableNumber - 1;

        if (tableIndex < 0 || tableIndex >= tableTeleportPoints.Length)
            return;

        obj.transform.position = tableTeleportPoints[tableIndex].position;
        obj.transform.rotation = tableTeleportPoints[tableIndex].rotation;

        // использовать тикет ТОЛЬКО если это Glass
        if (obj.CompareTag(consumeTicketTag))
        {
            reader.ConsumeTicket();
            Debug.Log("consume");
        }
    }
}
