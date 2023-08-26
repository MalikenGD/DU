using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public event Action OnUnitDataUpdated; 
    
    private UnitDataSO _unitData;
    private CardUI _cardUI;

    public Card(UnitDataSO unitData)
    {
        _unitData = unitData;
        CreateCardUI();
    }

    private void CreateCardUI()
    {
        World.Instance.CreateUIObject(1, this);
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
}
