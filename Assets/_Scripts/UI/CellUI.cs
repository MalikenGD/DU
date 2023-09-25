using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CellUI : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Cell _cell;
    private Vector3 _worldPosition;
    private Image _cellUIImage;
    private Color _defaultCellUIImageColor;
    [SerializeField] private Color highlightedColor;
    

    public event Action<Cell, CellUI> OnCellClicked;
    public event Action<Unit> OnUnitDragging;
    public event Action OnUnitDragged;
    public event Action<Cell, CellUI> OnCellMouseOver;
    public event Action<Cell, CellUI> OnCellMouseExit;

    private void Start()
    {
        transform.parent = World.Instance.GetWorldSpaceCanvas().transform;
        _cell = parentObjectWithDataToDisplay as Cell;
        _cellUIImage = GetComponentInChildren<Image>();
        _defaultCellUIImageColor = _cellUIImage.color;
    }

    public void Initialize(Vector3 worldPosition)
    {
        Vector3 newWorldPosition = new Vector3(worldPosition.x, worldPosition.y + 0.01f, worldPosition.z);
        transform.position = newWorldPosition;
        
        SetNewState(UIState.Disabled);
    }
    
    

    public void HandleClickLogic()
    {
        OnCellClicked?.Invoke(_cell, this);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnCellMouseOver?.Invoke(_cell, this);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        OnCellMouseExit?.Invoke(_cell, this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Maybe change material of _cell.GetUnit() to be transparent?
        OnUnitDragging?.Invoke(_cell.GetUnit());
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        //Maybe change material of _cell.GetUnit() to be opaque again?
        OnUnitDragged?.Invoke();
    }

    

    public void OnDrag(PointerEventData eventData)
    {

        if (_cell.GetUnit() is not null)
        {
            GameObject unitToDrag = _cell.GetUnit().gameObject;

            Vector3 unitPosition = unitToDrag.transform.position;

            RaycastHit hit;

            Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit);
            unitToDrag.transform.position = new Vector3(hit.point.x, unitToDrag.transform.position.y, hit.point.z);
        }
    }

    protected override void UpdateWhenInteractable()
    {
        //Debug.Log("Interactable");
    }
    
    protected override void UpdateWhenHighlighted()
    {
        //Debug.Log("Highlighted");
        _cellUIImage.color = _defaultCellUIImageColor;
        _cellUIImage.enabled = true;
    }

    protected override void UpdateWhenSelected()
    {
        //Debug.Log("Selected");
        _cellUIImage.color = highlightedColor;
        _cellUIImage.enabled = true;
        
    }
    
    protected override void UpdateWhenGreyedOut()
    {
        //Debug.Log("GreyedOut");
        _cellUIImage.color = Color.gray;
        _cellUIImage.enabled = true;
        
    }
    
    protected override void UpdateWhenDisabled()
    {
        //Debug.Log("Disabled");
        _cellUIImage.enabled = false;
    }

    protected override void Update()
    {
        base.Update();
        Debug.Log($"My state is: {currentState}");
        
    }
    
}
