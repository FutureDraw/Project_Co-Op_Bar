using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TicketCounterZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Collider countZone;
    [SerializeField] private TMP_Text counterText;

    private readonly HashSet<CdTicket> ticketsInside = new();

    private void Reset()
    {
        countZone = GetComponent<Collider>();
        if (countZone != null)
            countZone.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        CdTicket ticket = other.GetComponent<CdTicket>();
        if (ticket == null || ticket.isUsed == false)
            return;

        if (ticketsInside.Add(ticket))
            UpdateText();
    }

    private void OnTriggerExit(Collider other)
    {
        CdTicket ticket = other.GetComponent<CdTicket>();
        if (ticket == null)
            return;

        if (ticketsInside.Remove(ticket))
            UpdateText();
    }

    private void UpdateText()
    {
        if (counterText != null)
            counterText.text = ticketsInside.Count.ToString() + " завершено";
    }

    // На случай если тикет был уничтожен внутри зоны
    private void LateUpdate()
    {
        bool changed = false;

        foreach (var ticket in new List<CdTicket>(ticketsInside))
        {
            if (ticket == null)
            {
                ticketsInside.Remove(ticket);
                changed = true;
            }
        }

        if (changed)
            UpdateText();
    }
}
