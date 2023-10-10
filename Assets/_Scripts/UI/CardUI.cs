using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardUI : UIBehaviour
{

    private Card _card;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI goldCostText;
    [SerializeField] private TextMeshProUGUI unitNameText;
    private UnitDataSO _currentUnitDataToDisplay;
    
    
    private void Start()
    {
        gameObject.transform.parent = GameObject.FindWithTag("CardUI").transform;

        _card = parentObjectWithDataToDisplay as Card;

        if (_card == null) Debug.LogError("When built, CardUI has no reference to Card");
        _currentUnitDataToDisplay = _card.GetUnitData();
        _card.OnUnitDataUpdated += UpdateUI;
        //TODO: Handle card/cardUI deletion and unsubscribe from Card event?
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        _currentUnitDataToDisplay = _card.GetUnitData();

        if (backgroundImage != null)
        {
            backgroundImage.sprite = _currentUnitDataToDisplay.GetUnitSpriteForShopBackground();
        }
        
        if (goldCostText.text != null)
        {
            goldCostText.text = _currentUnitDataToDisplay.GetUnitGoldCost().ToString();
        }
        
        if (unitNameText.text != null)
        {
            unitNameText.text = _currentUnitDataToDisplay.GetUnitName();
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    //public void SetCardSelected() => _card.SetCardSelected();

    public void HandleClickLogic()
    {
        //CheckBalance
        //if canAfford()
        SetCardSelected();
    }

    private void SetCardSelected()
    {
        _card.SetCardSelected();
    }
}
