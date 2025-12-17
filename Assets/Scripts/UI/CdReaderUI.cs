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

        ClearDrinkList();

        foreach (var kv in ticket.drinks)
        {
            GameObject item = Instantiate(drinkItemPrefab, drinkListParent);
            DrinkItemUI di = item.GetComponent<DrinkItemUI>();
            di.SetData(kv.Key, kv.Value, this);
        }

        recipeText.text = "";
    }

    public void ClearData()
    {
        ClearDrinkList();
        if (tableText != null) tableText.text = "";
        if (recipeText != null) recipeText.text = "";
    }

    private void ClearDrinkList()
    {
        foreach (Transform ch in drinkListParent)
            Destroy(ch.gameObject);
    }

    public void OnEjectButtonPressed()
    {
        if (machineRef == null)
        {
            Debug.LogWarning("CdReaderUI: machineRef is NULL");
            return;
        }

        machineRef.Eject();
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

    public void SetMachineRef(CdReaderMachine m)
    {
        machineRef = m;
    }
}
