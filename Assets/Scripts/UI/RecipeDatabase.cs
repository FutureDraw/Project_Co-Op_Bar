using System.Collections.Generic;
using UnityEngine;

public class RecipeDatabase : MonoBehaviour
{
    public static RecipeDatabase Instance;

    [System.Serializable]
    public class DrinkRecipe
    {
        public string drinkName;
        [TextArea(3, 10)]
        public string recipeText;
    }

    public List<DrinkRecipe> recipes = new List<DrinkRecipe>();

    private void Awake()
    {
        Instance = this;
    }

    public string GetRecipe(string drinkName)
    {
        foreach (var r in recipes)
        {
            if (r.drinkName == drinkName)
                return r.recipeText;
        }
        return "Рецепт не найден.";
    }
}
