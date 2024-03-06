using System.Collections;
using System.Collections.Generic;
using _Scripts.Unit.GridBehaviour;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "Test", menuName = "ScriptableObjects/TEST2")]
public class ComponentDataSO : ScriptableObject
{
    [OdinSerialize]
    public List<IComponent> _components = new List<IComponent>();
}
