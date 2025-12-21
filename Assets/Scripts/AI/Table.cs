using UnityEngine;

public class Table : MonoBehaviour
{
    public int tableID;
    public bool isOccupied;

    public Transform seatPoint;
    public Transform trashPoint;

    public GameObject trashPrefab;

    // Флаг, есть ли основной мусор на столе
    public bool hasMainTrash = false;

    public void Occupy()
    {
        isOccupied = true;
    }

    public void Free()
    {
        isOccupied = false;
    }

    // Проверка свободного стола
    public bool IsFree()
    {
        return !isOccupied && !hasMainTrash;
    }

    // Спавн основного мусора на столе (тот, который блокирует стол)
    public void SpawnMainTrash()
    {
        if (!hasMainTrash)
        {
            hasMainTrash = true;
            Occupy(); // стол становится занятым

            GameObject t = Instantiate(trashPrefab, trashPoint.position, Quaternion.identity);
            Trash trash = t.GetComponent<Trash>();
            trash.table = this;
            trash.blocksTable = true; // основной мусор

            Debug.Log("Spawned main trash on table " + tableID);
        }
    }

    // Спавн дополнительного мусора вокруг (не блокирует стол)
    public void SpawnExtraTrash(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * 1.2f;
            pos.y = transform.position.y;

            GameObject t = Instantiate(trashPrefab, pos, Quaternion.identity);
            Trash trash = t.GetComponent<Trash>();
            trash.blocksTable = false; // дополнительный мусор
        }
    }

    // Вызывается только когда основной мусор убран с самого столика
    public void ClearMainTrash()
    {
        hasMainTrash = false;
        Free();

        // Освобождаем стол только если за ним нет клиента
        if (!isOccupied)
        {
            Free();
            Debug.Log("Table " + tableID + " is now free");
        }
    }
}
