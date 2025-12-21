using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TicketWatcher : MonoBehaviour
{
    [Header("Источник cdTicket")]
    public CdReaderMachine ticketSource;

    [Header("Элменты на отключение")]
    public XRLeverAdvancedSmooth lever;
    public XRSimpleInteractable button1;
    public XRSimpleInteractable button2;
    public XRKnob knob;

    [Header("Индикатор")]
    public Renderer indicatorRenderer;
    public float fadeSpeed = 2f;

    float currentAlpha;

    void Start()
    {
        currentAlpha = indicatorRenderer.material.color.a;
    }


    void Update()
    {
        bool hasTicket = ticketSource.currentTicket != null;

        lever.enabled = hasTicket;
        button1.enabled = hasTicket;
        button2.enabled = hasTicket;
        knob.enabled = hasTicket;
        


        // Плавный переход прозрачности
        float targetAlpha = hasTicket ? 1f : 0f;
        currentAlpha = Mathf.MoveTowards(
            currentAlpha,
            targetAlpha,
            fadeSpeed * Time.deltaTime
        );

        SetIndicatorAlpha(currentAlpha);
    }

    void SetIndicatorAlpha(float alpha)
    {
        Color c = indicatorRenderer.material.color;
        c.a = alpha;
        indicatorRenderer.material.color = c;
    }

}
