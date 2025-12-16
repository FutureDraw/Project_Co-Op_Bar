using UnityEngine;

public class XRLeverDebug : MonoBehaviour
{
    public void LogPositionIndex(int index)
    {
        Debug.Log($"[XRLever] Position Index: {index}");
    }

    public void LogAngle(float angle)
    {
        Debug.Log($"[XRLever] Angle: {angle}°");
    }

    public void LogOneShot()
    {
        Debug.Log("[XRLever] OneShot Triggered");
    }
}
