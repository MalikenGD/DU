using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card : MonoBehaviour
{
    public event Action OnUnitDataUpdated;
    public event Action<UnitDataSO> OnCardSelected;

    [SerializeField] private GameObject cardUIPrefab;
    private Transform _parentTransform;
    private UnitDataSO _unitData;
    private CardUI _cardUI;
    

    public Card(UnitDataSO unitData)
    {
        SetUnitData(unitData);
    }

    public CardUI CreateCardUI()
    {
        UIBehaviour cardUIBehaviour = World.Instance.BuildUI(cardUIPrefab, this);

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
