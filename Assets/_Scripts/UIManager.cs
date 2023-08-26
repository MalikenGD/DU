using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private List<IUIObject> _uiObjects = new List<IUIObject>();

    

    public void BuildUI(int selection, object builder)
    {
        _uiObjects.Add(UIObjectFactory.Build(selection,builder));
    }

    //TODO: Toggle when game state changes from buy to combat, and vice versa
    //This feels wrong, because I may want more granular control. Right now it's just a blanket on/off. 
    //If i'm buying a unit from the shop and want that card to disappear, will need a more granular method.
    private void ToggleUIObjects()
    {
        foreach (var uiObject in _uiObjects)
        {
            GameObject uiGameObject = uiObject.GetUIGameObject();
            uiGameObject.SetActive(!uiGameObject.activeSelf);
        }
    }


}
