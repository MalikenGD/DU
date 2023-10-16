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
    

    public event Action<Cell, CellUI> OnMouseClick;
    public event Action<object> OnBeginDragging;
    public event Action OnDragging;
    public event Action<PointerEventData> OnEndDragging;
    public event Action<Cell, CellUI> OnPointerEntering;
    public event Action<Cell, CellUI> OnPointerExiting;

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
        OnMouseClick?.Invoke(_cell, this);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEntering?.Invoke(_cell, this);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExiting?.Invoke(_cell, this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_cell.GetUnit() == null)
        {
            Debug.LogError("CellUI.OnBeginDrag: Trying to drag Null object.");
            return;
        }
        
        OnBeginDragging?.Invoke(_cell);
    }
    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log(eventData.selectedObject.GetComponent<Unit>());
        if (_cell.GetUnit() == null)
        {
            Debug.LogError("CellUI.OnDrag: Trying to drag Null object.");
            return;
        }
        
        OnDragging?.Invoke();
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        //TODO: Change material back from Transparent.
        OnEndDragging?.Invoke(eventData);
    }

    protected override void UpdateWhenInteractable()
    {
        //Debug.Log("Interactable");
    }
    
    protected override void UpdateWhenHighlighted()
    {
        _cellUIImage.color = _defaultCellUIImageColor;
        _cellUIImage.enabled = true;
    }

    protected override void UpdateWhenSelected()
    {
        _cellUIImage.color = highlightedColor;
        _cellUIImage.enabled = true;
        
    }
    
    protected override void UpdateWhenGreyedOut()
    {
        _cellUIImage.color = Color.gray;
        _cellUIImage.enabled = true;
        
    }
    
    protected override void UpdateWhenDisabled()
    {
        _cellUIImage.enabled = false;
    }

    protected override void Update()
    {
        base.Update();
        //Debug.Log($"My state is: {currentState}");
        
        
        
        
        //TODO: Delete debug code.
        if (_cell.GetUnit() != null)
        {
            _cellUIImage.enabled = true;
            _cellUIImage.color = Color.magenta;
        }
    }
    
}
