using System;
using UnityEngine;


public enum UIState
{
    Interactable = 0,
    Highlighted = 1,
    Selected = 2,
    GreyedOut = 3,
    Disabled = 4
}
public class UIBehaviour : MonoBehaviour
{
    protected object parentObjectWithDataToDisplay;
    protected UIState currentState;
    protected internal UIState previousState;

    public void SetParentObjectWithDataToDisplay(object parentObject)
    {
        parentObjectWithDataToDisplay = parentObject;
    }

    public void SetNewState(UIState newState)
    {
        currentState = newState;
    }

    public void StorePreviousState()
    {
        Debug.Log(currentState);
        previousState = currentState;
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case UIState.Interactable:
                UpdateWhenInteractable();
                break;
            case UIState.Highlighted:
                UpdateWhenHighlighted();
                break;
            case UIState.Selected:
                UpdateWhenSelected();
                break;
            case UIState.GreyedOut:
                UpdateWhenGreyedOut();
                break;
            case UIState.Disabled:
                UpdateWhenDisabled();
                break;
        }
    }

    protected virtual void UpdateWhenInteractable()
    {
        
    }

    protected virtual void UpdateWhenHighlighted()
    {
        
    }

    protected virtual void UpdateWhenSelected()
    {
        
    }
    
    protected virtual void UpdateWhenGreyedOut()
    {
        
    }
    
    protected virtual void UpdateWhenDisabled()
    {
        
    }
    
    
}