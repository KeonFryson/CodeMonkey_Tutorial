using System;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }


    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private State state;
    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;


    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        if(HasKitchenObject())
        {
             switch(state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    Frying();
                    break;
                case State.Fried:
                    Burning();
                    break;
                case State.Burned:
                    break;
            }
             
        }
    }


    

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // There is no KitchenObject here 
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // Player is carrying something that can be Fried
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Frying;
                    fryingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                     });
                }
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
                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {

            return null;
        }
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }
    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }

    private void Frying()
    {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
        });

        fryingTimer += Time.deltaTime;
        if (fryingTimer > fryingRecipeSO.fryingTimerMax)
        {
            // Fried
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
            Debug.Log("Fried! " + fryingRecipeSO.output.name);

           
            state = State.Fried;
            burningTimer = 0f;
            burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
            {
                state = state
            });
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
            });
        }
    }

    private void Burning()
    {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
        });

        burningTimer += Time.deltaTime;
        if (burningTimer > burningRecipeSO.burningTimerMax)
        {
            // Burned
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
            Debug.Log("Burned! " + burningRecipeSO.output.name);
            state = State.Burned;

            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
            {
                state = state
            });

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });

        }
    }
}
