using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Dash
{
    public class DashProvider : LocomotionProvider
    {
        [SerializeField]
        XRInputButtonReader m_DashInput = new XRInputButtonReader("Dash");

        [SerializeField]
        float m_NormalSpeed = 1.5f;

        [SerializeField]
        float m_DashMultiplier = 5f;

        [SerializeField]
        float m_DashDuration = 0.2f;

        [SerializeField]
        Transform m_HeadTransform;

        [SerializeField]
        float m_DashDistance = 1.2f;

        [SerializeField]
        XRInputValueReader<Vector2> m_MoveInput = new XRInputValueReader<Vector2>("Move");

        public XROriginMovement transformation { get; set; } = new XROriginMovement();

        bool m_IsDashing;
        float m_Timer;

        void OnEnable()
        {
            m_DashInput.EnableDirectActionIfModeUsed();
        }

        void OnDisable()
        {
            m_DashInput.DisableDirectActionIfModeUsed();
        }

        void Update()
        {
            if (!isActiveAndEnabled)
                return;

            if (!m_IsDashing && m_DashInput.ReadWasPerformedThisFrame())
                StartDash();

            if (m_IsDashing)
                UpdateDash();
        }

        void StartDash()
        {
            if (!m_HeadTransform)
                return;

            m_IsDashing = true;
            m_Timer = 0f;
        }

        void UpdateDash()
        {
            m_Timer += Time.deltaTime;
            float t = m_Timer / m_DashDuration;

            if (t >= 1f)
            {
                m_IsDashing = false;
                return;
            }

            Vector3 dashDir;
            Vector2 move = m_MoveInput.ReadValue();

            if (move.sqrMagnitude > 0.0001f)
            {
                Vector3 forward = m_HeadTransform.forward;
                Vector3 right = m_HeadTransform.right;
                forward.y = right.y = 0f;
                dashDir = (forward * move.y + right * move.x).normalized;
            }
            else
            {
                dashDir = m_HeadTransform.forward;
                dashDir.y = 0f;
                dashDir.Normalize();
            }

            float step = (m_DashDistance / m_DashDuration) * Time.deltaTime;
            Vector3 motion = dashDir * step;

            TryStartLocomotionImmediately();
            if (locomotionState != LocomotionState.Moving)
                return;

            transformation.motion = motion;
            TryQueueTransformation(transformation);
        }


    }
}
