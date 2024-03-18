using System.Collections.Generic;
using UnityEngine;

public class AssassinMovementComponent : MovementComponent
{
    public void BT_Leap(Unit target)
    {
        //TODO: Turn this from a teleport to an actual leap, maxing out at some high bounds, then coming back down.
        //transform.position = Vector3.SmoothDamp()
        //transform.position = Vector3.Slerp()

        Vector3 targetPosition = target.transform.position;
        //play anim (maybe key event in animation to leap? Probably not but...)
        transform.position = new Vector3(targetPosition.x, targetPosition.y - _attackRange.Value, targetPosition.z);
        //play end anim(or maybe just let anim finish)

    }
}