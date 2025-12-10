using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    public Dictionary<string, int> drinks = new Dictionary<string, int>();
    public int selectedTable = -1;

    public GameObject CdTicketPrefab;
    public Transform CdTicketSpawnPoint;

    public GameObject mainPanel;
    public GameObject drinkMenu;
    public GameObject tableMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddDrink(string drinkName)
    {
        if (drinks.ContainsKey(drinkName))
            drinks[drinkName]++;
        else
            drinks[drinkName] = 1;
    }

    public int GetDrinkCount(string drinkName)
    {
        return drinks.ContainsKey(drinkName) ? drinks[drinkName] : 0;
    }

    public bool HasAnyDrinks() => drinks.Count > 0;
    public bool HasTable() => selectedTable != -1;

    public bool ReadyToPrint() => HasAnyDrinks() && HasTable();

    public void SelectTable(int table)
    {
        selectedTable = table;
    }

    public void PrintOrder()
    {
        Debug.Log($"Print: Table={selectedTable}, Drinks={drinks.Count}");

        if (CdTicketPrefab != null && CdTicketSpawnPoint != null)
            Instantiate(CdTicketPrefab, CdTicketSpawnPoint.position, CdTicketPrefab.transform.rotation);
        else
            Debug.LogError("CdTicketPrefab или CdTicketSpawnPoint не назначены!");

        drinks.Clear();
        selectedTable = -1;

        mainPanel.SetActive(true);
        drinkMenu.SetActive(false);
        tableMenu.SetActive(false);

        FindObjectOfType<MainMenuUI>().UpdatePrintButton();
    }
}
