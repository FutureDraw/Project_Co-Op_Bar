using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject drinkMenu;
    public GameObject tableMenu;

    public GameObject printBtn;

    private void OnEnable()
    {
        UpdatePrintButton();
    }

    public void OpenDrinks()
    {
        drinkMenu.SetActive(true);
        tableMenu.SetActive(false);
    }

    public void OpenTables()
    {
        tableMenu.SetActive(true);
        drinkMenu.SetActive(false);
    }

    public void Print()
    {
        OrderManager.Instance.PrintOrder();
    }

    public void UpdatePrintButton()
    {
        bool canPrint = OrderManager.Instance.ReadyToPrint();
        printBtn.SetActive(canPrint);
    }
}
