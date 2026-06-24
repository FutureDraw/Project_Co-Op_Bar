using UnityEngine;
using TMPro;

public class UIVisibility : MonoBehaviour
{
    [Header("UI")]
    public Canvas UI;
    public CanvasGroup UIGroup;

    [Header("Objects that disappear")]
    public GameObject[] objectsToHide;

    [Header("Distance settings (meters)")]
    public float fullyVisibleDistance = 0.30f; // 30 см
    public float fadeStartDistance = 0.40f;    // 40 см
    public float disableDistance = 0.50f;      // 50 см

    [Header("Objects distance settings (meters)")]
    public float objectsEnableDistance = 1.0f;
    public float objectsDisableDistance = 1.0f;

    [Header("Camera")]
    public Camera playerCamera;

    Color baseColor;

    void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (UI != null)
            UI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerCamera == null || UI == null)
            return;

        float distance = Vector3.Distance(
            playerCamera.transform.position,
            UI.transform.position
        );

        if (distance >= disableDistance)
        {
            if (UI.gameObject.activeSelf)
            {
                UI.gameObject.SetActive(false);
                SetObjectsActive(false);
            }
            return;
        }

        if (!UI.gameObject.activeSelf)
        {
            UI.gameObject.SetActive(true);
            SetObjectsActive(true);
        }

        UpdateTransparency(distance);
        UpdateObjectsDistance();
    }

    void UpdateTransparency(float distance)
    {
        float alpha;

        if (distance <= fullyVisibleDistance)
        {
            alpha = 1f;
        }
        else if (distance <= fadeStartDistance)
        {
            alpha = Mathf.InverseLerp(fadeStartDistance, fullyVisibleDistance, distance);
        }
        else
        {
            alpha = 0f;
        }

        if (UIGroup != null)
        {
            UIGroup.alpha = alpha;
        }

        Debug.Log($"Distance: {distance:F2} | Alpha: {alpha:F2}");
    }

    void UpdateObjectsDistance()
    {
        if (objectsToHide == null || objectsToHide.Length == 0)
            return;

        foreach (GameObject obj in objectsToHide)
        {
            if (obj == null) continue;

            float d = Vector3.Distance(playerCamera.transform.position, obj.transform.position);

            bool shouldBeActive = d <= objectsEnableDistance;

            if (obj.activeSelf != shouldBeActive)
            {
                obj.SetActive(shouldBeActive);
            }
        }
    }

    private void SetObjectsActive(bool active)
    {
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }
}

