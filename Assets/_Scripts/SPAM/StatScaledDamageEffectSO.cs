using System.Collections;
using System.Collections.Generic;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

[CreateAssetMenu(menuName = "SPAM Framework/Ability effects/StatScaledDamageEffect", fileName = "damageeffect.asset")]
public class StatScaledDamageEffectSO : AbilityEffectSO  
{  
    // This exposes the stat to use, and base damage, in the effect settings
    [SerializeField] private UnitCombatDataSO _unitCombatDataSO;
    [SerializeField] private CharacterStat _damageModifierStat;  
    [SerializeField] private int _baseDamage;  

    protected override string _metaHelpDescription => "";  

    public override void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,  
        AbilityInvoker invoker)  
    {  
        // Get the targets component that's responsible for taking damage  
        // Here SPAMs built in interface is used.  
        var damageable = target.Transform.GetComponent<IDamageable>();  
        if (damageable == null) return; // Can't deal damage if target isn't damageable  

        /*
        // Get the component that holds the stats for the character  
        var casterStats = invoker.GetComponent<CharacterStats>();  

        // Get the actual stat to scale from  
        var damageModifierStat = casterStats.GetStatValue(_damageModifierStat);  

        // The actual calculation for how the stat affects the damage is  
        // highly specific to your game. This is just a very simple example so just  
        // add the stat to the final damage.  
        var finalDamage = _baseDamage + damageModifierStat;  

        damageable.Damage(finalDamage);  */
    }  
}
