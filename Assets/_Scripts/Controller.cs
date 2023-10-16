using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    //private List<BaseAction> _actions; // List of Actions for Controller

    public abstract void HandleInputs();
}
