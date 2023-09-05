using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class UIManager
{
    private List<GameObject> _uiGameObjects = new List<GameObject>();
    private List<UIBehaviour> _uiBehaviours = new List<UIBehaviour>();
    
    private void ToggleAllUIObjects()
    {
        foreach (var uiObject in _uiGameObjects)
        {
            bool uiObjectActiveStatus = uiObject.activeSelf;
            uiObject.SetActive(!uiObjectActiveStatus);
        }
    }


    public void StoreUIGameObjects(GameObject uiReferenceToStore)
    {
        _uiGameObjects.Add(uiReferenceToStore);
    }

    public UIBehaviour BuildUI(GameObject uiPrefab, object builder)
    {
        GameObject uiGameObject = GameObject.Instantiate(uiPrefab, World.Instance.GetParentCanvas().transform, true);
        UIBehaviour uiBehaviour = uiGameObject.GetComponent<UIBehaviour>();

        if (uiBehaviour == null)
        {
            Debug.LogError("Built UI Object that has no UIBehaviour");
            return null;
        }


        uiBehaviour.SetParentObjectWithDataToDisplay(builder);

        StoreUIGameObjects(uiGameObject);
        return uiGameObject.GetComponent<UIBehaviour>();
    }
}
