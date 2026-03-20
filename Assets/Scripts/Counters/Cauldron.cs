using System;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : BaseCounter
{
    public enum State
    {
        Empty,
        Full,
    }

    public event Action<State> OnStateChanged;

    private const int MaxIngredients = 3;
    private List<KitchenObjectSO> currentIngredients = new List<KitchenObjectSO>();

    [SerializeField] private List<PotionRecipe> potionRecipes;
    [SerializeField] private KitchenObjectSO emptyBottleSO; // Assign in Inspector
    [SerializeField] private KitchenObjectSO WaterBucketSO; // Assign in Inspector
    [SerializeField] private KitchenObjectSO EmptyBucketSO; // Assign in Inspector
    private State state;

    private void Start()
    {
       state = State.Empty;
       OnStateChanged?.Invoke(state);
    }


    public override void Interact(Player player)
    {

        if (state == State.Full)
        {

            if (player.HasKitchenObject())
            {
                KitchenObject playerObject = player.GetKitchenObject();
                if (playerObject.GetKitchenObjectSO() == emptyBottleSO)
                {
                    // Try to finish potion if there are ingredients
                    if (currentIngredients.Count > 0)
                    {
                        Debug.Log("[Cauldron] Player is holding an empty bottle. Attempting to finish potion...");
                        PotionRecipe matchedRecipe = GetMatchingRecipe();
                        if (matchedRecipe != null)
                        {
                            Debug.Log($"[Cauldron] Potion created: {matchedRecipe.result.name}");
                            playerObject.DestroySelf();
                            KitchenObject.SpawnKitchenObject(matchedRecipe.result, player);

                            ClearCauldron();
                        }
                        else
                        {
                            Debug.Log("[Cauldron] No valid recipe found for current ingredients. Potion not created.");
                            ClearCauldron();
                        }
                    }
                    else
                    {
                        Debug.Log("[Cauldron] No ingredients in cauldron. Cannot finish potion.");
                    }
                }
                else
                {
                    // Only add ingredient if not empty bottle and not full
                    if (currentIngredients.Count < MaxIngredients)
                    {
                        Debug.Log($"[Cauldron] Adding ingredient: {playerObject.GetKitchenObjectSO().name}");
                        AddIngredient(playerObject.GetKitchenObjectSO());
                        playerObject.DestroySelf();
                    }
                    else
                    {
                        Debug.Log("[Cauldron] Cannot add more ingredients. Cauldron is full.");
                    }
                }
            }
            else
            {
                Debug.Log("[Cauldron] Player tried to interact with cauldron with empty hands. Nothing happens.");
                KitchenObjectSO lastIngredient = currentIngredients[currentIngredients.Count - 1];
                RemoveIngredient(lastIngredient);
                KitchenObject.SpawnKitchenObject(lastIngredient, player);

            }
        }
        else { 

            if (player.HasKitchenObject())
            {
                KitchenObject playerObject = player.GetKitchenObject();

                if (playerObject.GetKitchenObjectSO() == WaterBucketSO)
                {
                    Debug.Log("[Cauldron] Player added water to the cauldron. Cauldron is now full.");
                    state = State.Full;
                    OnStateChanged?.Invoke(state);
                    playerObject.DestroySelf();
                    KitchenObject.SpawnKitchenObject(EmptyBucketSO, player);
                }
                else
                {
                    Debug.Log("[Cauldron] Player is holding an invalid object. Only water can be added to the cauldron.");
                }
            }
            else
            {
                Debug.Log("[Cauldron] Player tried to interact with empty cauldron with empty hands. Nothing happens.");
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        Debug.Log("[Cauldron] Alternate interaction triggered.");
      
    }

    private void AddIngredient(KitchenObjectSO ingredient)
    {
        currentIngredients.Add(ingredient);
        Debug.Log($"[Cauldron] Current ingredients count: {currentIngredients.Count}");
    }

    private void RemoveIngredient(KitchenObjectSO ingredient)
    {
        currentIngredients.Remove(ingredient);
        Debug.Log($"[Cauldron] Removed ingredient: {ingredient.name}. Current ingredients count: {currentIngredients.Count}");
    }

    private PotionRecipe GetMatchingRecipe()
    {
        foreach (var recipe in potionRecipes)
        {
            if (recipe.Matches(currentIngredients))
            {
                Debug.Log($"[Cauldron] Matching recipe found: {recipe.result.name}");
                return recipe;
            }
        }
        Debug.Log("[Cauldron] No matching recipe found.");
        return null;
    }

    private void ClearCauldron()
    {
        Debug.Log("[Cauldron] Clearing cauldron ingredients.");
        state = State.Empty;
        OnStateChanged?.Invoke(state);
        currentIngredients.Clear();
    }
}