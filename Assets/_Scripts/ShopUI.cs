using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : UIBehaviour
{
    private GameObject _uiGameObject;
    private Shop _shop;
    private List<Card> _cardsInHand;
    private List<CardUI> _cardUIs = new List<CardUI>();

    private void Start()
    {
        _shop = parentObjectWithDataToDisplay as Shop;
        
        InitializeCardUI();
    }

    private void InitializeCardUI()
    {
        _cardsInHand = _shop.GetCardsInHand();

        foreach (Card card in _cardsInHand)
        {
            CardUI cardUI = card.CreateCardUI();
            _cardUIs.Add(cardUI);
        }
    }

    public GameObject GetUIGameObject()
    {
        return _uiGameObject;
    }
    
}
