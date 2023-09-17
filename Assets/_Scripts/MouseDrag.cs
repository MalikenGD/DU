using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Update = UnityEngine.PlayerLoop.Update;

public class MouseDrag : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInputComponent;
    
    private InputAction _dragAction;
    private InputActionMap _inputActionMap;
    private GameObject _gameObjectToDrag;
    private bool _isHolding;
    private Vector2 delta;

    private void Start()
    {
        _gameObjectToDrag = null;
        _inputActionMap = playerInputComponent.currentActionMap;
        _dragAction = _inputActionMap.FindAction("Drag");

        InputAction holding = _inputActionMap.FindAction("Drag");
        holding.performed += OnClickStarted;
        holding.canceled += OnClickCanceled;
        
    }

    private void OnClickStarted(InputAction.CallbackContext context)
    {
        _isHolding = true;
    }


    private void OnClickCanceled(InputAction.CallbackContext context)
    {
        _isHolding = false;
        _gameObjectToDrag = null;
    }

    private void FixedUpdate()
    {
        if (_isHolding)
        {
            RaycastHit hit;
            if (_gameObjectToDrag is null)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit))
                {
                    if (!hit.collider.CompareTag("Unit")) return;
                    _gameObjectToDrag = hit.collider.gameObject;
                }
            }

            Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit);
            _gameObjectToDrag.transform.position = new Vector3(hit.point.x, _gameObjectToDrag.transform.position.y, hit.point.z);
        }
    }
}
