using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    //TODO: 
    //if UIManager.PlacingSelectedUnit, if !GridManager.GridSquareOccupied ShopManager.
    //if (UIManager.UnitToBuyIsSelected) GetGridObjectFromMouseClick() return;
    
    /*public static InputManager Instance; // Was experimenting with whether or not I needed a singleton. Went events instead.

    private GridObject _gridObject;

    public static event Action<Vector3> OnClickedGrid;
    public static event Action<Unit> OnClickedUnit;

    public static event Action<Vector3> OnMouseHolding;
    public static event Action OnMouseHoldingFinished;

    

    private float _mouseHeldThreshold = 0.15f;
    private float _timeMouseHasBeenHeldDown;
    private bool _mouseHeld;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (_mouseHeld)
        {
            HandleMouseHeldLogic();
        }

        if (Input.GetMouseButton(0))
        {
            if (_mouseHeld) {return;}
            
            _timeMouseHasBeenHeldDown += Time.deltaTime;
            if (_timeMouseHasBeenHeldDown > _mouseHeldThreshold)
            {
                _mouseHeld = true;
                HandleMouseClickLogic();
            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            _timeMouseHasBeenHeldDown = 0f;
            _mouseHeld = false;
            OnMouseHoldingFinished?.Invoke();
        }
    }

    private GameObject GetObjectUnderMouse()
    {
        GameObject gameObject = null;
        
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            gameObject = hit.collider.gameObject;
        }

        return gameObject;
    }

    private void HandleMouseHeldLogic()
    {
        if (!_mouseHeld)
        {
            OnMouseHoldingFinished?.Invoke();
            return;
        }
        
        //Make sure I have a selected unit
        //HandleMouseClickLogic();

        OnMouseHolding?.Invoke(GetMousePositionAlongGridVisual());
    }

    private void HandleMouseClickLogic()
    {
        var objectUnderMouse = GetObjectUnderMouse();
        var isObjectAUnit = objectUnderMouse.GetComponent<Unit>();

        if (isObjectAUnit)
        {
            var unit = objectUnderMouse.GetComponent<Unit>();
            OnClickedUnit?.Invoke(unit);
            return;
        }
        
        var isObjectGridVisual = objectUnderMouse.CompareTag("GridVisual");

        if (isObjectGridVisual)
        {
            OnClickedGrid?.Invoke(GetMousePositionAlongGridVisual()); // Likely need for placing down units from shop
        }

    }

    private Vector3 GetMousePositionAlongGridVisual()
    {
        Vector3 mousePositionAlongGridVisual = Vector3.zero;
            
        //Consider refactoring for a RaycastNonAlloc for perf gains.
        //Has downside of potentially lost overflow data though.
        RaycastHit[] hits;
            
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(ray);

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("GridVisual"))
            {
                    mousePositionAlongGridVisual = hit.point;
            }
        }

        return mousePositionAlongGridVisual;
    }*/
}
