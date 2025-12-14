using UnityEngine;

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

    IndicatorMode m_Mode;
    int m_CurrentIndex = -1;

    void Awake()
    {
        ResetAll();
    }

    // вызывается рычагом
    public void SetIndex(int index)
    {
        m_CurrentIndex = index;
        Refresh();
    }

    void Refresh()
    {
        UpdateRow(
            m_GreenRow,
            m_Mode == IndicatorMode.Green ? m_CurrentIndex : -1,
            m_GreenColor
        );

        UpdateRow(
            m_YellowRow,
            m_Mode == IndicatorMode.Yellow ? m_CurrentIndex : -1,
            m_YellowColor
        );
    }

    void UpdateRow(Renderer[] row, int activeIndex, Color activeColor)
    {
        for (int i = 0; i < row.Length; i++)
        {
            bool isActive = (i == activeIndex);
            ApplyColor(row[i], isActive ? activeColor : m_RedColor);
        }
    }

    void ResetAll()
    {
        UpdateRow(m_GreenRow, 1, m_GreenColor);
        UpdateRow(m_YellowRow, 1, m_YellowColor);
    }

    void ApplyColor(Renderer r, Color color)
    {
        if (r == null) return;
        r.material.color = color;
    }
}
