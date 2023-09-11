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
    [SerializeField] private int _MaxCardsAllowedInHand = 5;

    private GridManager _gridManager;
    private Transform _parentTransform;
    private List<UnitDataSO> _units;
    private List<Card> _cardsInHand = new List<Card>();
    private UnitDataSO _selectedCardUnit;

    private void Start()
    {
        _units = _shopData.GetUnitDataSOList();
        if (_units.Count <= 1)
        {
            Debug.LogError("Shop.Start: list of AllUnitData is too low/empty.");
        }

        InitializeCards();
        RandomizeHandOfCards();
        World.Instance.BuildUI(_shopData.GetShopUIPrefab(), this);
    }

    private void InitializeCards()
    {
        for (int i = 0; i < _MaxCardsAllowedInHand; i++)
        {
            Card card = Instantiate(_shopData.GetCardPrefab(), transform).GetComponent<Card>();
            card.OnCardSelected += SetCardSelected;
            _cardsInHand.Add(card);
        }
    }

    //TODO: On state updated to BuyPhase, randomize Shop for UI
    //This should subscribe to World.OnStateUpdated?
    private void RandomizeHandOfCards()
    {
        List<UnitDataSO> listOfRandomUnits = ChooseUniqueUnitsAtRandom(_cardsInHand.Count);
        for (int i = 0; i < _MaxCardsAllowedInHand; i++)
        {
            _cardsInHand[i].SetUnitData(listOfRandomUnits[i]);
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
        int randomNumber = Random.Range(0, _units.Count);
        UnitDataSO randomUnit = _units[randomNumber];

        return randomUnit;
    }
    

    public List<Card> GetCardsInHand()
    {
        return _cardsInHand;
    }

    public void SetGridManagerReference(GridManager gridManager)
    {
        _gridManager = gridManager;
        _gridManager.OnCellSelected += OnCellSelected;
    }

    private void SetCardSelected(UnitDataSO selectedCardUnit)
    {
        _selectedCardUnit = selectedCardUnit;
        if (_gridManager.HasSelectedCell())
        {
            CreateUnit(_selectedCardUnit, _gridManager.GetSelectedCell());
            SetCardSelected(null);
            _gridManager.SetSelectedCell(null);
        }
    }

    private bool HasSelectedCard()
    {
        return _selectedCardUnit != null;
    }

    private void OnCellSelected(Cell selectedCell)
    {
        if (HasSelectedCard())
        {
            CreateUnit(_selectedCardUnit, selectedCell);
            SetCardSelected(null);
            _gridManager.SetSelectedCell(null);
        }
    }

    private void CreateUnit(UnitDataSO unit, Cell cell)
    {
        //Instantiate(_selectedCardUnitData.GetUnitPrefab(), transform.Position, quaternion.identity
    }
}
