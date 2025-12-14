using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class XRLeverDualRowIndicators : MonoBehaviour
{
    public enum IndicatorMode
    {
        Green,
        Yellow,
        Red
    }

    [Header("Rows")]
    [SerializeField] Renderer[] m_GreenRow;
    [SerializeField] Renderer[] m_YellowRow;

    [Header("Colors")]
    [SerializeField] Color m_GreenColor = Color.green;
    [SerializeField] Color m_YellowColor = Color.yellow;
    [SerializeField] Color m_RedColor = Color.red;

    [Header("Initial State")]
    [SerializeField] IndicatorMode m_InitialMode = IndicatorMode.Green;
    [SerializeField] int m_InitialIndex = 0;

    [Header("Lever Sync")]
    [SerializeField] XRLeverAdvancedSmooth m_Lever;

    IndicatorMode m_Mode;
    int m_CurrentIndex;

    void Start()
    {
        m_Mode = m_InitialMode;
        m_CurrentIndex = m_InitialIndex;

        // 1️⃣ сначала индикаторы
        Refresh();

        // 2️⃣ потом рычаг
        if (m_Lever != null)
            m_Lever.SetInitialPositionByIndex(m_CurrentIndex);
    }


    // вызывается рычагом
    public void SetIndex(int index)
    {
        m_CurrentIndex = index;
        Refresh();
    }

    // вызывается кнопкой / логикой
    public void SetMode(int mode)
    {
        m_Mode = (IndicatorMode)Mathf.Clamp(mode, 0, 2);
        Refresh();
    }

    void Refresh()
    {
        // Зелёный ряд
        for (int i = 0; i < m_GreenRow.Length; i++)
        {
            bool active = (m_Mode == IndicatorMode.Green && i == m_CurrentIndex);
            ApplyColor(m_GreenRow[i], active ? m_GreenColor : m_RedColor);
        }

        // Жёлтый ряд
        for (int i = 0; i < m_YellowRow.Length; i++)
        {
            bool active = (m_Mode == IndicatorMode.Yellow && i == m_CurrentIndex);
            ApplyColor(m_YellowRow[i], active ? m_YellowColor : m_RedColor);
        }
    }

    void ApplyColor(Renderer r, Color color)
    {
        if (r == null) return;
        r.material.color = color;
    }
}
