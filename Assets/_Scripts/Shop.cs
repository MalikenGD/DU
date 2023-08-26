using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Random = UnityEngine.Random;

public class Shop
{
    private readonly List<UnitDataSO> _listOfAllUnitData;
    private List<UnitDataSO> _unitsCurrentlyInShop = new List<UnitDataSO>();
    private List<Card> _listOfActiveCards = new List<Card>();
    private const int MaxCardsAllowedInHand = 5;

    public Shop(List<UnitDataSO> listOfAllUnitData)
    {
        _listOfAllUnitData = listOfAllUnitData;
        
        CreateShopUI();
        RandomizeShop();
    }

    private void CreateShopUI()
    {
        World.Instance.CreateUIObject(0, this);
    }
    

    //TODO: On state updated to BuyPhase, randomize Shop for UI
    //This should subscribe to World.OnStateUpdated?
    private void RandomizeShop()
    {
        List<UnitDataSO> listOfRandomUnits = SelectUnitsForShopAtRandom();

        foreach (UnitDataSO unit in listOfRandomUnits)
        {
            if (_listOfActiveCards.Count < MaxCardsAllowedInHand)
            {
                _listOfActiveCards.Add(new Card(unit));
            }
            else
            {
                foreach (Card card in _listOfActiveCards)
                {
                    if (card.GetUnitData() != unit) card.SetUnitData(unit);
                }
            }
        }
    }

    private List<UnitDataSO> SelectUnitsForShopAtRandom()
    {
        _unitsCurrentlyInShop.Clear();

        while (_unitsCurrentlyInShop.Count < MaxCardsAllowedInHand)
        {
            if (_listOfAllUnitData.Count < MaxCardsAllowedInHand)
            {
                Debug.LogError($"Amount of units available to populate shop is too few: {_listOfAllUnitData.Count}, or maximum allowed cards is too few: {MaxCardsAllowedInHand}");
                break;
            }
            int randomNumber = Random.Range(0, _listOfAllUnitData.Count);
            UnitDataSO randomUnit = _listOfAllUnitData[randomNumber];
            
            if (_unitsCurrentlyInShop.Contains(randomUnit)) continue;
            _unitsCurrentlyInShop.Add(randomUnit);
        }


        if (_unitsCurrentlyInShop.Count < MaxCardsAllowedInHand)
        {
            Debug.LogError($"Number of units selected for shop: {_unitsCurrentlyInShop.Count} is too low.");
        }
        return _unitsCurrentlyInShop;
    }
}
