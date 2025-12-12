using UnityEngine;

public class CdReaderMachine : MonoBehaviour
{
    [Header("References")]
    public Transform cdSlot;
    public CdReaderUI ui;
    public Collider insertTrigger;

    private CdTicket currentTicket;

    public bool TryInsert(CdTicket ticket)
    {
        if (ticket == null) return false;
        if (currentTicket != null) return false;

        currentTicket = ticket;

        ticket.transform.position = cdSlot.position;
        ticket.transform.rotation = cdSlot.rotation;
        ticket.transform.SetParent(cdSlot, true);

        Rigidbody rb = ticket.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider col = ticket.GetComponent<Collider>();
        if (col) col.enabled = false;

        ui.ShowTicketData(ticket);

        return true;
    }

    public CdTicket Eject()
    {
        if (currentTicket == null) return null;

        CdTicket ticket = currentTicket;
        currentTicket = null;

        ui.Hide();

        Rigidbody rb = ticket.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        Collider col = ticket.GetComponent<Collider>();
        if (col) col.enabled = true;

        ticket.transform.SetParent(null, true);

        ticket.transform.position = cdSlot.position + cdSlot.forward * 0.5f;

        return ticket;
    }

    public bool HasTicket() => currentTicket != null;

    private void OnTriggerEnter(Collider other)
    {
        if (currentTicket != null) return;
        CdTicket t = other.GetComponent<CdTicket>();
        if (t != null)
        {
            TryInsert(t);
        }
    }
}
