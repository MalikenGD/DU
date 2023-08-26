using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public static class UIObjectFactory
{
    public static IUIObject Build(int selection, object builder)
    {
        GameObject uiGameObject = null;
        switch (selection)
        {
            case 0:
                uiGameObject = GameObject.FindGameObjectWithTag("ShopUI");
                ShopUI shopUI = new ShopUI(builder as Shop, uiGameObject);
                return shopUI;

            case 1:
                List<GameObject> singularCardUIGameObjects = new List<GameObject>();
                singularCardUIGameObjects.AddRange(GameObject.FindGameObjectsWithTag("CardUISingle"));
                foreach (GameObject cardUIGameObject in singularCardUIGameObjects)
                {
                    if (cardUIGameObject.GetComponentInChildren<Image>().sprite == null)
                    {
                        uiGameObject = cardUIGameObject;
                        break;
                    }
                }

                if (uiGameObject != null)
                {
                    CardUI cardUI = new CardUI(builder as Card, uiGameObject);
                    return cardUI;
                }

                Debug.LogError("CardUIGameObject unavailable, cardUI not built.");
                return null;

            case 2:
                uiGameObject = GameObject.FindGameObjectWithTag("GridUI");
                GridUI gridUI = new GridUI(builder as Grid, uiGameObject);
                return gridUI;
                
                default:
                    Debug.LogError("UIObject not built, selection outside of scope");
                    return null;
        }
    }
}
