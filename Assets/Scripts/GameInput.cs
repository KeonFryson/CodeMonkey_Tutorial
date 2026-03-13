using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    private InputSystem_Actions inputActions;
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Interact.performed += Interact_performed;
        inputActions.Player.InteractAlternate.performed += InteractAlternate_performed; ;
        inputActions.Enable();
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
       OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }


    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        inputVector.Normalize();
        return inputVector;
    }
    
}
