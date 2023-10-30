using System;
using System.Collections;
using System.Collections.Generic;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;

public class AgentSetDestination : MonoBehaviour
{
    public void MoveTo(Vector3 newPosition)
    {
        GetComponent<AgentAuthoring>().SetDestination(newPosition);
    }
}
