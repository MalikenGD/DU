using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : IUIObject
{
    private Card _card;
    private GameObject _cardUIGameObject;
    private Image _backgroundImage;
    private TextMeshProUGUI _goldCostText;
    private TextMeshProUGUI _unitNameText;
    private UnitDataSO _currentUnitDataToDisplay;
    
    
    public CardUI(Card card, GameObject cardUIGameObject)
    {
        SetCardUIGameObject();
        _card = card;
        _currentUnitDataToDisplay = _card.GetUnitData();
        _card.OnUnitDataUpdated += UpdateUI;
        //TODO: Handle card/cardUI deletion and unsubscribe from Card event?

        _cardUIGameObject = cardUIGameObject;
        InitializeCardUISettings();
        UpdateUI();
    }

    private void InitializeCardUISettings()
    {
        _backgroundImage = _cardUIGameObject.transform.Find("BackgroundUnitImage").GetComponent<Image>();
        _goldCostText = _cardUIGameObject.transform.Find("GoldText").GetComponent<TextMeshProUGUI>();
        _unitNameText = _cardUIGameObject.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
    }

    public GameObject GetUIGameObject()
    {
        return _cardUIGameObject;
    }

    private void SetCardUIGameObject()
    {
        List<GameObject> singularCardUIGameObjects = new List<GameObject>();
        singularCardUIGameObjects.AddRange(GameObject.FindGameObjectsWithTag("CardUISingle"));
        foreach (GameObject cardUIGameObject in singularCardUIGameObjects)
        {
            if (cardUIGameObject.GetComponentInChildren<Image>().sprite == null)
            {
                _cardUIGameObject = cardUIGameObject;
                break;
            }
        }

        if (_cardUIGameObject == null)
        {
            Debug.LogError("CardUI was not assigned a free CardUISingular GameObject");
        }
    }

    private void UpdateUI()
    {
        _currentUnitDataToDisplay = _card.GetUnitData();
        _backgroundImage.sprite = _currentUnitDataToDisplay.GetUnitSpriteForShopBackground();
        _goldCostText.text = _currentUnitDataToDisplay.GetUnitGoldCost().ToString();
        _unitNameText.text = _currentUnitDataToDisplay.GetUnitName();
    }
}
