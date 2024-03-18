using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Unit.GridBehaviour
{
    public enum Priority
    {
        Unassigned = 0,
        Immediate,
        Normal,
        Late
    }
    public class Component : MonoBehaviour
    {
        public Priority priority = Priority.Unassigned;

        public virtual void Start()
        {
            if (priority == Priority.Unassigned)
            {
                Debug.Log($"{name}.Start: Priority level unassigned.");
            }
            
        }
    }
}