using System;
using UnityEngine;

public class CauldronVisual : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Cauldron cauldron;
    [SerializeField] private GameObject LiquidVisual;

    private void Start()
    {
        cauldron.OnStateChanged += Cauldron_OnStateChanged;
         
    }

    private void Cauldron_OnStateChanged(Cauldron.State state)
    {
        switch (state)
        {
            case Cauldron.State.Empty:
                LiquidVisual.SetActive(false);
                break;
            case Cauldron.State.Full:
                LiquidVisual.SetActive(true);
                break;
        }
    }

    
}
