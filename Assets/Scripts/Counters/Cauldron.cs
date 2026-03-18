using System;
using UnityEngine;

public class Cauldron : BaseCounter
{
    public event Action OnCauldronCicked;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // There is no KitchenObject here 
            if (player.HasKitchenObject())
            {
                // Player is carrying something
               
            }
            else
            {
                // Player is not carrying anything

            }
        }
        else
        {
            // There is a KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something

            }
            else
            {
                // Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        OnCauldronCicked?.Invoke();
    }
}
