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
        XRInputValueReader<Vector2> m_MoveInput = new XRInputValueReader<Vector2>("Move");

        [SerializeField]
        float m_NormalSpeed = 1.5f;

        [SerializeField]
        float m_DashMultiplier = 5f;

        [SerializeField]
        float m_DashDuration = 0.2f;

        [SerializeField]
        Transform m_HeadTransform;

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
            if (m_Timer >= m_DashDuration)
            {
                m_IsDashing = false;
                return;
            }

            // Считываем ввод стика перемещения
            Vector2 move = m_MoveInput.ReadValue();
            Vector3 dashDir;

            if (move.sqrMagnitude > 0.0001f)
            {
                // направление движения на основе головы
                Vector3 forward = m_HeadTransform.forward;
                Vector3 right = m_HeadTransform.right;

                forward.y = 0f;
                right.y = 0f;

                forward.Normalize();
                right.Normalize();

                dashDir = (forward * move.y + right * move.x).normalized;
            }
            else
            {
                // если игрок стоит — используем forward камеры
                dashDir = m_HeadTransform.forward;
                dashDir.y = 0f;
                dashDir.Normalize();
            }

            float speed = m_NormalSpeed * m_DashMultiplier;
            Vector3 motion = dashDir * speed * Time.deltaTime;

            TryStartLocomotionImmediately();
            if (locomotionState != LocomotionState.Moving)
                return;

            transformation.motion = motion;
            TryQueueTransformation(transformation);
        }

    }
}
