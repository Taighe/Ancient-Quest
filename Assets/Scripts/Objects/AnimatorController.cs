using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KinematicObject3D))]
public class AnimatorController : MonoBehaviour
{
    public Animator Animator;
    private KinematicObject3D _kinematicObj;
    // Start is called before the first frame update
    void Start()
    {
        _kinematicObj = GetComponent<KinematicObject3D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Animator != null)
        {
            int ground = Convert.ToInt32(_kinematicObj.IsGrounded());
            int air = Convert.ToInt32(!_kinematicObj.IsGrounded());

            Animator.SetLayerWeight(0, ground);
            Animator.SetLayerWeight(1, air);
            Animator.SetBool("isGrounded", _kinematicObj.IsGrounded());
            Animator.SetFloat("velX", Mathf.Abs(_kinematicObj.Velocity.x));
            Animator.SetFloat("velY", _kinematicObj.Velocity.y);
            Animator.SetBool("canFidget", _kinematicObj.CanFidget());

        }
    }
}
