using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card : MonoBehaviour
{
    
    //TODO: Card currently stores CardUIPrefab in drag and drop reference. 
    //TODO: Maybe make a cardDataSO instead? Consistency's sake?
    public event Action OnUnitDataUpdated;
    public event Action<UnitDataSO> OnCardSelected;

    [SerializeField] private CardDataSO cardDataSO;
    private GameObject _cardUIPrefab;
    private Transform _parentTransform;
    private UnitDataSO _unitData;
    private CardUI _cardUI;
    


    private void Start()
    {
        _cardUIPrefab = cardDataSO.GetCardUIPrefab();
    }


    public Card(UnitDataSO unitData)
    {
        SetUnitData(unitData);
    }

    public CardUI CreateCardUI()
    {
        UIBehaviour cardUIBehaviour = World.Instance.BuildUI(_cardUIPrefab, this);

        return cardUIBehaviour as CardUI;
    }

    public UnitDataSO GetUnitData()
    {
        return _unitData;
    }

    

    public void SetUnitData(UnitDataSO newUnitData)
    {
        _unitData = newUnitData;
        
        OnUnitDataUpdated?.Invoke();
    }

    public void SetCardSelected()
    {
        OnCardSelected?.Invoke(_unitData);
    }
}
