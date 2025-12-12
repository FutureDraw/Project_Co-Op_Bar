using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Dash
{
    public class DashProvider : LocomotionProvider
    {
        [Header("Input")]
        [SerializeField]
        XRInputButtonReader m_DashInput = new XRInputButtonReader("Dash");

        [SerializeField]
        XRInputValueReader<Vector2> m_MoveInput = new XRInputValueReader<Vector2>("Move");

        [Header("Settings")]
        [SerializeField]
        float m_NormalSpeed = 1.5f;

        [SerializeField]
        float m_DashMultiplier = 5f;

        [SerializeField]
        float m_DashDuration = 0.2f;

        [SerializeField]
        float m_Cooldown = 1.0f;

        [SerializeField]
        AnimationCurve m_AccelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("References")]
        [SerializeField]
        Transform m_HeadTransform;

        public XROriginMovement transformation { get; set; } = new XROriginMovement();

        bool m_IsDashing;
        bool m_CooldownActive;
        float m_Timer;
        float m_CooldownTimer;

        void OnEnable()
        {
            m_DashInput.EnableDirectActionIfModeUsed();
            m_MoveInput.EnableDirectActionIfModeUsed();
        }

        void OnDisable()
        {
            m_DashInput.DisableDirectActionIfModeUsed();
            m_MoveInput.DisableDirectActionIfModeUsed();
        }

        void Update()
        {
            if (!isActiveAndEnabled)
                return;

            HandleCooldown();

            if (!m_IsDashing && !m_CooldownActive && m_DashInput.ReadWasPerformedThisFrame())
                StartDash();

            if (m_IsDashing)
                UpdateDash();
        }

        void HandleCooldown()
        {
            if (!m_CooldownActive)
                return;

            m_CooldownTimer += Time.deltaTime;
            if (m_CooldownTimer >= m_Cooldown)
                m_CooldownActive = false;
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
                m_CooldownActive = true;
                m_CooldownTimer = 0f;
                return;
            }

            // Кривая плавности
            float curve = m_AccelCurve.Evaluate(t);

            // Направление — движение или взгляд
            Vector2 move = m_MoveInput.ReadValue();
            Vector3 dashDir;

            if (move.sqrMagnitude > 0.0001f)
            {
                Vector3 f = m_HeadTransform.forward;
                Vector3 r = m_HeadTransform.right;

                f.y = 0;
                r.y = 0;

                f.Normalize();
                r.Normalize();

                dashDir = (f * move.y + r * move.x).normalized;
            }
            else
            {
                dashDir = m_HeadTransform.forward;
                dashDir.y = 0;
                dashDir.Normalize();
            }

            float speed = m_NormalSpeed * m_DashMultiplier * curve;
            Vector3 motion = dashDir * speed * Time.deltaTime;

            TryStartLocomotionImmediately();
            if (locomotionState != LocomotionState.Moving)
                return;

            transformation.motion = motion;
            TryQueueTransformation(transformation);
        }
    }
}
