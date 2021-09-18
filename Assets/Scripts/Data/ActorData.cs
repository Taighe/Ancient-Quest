using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Actor", menuName = "ScriptableObjects/Actor/ActorData", order = 1)]
public class ActorData : ScriptableObject
{
    [Header("Gravity")]
    public float GravityModifier = 1;
    [Header("Movement")]
    public float Speed = 1;
    public float MaxSpeed = 5;
    public float Traction = 1;
    public float AccelerationTimeAirborne = .2f;
    public float AccelerationTimeGrounded = .1f;
    public float TurnSpeed = 800;
    [Header("Slopes")]
    public float MaxClimbAngle = 80;
    public float MaxDescendAngle = 80;

    [Header("Jumping")]
    public float MinJumpHeight = 3;
    public float MaxJumpHeight = 6;
    public float JumpGravityModifier = 1;
    [Header("Misc")]
    public float MinGroundNormalY = .65f;
    public bool IsFlying = false;
    public float FidgetTime = 1.0f;
}
