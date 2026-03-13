using System;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject fryingVisualGameObject;
    [SerializeField] private GameObject partiaclGameObject;
 
    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
       bool showFryingVisual = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        fryingVisualGameObject.SetActive(showFryingVisual);
        partiaclGameObject.SetActive(showFryingVisual);
    }
}
