using UnityEngine;
using TMPro;

public class UIVisibility : MonoBehaviour
{
    [Header("UI")]
    public Canvas UI;
    public CanvasGroup UIGroup;

    [Header("Distance settings (meters)")]
    public float fullyVisibleDistance = 0.30f; // 30 см
    public float fadeStartDistance = 0.40f;    // 40 см
    public float disableDistance = 0.50f;      // 50 см

    [Header("Camera")]
    public Camera playerCamera;

    Color baseColor;

    void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

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

        // Полное отключение
        if (distance >= disableDistance)
        {
            if (UI.gameObject.activeSelf)
                UI.gameObject.SetActive(false);
            return;
        }

        // Включаем объект если подошли достаточно близко
        if (!UI.gameObject.activeSelf)
            UI.gameObject.SetActive(true);

        UpdateTransparency(distance);
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
            // плавная интерполяция от 30см до 40см
            alpha = Mathf.InverseLerp(fadeStartDistance, fullyVisibleDistance, distance);
        }
        else
        {
            alpha = 0f;
        }

        Color c = baseColor;
        c.a = alpha;
        UIGroup.alpha = alpha;
    }

}
