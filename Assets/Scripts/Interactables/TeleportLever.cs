using UnityEngine;

public class TeleportLever : MonoBehaviour, ITeleportSelector
{
    [SerializeField] private int selectedIndex;

    public int SelectedIndex => selectedIndex;

    public void SelectTarget(int index)
    {
        selectedIndex = index;
        Debug.Log("index " + index);
    }
}
