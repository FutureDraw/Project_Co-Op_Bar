using UnityEngine;
using System.Collections;

public class LeverDrivenMover : MonoBehaviour
{
    [Header("Target Points (same count as lever positions)")]
    [SerializeField] Transform[] m_TargetPoints;

    [Header("Movement")]
    [SerializeField] float m_MoveTime = 0.4f;

    Coroutine m_MoveRoutine;

    public void MoveToIndex(int index)
    {
        if (m_TargetPoints == null || m_TargetPoints.Length == 0)
            return;

        index = Mathf.Clamp(index, 0, m_TargetPoints.Length - 1);

        if (m_MoveRoutine != null)
            StopCoroutine(m_MoveRoutine);

        m_MoveRoutine = StartCoroutine(MoveRoutine(m_TargetPoints[index]));
    }

    IEnumerator MoveRoutine(Transform target)
    {
        Vector3 startPos = transform.position;

        Vector3 endPos = target.position;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / m_MoveTime;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        m_MoveRoutine = null;
    }
}
