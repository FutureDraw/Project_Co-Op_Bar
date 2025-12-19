using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CdReaderMachine : MonoBehaviour
{
    [Header("References")]
    public Transform cdSlot;
    public Transform ejectPoint;
    public CdReaderUI ui;

    public CdTicket currentTicket;

    public bool HasTicket()
    {
        return currentTicket != null;
    }

    public int GetTableNumber()
    {
        return currentTicket != null ? currentTicket.tableNumber : -1;
    }

    public bool TryInsert(CdTicket ticket)
    {
        if (ticket == null || currentTicket != null)
            return false;

        currentTicket = ticket;
        StartCoroutine(FixSlotInsert(ticket));

        if (ui != null)
            ui.ShowTicketData(ticket);

        return true;
    }

    public void ConsumeTicket()
    {
        if (currentTicket == null)
            return;

        Debug.Log("consume by reader " + currentTicket);

        CdTicket ticket = currentTicket;
 
        if (ejectPoint != null)
            ticket.transform.position = ejectPoint.position;

        ticket.transform.rotation = ejectPoint.rotation;
        ticket.transform.SetParent(null);

        var grab = ticket.GetComponent<XRGrabInteractable>();
        if (grab) grab.enabled = true;

        var col = ticket.GetComponent<Collider>();
        if (col) col.enabled = true;

        var rb = ticket.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        if (ui != null)
            ui.ClearData();

        currentTicket = null;
    }

    private IEnumerator FixSlotInsert(CdTicket ticket)
    {
        ticket.transform.SetParent(cdSlot);
        ticket.transform.localPosition = Vector3.zero;
        ticket.transform.localRotation = Quaternion.identity;

        yield return new WaitForFixedUpdate();

        var grab = ticket.GetComponent<XRGrabInteractable>();
        if (grab) grab.enabled = false;

        var col = ticket.GetComponent<Collider>();
        if (col) col.enabled = false;

        var rb = ticket.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentTicket != null)
            return;

        CdTicket ticket = other.GetComponent<CdTicket>();
        if (ticket != null)
            TryInsert(ticket);
    }

    public void Eject()
    {

        CdTicket ticket = currentTicket;
        currentTicket = null;

        if (ejectPoint != null)
            ticket.transform.position = ejectPoint.position;
        ticket.transform.rotation = ejectPoint.rotation;
        ticket.transform.SetParent(null);

        XRGrabInteractable grab = ticket.GetComponent<XRGrabInteractable>();
        if (grab) grab.enabled = true;

        Collider col = ticket.GetComponent<Collider>();
        if (col) col.enabled = true;

        Rigidbody rb = ticket.GetComponent<Rigidbody>();
        if (rb != null)
            StartCoroutine(EnablePhysicsNextFrame(rb));

        if (ui != null)
            ui.ClearData();
    }

    private IEnumerator EnablePhysicsNextFrame(Rigidbody rb)
    {
        yield return new WaitForFixedUpdate();
        rb.isKinematic = false;
    }

}
