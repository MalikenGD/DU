using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

//using Random = UnityEngine.Random;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopDataSO shopData;
    
    [SerializeField] private int maxCardsAllowedInHand = 5;
    
    
    private Grid _grid;
    private Transform _parentTransform;
    private List<UnitDataSO> _units;
    private List<Card> _cardsInHand = new List<Card>();
    private UnitDataSO _selectedCardUnit;


    private void Start()
    {
        _units = shopData.GetUnitDataSOList();
        
        if (_units.Count <= 1)
        {
            Debug.LogError("Shop.Start: List of AllUnitData is too low/empty");
        }

        InitializeCards();
        RandomizeHandOfCards();
        World.Instance.BuildUI(shopData.GetShopUIPrefab(), this);
    }

    


    private void InitializeCards()
    {
        
        for (int i = 0; i < maxCardsAllowedInHand; i++)
        {
            Card card = Instantiate(shopData.GetCardPrefab(), transform).GetComponent<Card>();
            card.OnCardSelected += SetCardSelected;
            _cardsInHand.Add(card);
            
        }
    }


    //TODO: On state updated to BuyPhase, randomize Shop for UI
    //This should subscribe to World.OnStateUpdated?
    private void RandomizeHandOfCards()
    {
        List<UnitDataSO> listOfRandomUnits = ChooseUniqueUnitsAtRandom(_cardsInHand.Count);
        
        for (int i = 0; i < maxCardsAllowedInHand; i++)
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

    public void SetGridReference(Grid gridReference)
    {
        this._grid = gridReference;
    }

    private void SetCardSelected(UnitDataSO selectedCardUnitData)
    {
        _selectedCardUnit = selectedCardUnitData;

        if (_grid.HasSelectedCell())
        {
            CreateUnit(_grid.GetSelectedCell());
            _grid.SetSelectedCell(null);
            _selectedCardUnit = null;
        }
    }

    private void CreateUnit(Cell cell)
    {
        //TODO: Refactor into Factory
        Vector3 positionToSpawnUnitAt =
            _grid.ConvertFromGridPositionToWorldPosition(cell.GetGridPosition());
        
        Unit newUnit =
            Instantiate(_selectedCardUnit.GetUnitPrefab(), positionToSpawnUnitAt,
                quaternion.identity).GetComponent<Unit>();
        
        newUnit.InitialSetup(cell);
                    
        //TODO: Refactor into Factory
        World.Instance.CreatePlayerUnit(newUnit);
    }
}
