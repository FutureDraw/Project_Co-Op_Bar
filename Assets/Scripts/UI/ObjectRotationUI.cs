using UnityEngine;

public class ObjectRotationUI : MonoBehaviour
{
    [Header("Настройки вращения")]
    [Tooltip("Скорость вращения в градусах в секунду")]
    public float rotationSpeed = 60f;

    [Tooltip("Вокруг какой оси крутить (Y - по вертикали)")]
    public Vector3 rotationAxis = Vector3.up;

    [Tooltip("Вращать автоматически?")]
    public bool autoRotate = true;

    [Tooltip("Можно ли вращать мышкой?")]
    public bool mouseDragRotate = false;

    private void Update()
    {
        // Автоматическое вращение
        if (autoRotate)
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }

        // Вращение мышкой (зажатая левая кнопка)
        if (mouseDragRotate && Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, -mouseX * 150f * Time.deltaTime, Space.World);
        }
    }

    // Метод для внешнего управления (например, из UI)
    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }

    public void ToggleAutoRotate()
    {
        autoRotate = !autoRotate;
    }
}