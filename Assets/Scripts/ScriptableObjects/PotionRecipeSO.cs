using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Alchemy/PotionRecipe")]
public class PotionRecipe : ScriptableObject
{
    public List<KitchenObjectSO> ingredients;
    public KitchenObjectSO result;

    public bool Matches(List<KitchenObjectSO> inputIngredients)
    {
        if (ingredients.Count != inputIngredients.Count)
            return false;

        var required = new List<KitchenObjectSO>(ingredients);
        foreach (var ing in inputIngredients)
        {
            if (!required.Remove(ing))
                return false;
        }
        return required.Count == 0;
    }
}