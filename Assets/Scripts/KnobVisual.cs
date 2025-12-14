using System;
using TMPro;
using UnityEngine;

public class XRLeverTextDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text m_Text;

    public void ShowIndex(int index)
    {
        m_Text.text = $"{index} мл";
    }

    public void ShowAngle(float angle)
    {
        m_Text.text = $"{(float)System.Math.Round(angle*100)} мл";
    }
}
