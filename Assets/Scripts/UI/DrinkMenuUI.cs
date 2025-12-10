using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrinkMenuUI : MonoBehaviour
{
    public GameObject nextPageBtn;
    public GameObject printBtn;

    [System.Serializable]
    public class DrinkUI
    {
        public string drinkName;
        public TMP_Text countText;
    }

    public DrinkUI[] drinksUI;

    private void OnEnable()
    {
        UpdateAllCounters();
        UpdateButtons();
    }

    public void AddDrink(string drinkName)
    {
        OrderManager.Instance.AddDrink(drinkName);
        UpdateDrinkCounter(drinkName);
        UpdateButtons();
    }

    public void RemoveDrink(string drinkName)
    {
        if (OrderManager.Instance.drinks.ContainsKey(drinkName))
        {
            OrderManager.Instance.drinks[drinkName]--;
            if (OrderManager.Instance.drinks[drinkName] <= 0)
                OrderManager.Instance.drinks.Remove(drinkName);
        }

        UpdateDrinkCounter(drinkName);
        UpdateButtons();
    }

    void UpdateDrinkCounter(string drinkName)
    {
        foreach (var d in drinksUI)
        {
            if (d.drinkName == drinkName)
            {
                int count = OrderManager.Instance.GetDrinkCount(drinkName);
                d.countText.text = count.ToString();
            }
        }
    }

    void UpdateAllCounters()
    {
        foreach (var d in drinksUI)
            UpdateDrinkCounter(d.drinkName);
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
