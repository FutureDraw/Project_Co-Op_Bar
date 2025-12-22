using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OrderManager : NetworkBehaviour
{
    public static OrderManager Instance;

    // Заказ
    public Dictionary<string, int> drinks = new Dictionary<string, int>();
    public int selectedTable = -1;

    // CdTicket
    [SerializeField] private GameObject CdTicketPrefab;
    [SerializeField] private Transform CdTicketSpawnPoint;

    // UI
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject drinkMenu;
    [SerializeField] private GameObject tableMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ------------------ ЛОГИКА ЗАКАЗА ------------------

    public void AddDrink(string drinkName)
    {
        if (drinks.ContainsKey(drinkName))
            drinks[drinkName]++;
        else
            drinks.Add(drinkName, 1);
    }

    public bool HasAnyDrinks() => drinks.Count > 0;
    public bool HasTable() => selectedTable != -1;
    public bool ReadyToPrint() => HasAnyDrinks() && HasTable();

    public void SelectTable(int table)
    {
        selectedTable = table;
    }

    public int GetDrinkCount(string drinkName) { return drinks.ContainsKey(drinkName) ? drinks[drinkName] : 0; }

    // ------------------ ПЕЧАТЬ ------------------

    public void PrintOrder()
    {
        if (!ReadyToPrint()) return;

        PrintOrderServerRpc(selectedTable);

        // очистка локального UI
        drinks.Clear();
        selectedTable = -1;

        if (TableMenuUI.Instance != null)
            TableMenuUI.Instance.ResetTableSelection();

        mainPanel.SetActive(true);
        drinkMenu.SetActive(false);
        tableMenu.SetActive(false);

        FindObjectOfType<MainMenuUI>().UpdatePrintButton();
    }

    // ------------------ SERVER RPC ------------------

    [ServerRpc(RequireOwnership = false)]
    private void PrintOrderServerRpc(int table)
    {
        if (CdTicketPrefab == null)
        {
            Debug.LogError("CdTicketPrefab == null (на сервере)");
            return;
        }

        if (CdTicketSpawnPoint == null)
        {
            Debug.LogError("CdTicketSpawnPoint == null (на сервере)");
            return;
        }

        GameObject cd = Instantiate(
            CdTicketPrefab,
            CdTicketSpawnPoint.position,
            CdTicketSpawnPoint.rotation
        );

        NetworkObject netObj = cd.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("На CdTicketPrefab нет NetworkObject!");
            Destroy(cd);
            return;
        }

        netObj.Spawn();

        CdTicket ticket = netObj.GetComponent<CdTicket>();
        if (ticket != null)
        {
            ticket.tableNumber = selectedTable;
            ticket.drinks = new Dictionary<string, int>(drinks);
            ticket.isUsed = false;
        }
    }
}
