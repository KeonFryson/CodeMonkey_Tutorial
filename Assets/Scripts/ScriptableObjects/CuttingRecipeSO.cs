using UnityEngine;

[CreateAssetMenu(fileName = "CuttingRecipe", menuName = "ScriptableObjects/CuttingRecipe")]
public class CuttingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax;
}
