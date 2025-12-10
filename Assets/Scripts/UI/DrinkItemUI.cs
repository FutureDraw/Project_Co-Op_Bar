using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrinkItemUI : MonoBehaviour
{
    public TMP_Text drinkLabel;
    public TMP_Text countLabel;
    public Button button;

    private string drinkName;
    private CdReaderUI ui;

    public void SetData(string name, int count, CdReaderUI uiRef)
    {
        drinkName = name;
        ui = uiRef;

        if (drinkLabel != null) drinkLabel.text = name;
        if (countLabel != null) countLabel.text = "x" + count;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (ui != null)
            ui.ShowRecipe(drinkName);
    }
}
