using System;
using System.Collections.Generic;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour, IAbilityTarget, IDamageable
{
    private int _faction;
    [SerializeField]
    private List<CharacterStat> stats;
    
    public Transform Transform { get; private set; }
    
    

    private void Start()
    {
        Transform = transform;
        stats = new List<CharacterStat>();
    }

    public void SetFaction(int faction)
    {
        _faction = faction;
    }

    public int GetFaction()
    {
        return _faction;
    }
    
    //set these
    public AudioSource AudioSource { get; }
    public Vector3 BasePosition { get; }
    public void Damage(int damageAmount)
    {
        Debug.Log("Taking Damage In Unit");
        HealthComponent healthComponent = GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.TakeDamage(damageAmount);
        }
    }
}