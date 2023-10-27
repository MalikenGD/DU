using System;
using System.Collections;
using System.Collections.Generic;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;

public class AgentSetDestination : MonoBehaviour
{
    private void Start()
    {
        MoveTo(new Vector3(55, transform.position.y, 25));
    }

    public void MoveTo(Vector3 newPosition)
    {
        GetComponent<AgentAuthoring>().SetDestination(newPosition);
    }
}
