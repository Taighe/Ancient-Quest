using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public Animator Animator;
    protected Object3D _obj3D;
    // Start is called before the first frame update
    public virtual void Awake()
    {
        _obj3D = GetComponent<Object3D>();
    }

    public virtual bool UpdateAnimations()
    {
        if (Animator != null && Animator.runtimeAnimatorController != null)
        {
            Animator.SetFloat("velX", Mathf.Abs(_obj3D.Velocity.x));
            Animator.SetFloat("velY", _obj3D.Velocity.y);
            return true;
        }

        //Debug.LogError($"{gameObject.name} Object: {_obj3D.GetType().Name} {GetType().Name} is null.");
        return false;
    }
}
