using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField]private float moveSpeed = 5f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

  
    private bool isWalking = false;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance in the scene!");
           Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction; ;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();

    }


    public bool GetIsWalking()
    {
        return isWalking;
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = gameInput.GetMovementVector();
        Vector3 targetDirection = new Vector3(inputVector.x, 0, inputVector.y);

        if (targetDirection != Vector3.zero)
        {
            lastInteractDir = targetDirection;
        }

        float interactionDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactionDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // Has ClearCounter
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVector();
       

        Vector3 targetDirection = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, targetDirection, moveDistance);

        if (!canMove)
        {
            // Cannot Move in the target direction, try to move only in the X direction

            Vector3 targetDirectionX = new Vector3(targetDirection.x, 0, 0).normalized;
            canMove = targetDirection.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, targetDirectionX, moveDistance);
            if (canMove)
            {
                // Can move only in the X direction
                targetDirection = targetDirectionX;
            }
            else
            {
                // Cannot move in the X direction, try to move only in the Z direction
                Vector3 targetDirectionZ = new Vector3(0, 0, targetDirection.z).normalized;
                canMove = targetDirection.z != 0 &&!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, targetDirectionZ, moveDistance);
                if (canMove)
                {
                    // Can move only in the Z direction
                    targetDirection = targetDirectionZ;
                }
                else
                {
                    // Cannot move in either direction
                }
            }
        }

        if (canMove)
        {
            transform.position += targetDirection * Time.deltaTime * moveSpeed;
        }

        inputVector.Normalize();

        float roatationSpeed = 10f;
        transform.LookAt(transform.position + Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * roatationSpeed));


        isWalking = targetDirection != Vector3.zero;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
