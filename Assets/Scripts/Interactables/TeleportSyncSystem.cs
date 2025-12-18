using UnityEngine;

public class TeleportSyncSystem : MonoBehaviour
{
    [SerializeField] private TeleportLever leverA;
    [SerializeField] private TeleportLever leverB;

    public bool IsTeleportAllowed()
    {
        return leverA.SelectedIndex == leverB.SelectedIndex;
    }

    public int GetSyncedIndex()
    {
        return leverA.SelectedIndex;
    }
}
