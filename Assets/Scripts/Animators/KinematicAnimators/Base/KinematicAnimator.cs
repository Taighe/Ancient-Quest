using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicAnimator : AnimatorController
{
    protected KinematicObject3D _kinematicObj;

    public override void Awake()
    {
        _kinematicObj = GetComponent<KinematicObject3D>();
    }

    public override bool UpdateAnimations()
    {
        // Facing towards direction
        _kinematicObj.transform.rotation = Quaternion.RotateTowards(_kinematicObj.transform.rotation, Quaternion.Euler(0, (float)_kinematicObj.Direction, 0), TurnSpeed * Time.fixedDeltaTime);


        if (Animator != null && Animator.runtimeAnimatorController != null)
        {
            int ground = _kinematicObj.IsGrounded ? 1 : 0;
            int air = ground == 0 ? 1 : 0;

            Animator.SetLayerWeight(0, ground);
            Animator.SetLayerWeight(1, air);
            Animator.SetBool("isGrounded", _kinematicObj.IsGrounded);
            Animator.SetFloat("velX", Mathf.Abs(_kinematicObj.Velocity.x));
            Animator.SetFloat("velY", _kinematicObj.Velocity.y);
            Animator.SetBool("canFidget", _kinematicObj.CanFidget());
            return true;
        }

        //Debug.LogError($"{gameObject.name} Object: {_kinematicObj.GetType().Name} {GetType().Name} is null.");
        return false;
    }
}
