using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : KinematicAnimator
{
    private Player _player;

    public override void Awake()
    {
        base.Awake();
        _player = GetComponent<Player>();
    }
    public override bool UpdateAnimations()
    {
        if (!base.UpdateAnimations())
            return false;

        Animator.SetFloat("axisX", ControllerMaster.Input.GetAxis().x);
        Animator.SetFloat("axisY", ControllerMaster.Input.GetAxis().y);

        return true;
    }
}
