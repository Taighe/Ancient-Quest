using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : PhysicsObject
{
    public ActorData Data;
    public Vector2 Velocity;
    public bool IsGrounded;
    PlayerController m_input;
    Vector2 m_velocity;
    Vector2 m_targetVelocity;
    Vector2 m_animationVelocity;
    float m_jumpVelocity;
    float m_gravity;
    float m_velocityXSmoothing;
    float m_gravityModifier;
    float m_jumpGravityModifier;
    protected override void Awake()
    {
        base.Awake();
        m_velocity = Vector2.zero;
        m_input = GetComponent<PlayerController>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public void SetHorizontalMovement(float speed)
    {
        m_targetVelocity.x = speed;
    }

    public void Jump()
    {
        if(IsGrounded)
            m_velocity.y = Data.MinJumpHeight;

    }

    public void Jump(float force)
    {
        m_velocity.y = force;
    }

    public void EndJump()
    {
        if(m_velocity.y > 0 && !IsGrounded)
        {
            m_velocity.y *= 0.5f;
        }
    }

    public override void Update()
    {
        m_maxClimbAngle = Data.MaxClimbAngle;
        m_maxDescendAngle = Data.MaxDescendAngle;
        m_gravityModifier = Data.GravityModifier;
        m_jumpGravityModifier = Data.JumpGravityModifier;
        IsGrounded = m_collisonInfo.below;
        if (IsGrounded || m_collisonInfo.above)
            m_velocity.y = 0;

        if(m_input != null)
            //InputSystem.Update();

        m_velocity.x = Mathf.SmoothDamp(m_velocity.x, m_targetVelocity.x, ref m_velocityXSmoothing, IsGrounded ? Data.AccelerationTimeGrounded : Data.AccelerationTimeAirborne);
        if(m_velocity.y < 0)
            m_velocity.y += (Physics2D.gravity.y * m_gravityModifier) * Time.deltaTime;
        else
            m_velocity.y += (Physics2D.gravity.y * m_jumpGravityModifier) * Time.deltaTime;

        Vector2 collisonVel = Move(m_velocity * Time.deltaTime);
        float x = (float)System.Math.Round(collisonVel.x, 4);
        float y = (float)System.Math.Round(collisonVel.y, 4);
        m_animationVelocity = new Vector2(x != 0?m_velocity.x:x, y != 0?m_velocity.y:y);
        Velocity = m_velocity;
        base.Update();
    }
}
