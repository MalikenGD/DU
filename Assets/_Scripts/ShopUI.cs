using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : IUIObject
{
    private GameObject _uiGameObject;
    private Shop _shop;

    public ShopUI(Shop shop, GameObject uiGameObject)
    {
        _shop = shop;
        _uiGameObject = uiGameObject;
    }

    public GameObject GetUIGameObject()
    {
        return _uiGameObject;
    }
}
