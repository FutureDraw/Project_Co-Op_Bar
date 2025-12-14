using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Content.Interaction
{
    public class XRLeverAdvancedSmooth : XRBaseInteractable
    {
        public enum LeverMode
        {
            MultiPosition,
            OneShot
        }

        [Header("References")]
        [SerializeField] Transform m_Handle;
        [SerializeField] Transform m_InteractorSnapTransform;

        [Header("Lever Mode")]
        [SerializeField] LeverMode m_Mode = LeverMode.MultiPosition;

        [Header("Angles")]
        [SerializeField] float m_MinAngle = 0f;
        [SerializeField] float m_MaxAngle = 60f;

        [Tooltip("Фиксированные углы (например 15,30,45,60)")]
        [SerializeField] float[] m_Positions = { 15f, 30f, 45f, 60f };

        [Header("Smoothing")]
        [SerializeField] float m_SmoothTime = 0.25f;

        [Header("Events")]
        public UnityEvent<int> OnPositionChanged;
        public UnityEvent<float> OnAngleChanged;
        public UnityEvent OnOneShotTriggered;

        IXRSelectInteractor m_Interactor;
        Coroutine m_SmoothRoutine;
        int m_CurrentIndex = -1;
        bool m_OneShotFired;

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(StartGrab);
            selectExited.AddListener(EndGrab);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(StartGrab);
            selectExited.RemoveListener(EndGrab);
            base.OnDisable();
        }

        void StartGrab(SelectEnterEventArgs args)
        {
            m_Interactor = args.interactorObject;
            StopSmoothing();
        }

        void EndGrab(SelectExitEventArgs args)
        {
            if (m_Mode == LeverMode.MultiPosition)
                SnapToNearestSmooth();

            if (m_Mode == LeverMode.OneShot)
                ReturnToStart();

            m_Interactor = null;
            m_OneShotFired = false;
        }

        public override Transform GetAttachTransform(IXRInteractor interactor)
        {
            return m_InteractorSnapTransform;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected)
                UpdateLever();
        }

        void UpdateLever()
        {
            Vector3 dir = m_Interactor.GetAttachTransform(this).position - m_Handle.position;
            dir = transform.InverseTransformDirection(dir);
            dir.x = 0f;

            float angle = Mathf.Atan2(dir.z, dir.y) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, m_MinAngle, m_MaxAngle);

            SetHandleAngle(angle);

            if (m_Mode == LeverMode.OneShot && !m_OneShotFired && angle >= m_MaxAngle - 1f)
            {
                m_OneShotFired = true;
                OnOneShotTriggered.Invoke();
                ReturnToStart();
            }
        }

        void SnapToNearestSmooth()
        {
            float current = GetCurrentAngle();
            float closest = float.MaxValue;
            int index = 0;

            for (int i = 0; i < m_Positions.Length; i++)
            {
                float d = Mathf.Abs(current - m_Positions[i]);
                if (d < closest)
                {
                    closest = d;
                    index = i;
                }
            }

            m_CurrentIndex = index;
            float targetAngle = m_Positions[index];

            SmoothSetAngle(targetAngle);

            OnPositionChanged.Invoke(index);
            OnAngleChanged.Invoke(targetAngle);
        }

        void ReturnToStart()
        {
            SmoothSetAngle(m_MinAngle);
        }

        void SmoothSetAngle(float target)
        {
            StopSmoothing();
            m_SmoothRoutine = StartCoroutine(SmoothRotateRoutine(target));
        }

        IEnumerator SmoothRotateRoutine(float target)
        {
            float start = GetCurrentAngle();
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / m_SmoothTime;
                float angle = Mathf.LerpAngle(start, target, t);
                SetHandleAngle(angle);
                yield return null;
            }

            SetHandleAngle(target);
            m_SmoothRoutine = null;
        }

        void StopSmoothing()
        {
            if (m_SmoothRoutine != null)
            {
                StopCoroutine(m_SmoothRoutine);
                m_SmoothRoutine = null;
            }
        }

        float GetCurrentAngle()
        {
            float angle = m_Handle.localEulerAngles.x;
            if (angle > 180f) angle -= 360f;
            return angle;
        }

        void SetHandleAngle(float angle)
        {
            if (m_Handle != null)
                m_Handle.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }
    }
}
