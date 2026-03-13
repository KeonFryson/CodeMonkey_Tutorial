using System;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event Action OnPlateSpawned;
    public event Action OnPlateDestroyed;

    [SerializeField] private KitchenObjectSO platekitchenObjectSO;
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int plateSpawnAmount;
    private int plateSpawnAmountMax = 4;

    void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;
            if(plateSpawnAmount < plateSpawnAmountMax)
            {
                plateSpawnAmount++;
                OnPlateSpawned?.Invoke();
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            // Player is not carrying anything, try to give them a plate
            if (plateSpawnAmount > 0)
            {
                // There is at least one plate available, give it to the player
                plateSpawnAmount--;
               
                KitchenObject.SpawnKitchenObject(platekitchenObjectSO, player);
                OnPlateDestroyed?.Invoke();
            }
        }
    }
}
