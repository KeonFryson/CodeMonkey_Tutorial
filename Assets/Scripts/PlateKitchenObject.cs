using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {
    
    private List<KitchenObjectSO> kitchenObjectSOList;

    private void Awake() {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // Already contains this ingredient, cannot add it again
            return false;
        } else
        {
            // Add the ingredient to the plate
            kitchenObjectSOList.Add(kitchenObjectSO);
            return true;
        }
      
    }

}
