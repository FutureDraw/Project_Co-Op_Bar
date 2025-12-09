using UnityEngine;

public class TableMenuUI : MonoBehaviour
{
    public GameObject nextPageBtn;
    public GameObject printBtn;

    private void OnEnable()
    {
        UpdateButtons();
    }

    public void SelectTable(int tableId)
    {
        OrderManager.Instance.SelectTable(tableId);
        UpdateButtons();
    }

    void UpdateButtons()
    {
        bool canPrint = OrderManager.Instance.ReadyToPrint();

        printBtn.SetActive(canPrint);
        nextPageBtn.SetActive(!canPrint);
    }


    public void CloseMenu()
    {
        this.gameObject.SetActive(false);

        FindObjectOfType<MainMenuUI>().UpdatePrintButton();
    }

}
