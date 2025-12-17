using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CdReaderMachine : MonoBehaviour
{
    [Header("References")]
    public Transform cdSlot;
    public Transform ejectPoint;
    public CdReaderUI ui;
    public Collider insertTrigger;

    private CdTicket currentTicket;


    private void Awake()
    {
        if (ui != null)
            ui.SetMachineRef(this);
    }

    // Вставка диска
    public bool TryInsert(CdTicket ticket)
    {
        if (ticket == null || currentTicket != null) return false;

        currentTicket = ticket;

        StartCoroutine(FixSlotInsert(ticket));

        if (ui != null)
            ui.ShowTicketData(ticket);

        return true;
    }

    private IEnumerator FixSlotInsert(CdTicket ticket)
    {
        ticket.transform.SetParent(cdSlot);
        ticket.transform.localPosition = Vector3.zero;
        ticket.transform.localRotation = Quaternion.identity;

        yield return new WaitForFixedUpdate();

        XRGrabInteractable grab = ticket.GetComponent<XRGrabInteractable>();
        if (grab) grab.enabled = false;

        Collider col = ticket.GetComponent<Collider>();
        if (col) col.enabled = false;

        Rigidbody rb = ticket.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    public void Eject()
    {
        if (currentTicket == null) return;

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

    public bool HasTicket()
    {
        return currentTicket != null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentTicket != null) return;

        CdTicket ticket = other.GetComponent<CdTicket>();
        if (ticket != null)
        {
            TryInsert(ticket);
        }
    }
}
