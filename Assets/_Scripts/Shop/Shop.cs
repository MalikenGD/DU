using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

//using Random = UnityEngine.Random;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopDataSO shopData;
    
    [SerializeField] private int maxCardsAllowedInHand = 5;

    public event Action<Unit> OnGridUnitCreated;
    
    private Grid _grid;
    private Player _player; // For gold?
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
        GameObject cardPrefab = shopData.GetCardPrefab();
        for (int i = 0; i < maxCardsAllowedInHand; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, transform);
            bool isCardObjectValid = cardObject != null;
            Card card = isCardObjectValid ? cardObject.GetComponent<Card>() : null;
            bool isCardValid = card != null;

            if (!isCardValid)
            {
                if (!isCardObjectValid)
                {
                    Debug.LogError("Shop.InitializeCards: Card GameObject is not valid.");
                }

                if (!isCardValid)
                {
                    Debug.LogError("Shop.InitializeCards: Card is not valid.");
                }
                continue;
            }
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
        _grid = gridReference;
    }

    private void SetCardSelected(UnitDataSO selectedCardUnitData)
    {
        _selectedCardUnit = selectedCardUnitData;

        if (_grid.HasSelectedCell())
        {
            CreateUnit();
            _selectedCardUnit = null;
        }
    }
    
    private void CreateUnit()
    {
        GameObject unitPrefab = _selectedCardUnit.GetUnitPrefab();
        
        Unit newUnit = World.Instance.BuildUnit(unitPrefab);
        
        if (_grid.HasSelectedCell())
        {
            GridPosition selectedCellGridPosition = _grid.GetSelectedCell().GetGridPosition();
            Vector3 spawningPosition = _grid.ConvertFromGridPositionToWorldPosition(selectedCellGridPosition);
            BehaviourTree behaviourTree = _selectedCardUnit.GetBehaviourTree();
            UnitGridBehaviour unitGridBehaviour = newUnit.AddComponent<UnitGridBehaviour>();
            
            newUnit.SetFaction(0);
            
            OnGridUnitCreated?.Invoke(newUnit);
            
            AIController aiController = newUnit.GetComponentInChildren<AIController>();
            bool isAIControllerNull = aiController == null;

            if (isAIControllerNull)
            {
                Debug.LogError("Shop.CreateUnit: AIControllerIsNull for New Unit.");
                //Can't return, too late in code?
            }
        
            aiController.SetBehaviourTree(behaviourTree);
            
            World.Instance.OnGameStateChanged += unitGridBehaviour.OnGameStateChanged;
            
            newUnit.gameObject.transform.position = spawningPosition;
            
            _grid.SetUnitAtSelectedCell(newUnit);
        }
        

        

        //Someone should subscribe to this, but maybe it's brain?\
        //This is mostly for turning on/off the navmesh,
        //But will eventually be for resetting unit position (movement component?)
        //And Resetting health (health component?)
        
        
        
        //Create unit
        //--
        //
        //--
        //Register(?) unit to the GameMode
        //GameMode creates AIController
        //GameMode assigns AIController new Unit
        //AIController creates Brain and passes in this unit as ControlledUnit
        //Brain then communicates with MovementComponent
    }

    public void SetPlayerReference(Player player)
    {
        _player = player;
    }
}
