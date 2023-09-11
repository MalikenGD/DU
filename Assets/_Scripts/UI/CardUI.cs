using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : UIBehaviour
{
    private Card _card;
    private Image _backgroundImage;
    private TextMeshProUGUI _goldCostText;
    private TextMeshProUGUI _unitNameText;
    private UnitDataSO _currentUnitDataToDisplay;
    
    
    private void Start()
    {
        gameObject.transform.parent = GameObject.FindWithTag("CardUI").transform;

        _card = parentObjectWithDataToDisplay as Card;

        if (_card == null) Debug.LogError("When built, CardUI has no reference to Card");
        _currentUnitDataToDisplay = _card.GetUnitData();
        _card.OnUnitDataUpdated += UpdateUI;
        //TODO: Handle card/cardUI deletion and unsubscribe from Card event?

        
        InitializeCardUISettings();
        UpdateUI();
    }

    private void InitializeCardUISettings()
    {
        _backgroundImage = GetComponentInChildren<Image>();
        _goldCostText = gameObject.transform.Find("GoldText").GetComponent<TextMeshProUGUI>();
        _unitNameText = gameObject.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
    }

    public GameObject GetUIGameObject()
    {
        return gameObject;
    }

    private void UpdateUI()
    {
        _currentUnitDataToDisplay = _card.GetUnitData();
        _backgroundImage.sprite = _currentUnitDataToDisplay.GetUnitSpriteForShopBackground();
        _goldCostText.text = _currentUnitDataToDisplay.GetUnitGoldCost().ToString();
        _unitNameText.text = _currentUnitDataToDisplay.GetUnitName();
    }

    private void Update()
    {
        UpdateUI();
    }

    //public void SetCardSelected() => _card.SetCardSelected();

    public void SetCardSelected()
    {
        //CheckBalance
        //if canAfford()
        _card.SetCardSelected();
    }
}
