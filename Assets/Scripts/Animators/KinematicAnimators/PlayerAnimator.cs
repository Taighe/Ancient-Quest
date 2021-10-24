using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : KinematicAnimator
{
    public SkinnedMeshRenderer ShieldMesh;
    public SkinnedMeshRenderer SlingshotMesh;
    private Player _player;

    public override void Awake()
    {
        base.Awake();
        _player = GetComponent<Player>();
    }

    public void Start()
    {
        UpdatePowerUps();
    }

    public override bool UpdateAnimations()
    {
        TurnSpeed = _player.Data.TurnSpeed;
        if (!base.UpdateAnimations())
            return false;

        Animator.SetFloat("axisX", ControllerMaster.Input.GetAxis().x);
        Animator.SetFloat("axisY", ControllerMaster.Input.GetAxis().y);
        UpdatePowerUps();

        return true;
    }

    private void UpdatePowerUps()
    {
        float shield = _player.HasPowerUp(PowerUps.Shield) && _player.IsGrounded ? 1 : 0;
        ShieldMesh.enabled = _player.HasPowerUp(PowerUps.Shield);
        Animator.SetBool("canFidget", _player.CanFidget() && shield == 0);
        Animator.SetFloat("shieldPU", shield);
        Animator.SetLayerWeight(5, shield);

        float sling = _player.HasPowerUp(PowerUps.Sling) ? 1 : 0;
        SlingshotMesh.enabled = sling == 1;
        Animator.SetFloat("slingPU", shield);
        Animator.SetLayerWeight(2, sling);
        Animator.SetLayerWeight(3, sling);
        Animator.SetLayerWeight(4, sling);
        if (_player.IsUsing)
        {
            Animator.SetTrigger("useTrigger");
        }
    }
}
