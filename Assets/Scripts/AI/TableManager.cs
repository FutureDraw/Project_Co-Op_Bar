using UnityEngine;
using System.Collections.Generic;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance;
    public List<Table> tables;

    void Awake()
    {
        Instance = this;
    }

    public Table GetFreeTable()
    {
        foreach (var table in tables)
        {
            if (table.IsFree())
                return table;
        }
        return null;
    }

}
