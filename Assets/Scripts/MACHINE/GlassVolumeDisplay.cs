using UnityEngine;
using TMPro;

[RequireComponent(typeof(GlassInventory))]
public class GlassVolumeDisplay : MonoBehaviour
{
    [Header("UI")]
    public TextMeshPro volumeText;

    [Header("Distance settings (meters)")]
    public float fullyVisibleDistance = 0.30f; // 30 см
    public float fadeStartDistance = 0.40f;    // 40 см
    public float disableDistance = 0.50f;      // 50 см

    [Header("Camera")]
    public Camera playerCamera;

    GlassInventory glassInventory;
    Color baseColor;

    void Awake()
    {
        glassInventory = GetComponent<GlassInventory>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        baseColor = volumeText.color;
        volumeText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerCamera == null || volumeText == null)
            return;

        float distance = Vector3.Distance(
            playerCamera.transform.position,
            volumeText.transform.position
        );

        // Полное отключение
        if (distance >= disableDistance)
        {
            if (volumeText.gameObject.activeSelf)
                volumeText.gameObject.SetActive(false);
            return;
        }

        // Включаем объект если подошли достаточно близко
        if (!volumeText.gameObject.activeSelf)
            volumeText.gameObject.SetActive(true);

        UpdateText();
        RotateToCamera();
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
        volumeText.color = c;
    }

    void RotateToCamera()
    {
        volumeText.transform.LookAt(playerCamera.transform);
        volumeText.transform.Rotate(0f, 180f, 0f);
    }

    void UpdateText()
    {
        float totalVolume = 0f;

        foreach (var drink in glassInventory.contents)
            totalVolume += drink.volumeMl;

        volumeText.text = $"{Mathf.RoundToInt(totalVolume)} мл";
    }
}
