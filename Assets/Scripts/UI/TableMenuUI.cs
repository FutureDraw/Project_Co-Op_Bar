using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableMenuUI : MonoBehaviour
{
    public static TableMenuUI Instance;

    public List<Button> tableButtons;
    private Button previousSelectedButton;

    public GameObject nextPageBtn;
    public GameObject printBtn;

    private void Awake()
    {
        // Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        UpdateButtons();
    }

    // Выбор стола
    public void SelectTable(int tableId)
    {
        OrderManager.Instance.SelectTable(tableId);

        // Находим кнопку с этим tableId
        Button selectedButton = tableButtons.Find(b => b.GetComponent<TableButton>().tableId == tableId);

        if (selectedButton != null)
        {
            if (previousSelectedButton != null)
                previousSelectedButton.image.color = Color.white;

            selectedButton.image.color = Color.yellow;
            previousSelectedButton = selectedButton;
        }

        UpdateButtons();
    }

    void UpdateButtons()
    {
        bool canPrint = OrderManager.Instance.ReadyToPrint();
        printBtn.SetActive(canPrint);
        nextPageBtn.SetActive(!canPrint);
    }

    public void ResetTableSelection()
    {
        foreach (var button in tableButtons)
        {
            button.image.color = Color.white;
        }
        previousSelectedButton = null;
    }

    public void CloseMenu()
    {
        this.gameObject.SetActive(false);
        FindObjectOfType<MainMenuUI>().UpdatePrintButton();
    }
}
