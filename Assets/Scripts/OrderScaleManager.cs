using UnityEngine;

public class OrderScaleManager : MonoBehaviour
{
    public static OrderScaleManager Instance;
    public int orderScale = 100;

    void Awake()
    {
        Instance = this;
    }

    public void AddPoints(int value)
    {
        orderScale = Mathf.Clamp(orderScale + value, 0, 100);
        Debug.Log("Order Scale: " + orderScale);
    }
}
