using UnityEngine;

public class Trash : MonoBehaviour
{
    public Table table;
    public bool blocksTable; // true дл€ основного мусора на столе

    private void OnDestroy()
    {
        // если удал€етс€ основной мусор Ч освобождаем стол
        if (blocksTable && table != null)
        {
            table.ClearMainTrash();
        }
    }

    // метод, который вызывает официант
    public void Clean()
    {
        Destroy(gameObject); // OnDestroy вызовет ClearMainTrash
    }
}
