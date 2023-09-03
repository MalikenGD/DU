using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

//using Random = UnityEngine.Random;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopDataSO _shopData;
    
    private const int MaxCardsAllowedInHand = 5;
    
    
    private GridManager _gridManagerReference;
    private Transform _parentTransform;
    private List<UnitDataSO> _listOfAllUnitData;
    private List<Card> _listOfCardsInHand = new List<Card>();
    private UnitDataSO _selectedCardUnitData;
    private bool _cardSelected = false;
    

    private void Start()
    {
        _listOfAllUnitData = _shopData.GetUnitDataSOList();
        if (_listOfAllUnitData.Count <= 1)
        {
            Debug.LogError("List of AllUnitData is too low/empty");
            throw new Exception();
        }

        InitializeCards();
        RandomizeHandOfCards();
        CreateShopUI();
    }

    private void Update()
    {
        if (_cardSelected)
        {
            if (_gridManagerReference.GetCellSelectedStatus())
            {
                //Instantiate(_selectedCardUnitData.GetUnitPrefab(), transform.Position, quaternion.identity);
                _cardSelected = false;
                //Reset selectedCell as well. But how? Event in World, or _gridManagerReference.ResetSelectedCell()?
            }
        }
    }


    private void InitializeCards()
    {
        for (int i = 0; i < MaxCardsAllowedInHand; i++)
        {
            _listOfCardsInHand.Add(Instantiate(_shopData.GetCardPrefab(), transform).GetComponent<Card>());
        }

        RegisterToCardEvents();
    }

    private void RegisterToCardEvents()
    {
        foreach (Card card in _listOfCardsInHand)
        {
            card.OnCardSelected += SetCardSelected;
        }
    }

    private void CreateShopUI()
    {
        World.Instance.BuildUI(_shopData.GetShopUIPrefab(), this);
    }
    

    //TODO: On state updated to BuyPhase, randomize Shop for UI
    //This should subscribe to World.OnStateUpdated?
    private void RandomizeHandOfCards()
    {
        List<UnitDataSO> listOfRandomUnits = ChooseUniqueUnitsAtRandom(_listOfCardsInHand.Count);
        
        for (int i = 0; i < MaxCardsAllowedInHand; i++)
        {
            _listOfCardsInHand[i].SetUnitData(listOfRandomUnits[i]);
        }
    }

    //List will have no duplicates
    private List<UnitDataSO> ChooseUniqueUnitsAtRandom(int numberOfUnits)
    {
        
        List<UnitDataSO> randomizedUnits = new List<UnitDataSO>();

        for (int i = 0; i < numberOfUnits; i++)
        {
            UnitDataSO randomUnitData = ChooseUnitAtRandom();

            while (randomizedUnits.Contains(randomUnitData))
            {
                randomUnitData = ChooseUnitAtRandom();
            }

            randomizedUnits.Add(randomUnitData);
        }

        return randomizedUnits;
    }

    private UnitDataSO ChooseUnitAtRandom()
    {
        int randomNumber = Random.Range(0, _listOfAllUnitData.Count);
        UnitDataSO randomUnit = _listOfAllUnitData[randomNumber];

        return randomUnit;
    }
    

    public List<Card> GetCardsInHand()
    {
        return _listOfCardsInHand;
    }

    public void SetGridManagerReference(GridManager gridManagerReference)
    {
        _gridManagerReference = gridManagerReference;
    }

    private void SetCardSelected(UnitDataSO selectedCardUnitData)
    {
        _cardSelected = true;
        _selectedCardUnitData = selectedCardUnitData;
    }
}
