using UnityEngine;
using TMPro;

public class CdReaderUI : MonoBehaviour
{
    public TMP_Text tableText;
    public TMP_Text recipeText;

    public Transform drinkListParent;    
    public GameObject drinkItemPrefab; 

    private CdReaderMachine machineRef;

    public void ShowTicketData(CdTicket ticket)
    {
        if (ticket == null) return;

        gameObject.SetActive(true);

        tableText.text = "Стол: " + ticket.tableNumber;

        foreach (Transform ch in drinkListParent)
            Destroy(ch.gameObject);

        foreach (var kv in ticket.drinks)
        {
            GameObject item = Instantiate(drinkItemPrefab, drinkListParent);
            DrinkItemUI di = item.GetComponent<DrinkItemUI>();
            di.SetData(kv.Key, kv.Value, this); // передаем ссылку на UI
        }

        recipeText.text = "";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetMachineRef(CdReaderMachine m)
    {
        machineRef = m;
    }

    public void ClearData()
    {
        foreach (Transform ch in drinkListParent)
            Destroy(ch.gameObject);

        if (recipeText != null)
            recipeText.text = "";

        if (tableText != null)
            tableText.text = "";
    }

    public void OnEjectButtonPressed()
    {
        if (machineRef == null) return;

        CdTicket ejected = machineRef.Eject();

        ClearData();
    }

    public void ShowRecipe(string drinkName)
    {
        if (RecipeDatabase.Instance == null)
        {
            recipeText.text = $"Рецепт: {drinkName}";
            return;
        }

        string recipe = RecipeDatabase.Instance.GetRecipe(drinkName);
        recipeText.text = $"<b>{drinkName}</b>\n\n{recipe}";
    }
}
