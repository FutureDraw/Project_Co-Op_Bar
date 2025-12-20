using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SlowLever : XRBaseInteractable
{
    [Header("References")]
    [SerializeField] private Transform handle;
    [SerializeField] private Transform interactorAttach;

    [Header("Angles")]
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 65f;

    [Header("Heavy Feeling")]
    [Tooltip("Насколько медленно игрок может тянуть рычаг")]
    [SerializeField] private float pullSpeed = 10f;

    [Tooltip("Скорость возврата рычага назад")]
    [SerializeField] private float returnSpeed = 140f;

    [Tooltip("Процент от maxAngle для срабатывания")]
    [Range(0.9f, 1f)]
    [SerializeField] private float triggerThreshold = 0.97f;

    [Header("Events")]
    public UnityEvent OnTriggered;

    private IXRSelectInteractor interactor;
    private bool triggered;
    private Coroutine returnRoutine;

    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(OnGrab);
        selectExited.AddListener(OnRelease);
    }

    protected override void OnDisable()
    {
        selectEntered.RemoveListener(OnGrab);
        selectExited.RemoveListener(OnRelease);
        base.OnDisable();
    }

    public override Transform GetAttachTransform(IXRInteractor interactor)
    {
        return interactorAttach != null ? interactorAttach : base.GetAttachTransform(interactor);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;
        triggered = false;

        if (returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;
        returnRoutine = StartCoroutine(ReturnToStart());
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected)
            UpdateLever();
    }

    private void UpdateLever()
    {
        Vector3 dir = interactor.GetAttachTransform(this).position - handle.position;
        dir = transform.InverseTransformDirection(dir);
        dir.x = 0f;

        float targetAngle = Mathf.Atan2(dir.z, dir.y) * Mathf.Rad2Deg;
        targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

        float currentAngle = GetCurrentAngle();

        float newAngle = Mathf.MoveTowards(
            currentAngle,
            targetAngle,
            pullSpeed * Time.deltaTime
        );

        SetAngle(newAngle);

        // OneShot активация
        if (!triggered && newAngle >= maxAngle * triggerThreshold)
        {
            triggered = true;
            OnTriggered.Invoke();
        }
    }

    private IEnumerator ReturnToStart()
    {
        float angle = GetCurrentAngle();

        while (angle > minAngle + 0.05f)
        {
            angle = Mathf.MoveTowards(
                angle,
                minAngle,
                returnSpeed * Time.deltaTime
            );

            SetAngle(angle);
            yield return null;
        }

        SetAngle(minAngle);
        returnRoutine = null;
    }

    private float GetCurrentAngle()
    {
        float angle = handle.localEulerAngles.x;
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    private void SetAngle(float angle)
    {
        handle.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }
}
