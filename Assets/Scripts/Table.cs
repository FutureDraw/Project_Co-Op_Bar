using UnityEngine;

public class Table : MonoBehaviour
{
    public int tableID;
    public bool isOccupied;

    public Transform seatPoint;
    public Transform trashPoint;

    public void Occupy()
    {
        isOccupied = true;
    }

    public void Free()
    {
        isOccupied = false;
    }
}
