using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KinematicObject3D))]
public class AnimatorController : MonoBehaviour
{
    public Animator Animator;
    protected KinematicObject3D _kinematicObj;
    // Start is called before the first frame update
    void Awake()
    {
        _kinematicObj = GetComponent<KinematicObject3D>();
    }

    public virtual bool UpdateAnimations()
    {
        if (Animator != null)
        {
            int ground = Convert.ToInt32(_kinematicObj.IsGrounded);
            int air = Convert.ToInt32(!_kinematicObj.IsGrounded);

            Animator.SetLayerWeight(0, ground);
            Animator.SetLayerWeight(1, air);
            Animator.SetBool("isGrounded", _kinematicObj.IsGrounded);
            Animator.SetFloat("velX", Mathf.Abs(_kinematicObj.Velocity.x));
            Animator.SetFloat("velY", _kinematicObj.Velocity.y);
            Animator.SetBool("canFidget", _kinematicObj.CanFidget());
            return true;
        }

        Debug.LogError($"{gameObject.name} Object: {_kinematicObj.GetType().Name} {GetType().Name} is null.");
        return false;
    }
}
