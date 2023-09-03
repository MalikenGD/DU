using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "ShopData", menuName = "ScriptableObjects/Shop/ShopData")]
public class ShopDataSO : ScriptableObject
{
    [SerializeField] private GameObject _shopUIPrefab;
    [SerializeField] private UnitCompleteListSO _completeListOfUnitDataSO;
    [SerializeField] private GameObject _cardPrefab;

    public GameObject GetShopUIPrefab()
    {
        return _shopUIPrefab;
    }

    public List<UnitDataSO> GetUnitDataSOList()
    {
        return _completeListOfUnitDataSO.GetList();
    }

    public GameObject GetCardPrefab()
    {
        return _cardPrefab;
    }
}
