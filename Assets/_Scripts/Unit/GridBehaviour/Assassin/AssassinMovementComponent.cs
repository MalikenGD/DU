using System.Collections.Generic;
using _Scripts.Unit.GridBehaviour;
using UnityEngine;

public class AssassinMovementComponent : MovementComponent
{
    //TODO: Refactor to combatdataSO? Or unique ability?
    [SerializeField] private float leapCooldown = 0.5f;
    private float _minimumLeapThreshold = 3f;
    private float timeUntilNextLeap = 0.5f;
    private bool canLeap = true;

    public override void Tick()
    {
        base.Tick();
        
        //TODO: Refactor into ability (leap) probably custom.
        timeUntilNextLeap -= Time.deltaTime;

        if (timeUntilNextLeap <= 0)
        {
            canLeap = true;
            
        }
    }

    public bool BT_ShouldLeap(Unit target)
    {
        return Vector3.Distance(transform.position, target.transform.position) > _minimumLeapThreshold;
    }
    
    public void BT_Leap(Unit target)
    {
        if (!canLeap)
        {
            return;
        }
        
        //TODO: Turn this from a teleport to an actual leap, maxing out at some high bounds, then coming back down.
        //transform.position = Vector3.SmoothDamp()
        //transform.position = Vector3.Slerp()

        Vector3 targetPosition = target.transform.position;
        
        //TODO: 
        //play anim (maybe key event in animation to leap? Probably not but...)
        transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z - _attackRange.Value);
        //play end anim(or maybe just let anim finish)
        

        timeUntilNextLeap = leapCooldown;
        canLeap = false;

    }
}