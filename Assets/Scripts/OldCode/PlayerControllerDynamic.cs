using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerDynamic : MonoBehaviour, InputMaster.IPlayerActions
{
    public float Speed;
    public float MaxSpeed;
    public float JumpForce;
    public float MaxFallSpeed;
    public float Deceleration;
    Rigidbody2D m_body;
    BoxCollider2D m_collider;
    Vector2 m_previousPosition;
    Vector2 m_currentPosition;
    Vector2 m_nextMovement;
    Vector2 m_movement;
    protected Vector2 m_groundNormal;
    protected ContactFilter2D m_contactFilter;
    protected RaycastHit2D[] m_hitBuffer = new RaycastHit2D[16];
    protected float m_minGroundNormalY = .65f;
    protected float m_gravityModifier = 1;
    protected const float m_minMoveDistance = 0.001f;
    protected const float m_shellRadius = 0.01f;

    public bool IsGrounded { get; protected set; }
    public bool IsCeilinged { get; protected set; }
    public Vector2 Velocity;
    private Vector2 m_normal;
    private InputMaster m_controls;
    private Ray2D[] m_rays;

    void Awake()
    {
        m_body = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<BoxCollider2D>();
        m_rays = new Ray2D[3];
    }

    void Start()
    {
        m_contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        m_contactFilter.useLayerMask = true;
    }

    public void OnEnable()
    {
        if (m_controls == null)
        {
            m_controls = new InputMaster();
            // Tell the "gameplay" action map that we want to get told about
            // when actions get triggered.
            m_controls.Player.SetCallbacks(this);
        }
        m_controls.Player.Enable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 axis = m_controls.Player.Movement.ReadValue<Vector2>();
        Move(axis);
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if(IsGrounded)
        {
            if (context.phase == InputActionPhase.Started)
            {
                m_body.velocity = new Vector2(m_body.velocity.x, JumpForce);
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                //m_player.EndJump();
            }
        }
    }
    void FixedUpdate()
    {
        Velocity.x += m_movement.x * Speed * Time.deltaTime;
        Velocity.y = m_body.velocity.y;

        if (Velocity.x >= MaxSpeed)
            Velocity.x = MaxSpeed;
        else if (Velocity.x <= -MaxSpeed)
            Velocity.x = -MaxSpeed;

        if (m_movement.x == 0)
        {
            if (Velocity.x > 0)
                Velocity.x -= Deceleration * Time.deltaTime;
            else if (Velocity.x < 0)
                Velocity.x += Deceleration * Time.deltaTime;

            if (Velocity.x >= -1 && Velocity.x <= 1)
                Velocity.x = 0;
        }

        if (m_body.velocity.y < -MaxFallSpeed)
        {
            m_body.velocity = new Vector2(m_body.velocity.x, -MaxFallSpeed);
        }

        Vector2 pos = m_body.transform.position;
        Vector2 next = pos + new Vector2(Velocity.x, Velocity.y);
        Vector2 move = (next - pos) * Time.deltaTime;
        CalculateCollisons(move);
        CheckFeet();
        m_body.velocity = new Vector2(Velocity.x, Velocity.y);
    }

    void Move(Vector2 axis)
    {
        m_movement = axis;
    }
    void CheckFeet()
    {
        IsGrounded = false;
        float feetOffsetY = m_collider.bounds.extents.y + m_collider.offset.y + (2.0f - m_collider.size.y) + m_shellRadius;
        Vector2 feet = new Vector2(m_body.position.x, m_body.position.y - feetOffsetY);
        var distance = 2.0f;
        m_rays[0] = new Ray2D(feet, Vector2.down);
        feet.x = m_body.position.x - m_collider.bounds.extents.x;
        m_rays[1] = new Ray2D(feet, Vector2.down);
        feet.x = m_body.position.x + m_collider.bounds.extents.x;
        m_rays[2] = new Ray2D(feet, Vector2.down);
        for (int r = 0; r < 3; r++)
        {
            int count = Physics2D.Raycast(m_rays[r].origin, m_rays[r].direction, m_contactFilter, m_hitBuffer, distance);
            for (int i = 0; i < count; i++)
            {
                float dist = Vector2.Distance(m_rays[r].origin, m_hitBuffer[i].point) - m_shellRadius;
                if (dist <= 2.0f)
                {
                    Debug.DrawLine(m_rays[r].origin, m_hitBuffer[i].point, Color.red);
                }

                distance = dist < distance ? dist : distance;
            }

            if (distance <= 0.4f)
            {
                IsGrounded = true;
            }

            if (count == 0)
            {
                Debug.DrawRay(m_rays[r].origin, Vector2.down, Color.green);
            }
        }
    }

    void CalculateCollisons(Vector2 move)
    {
        var distance = move.magnitude;
        var count = m_body.Cast(move, m_contactFilter, m_hitBuffer, distance + m_shellRadius);
        for (var i = 0; i < count; i++)
        {
            var currentNormal = m_hitBuffer[i].normal;

            if (currentNormal.x != 0)
            {
                distance = m_hitBuffer[i].distance - m_shellRadius;
                Velocity.x += Mathf.Abs(Velocity.x) * currentNormal.x;
            }
            if (IsGrounded)
            {
                //how much of our velocity aligns with surface normal?
                var projection = Vector2.Dot(Velocity, currentNormal);
                if(projection < 0 && System.Math.Round(currentNormal.x, 2) != 0 && System.Math.Round(currentNormal.y, 2) != 0)
                {
                    Velocity = new Vector2(-currentNormal.x * MaxSpeed * 10, 0);
                    //m_body.transform.position = new Vector2(m_body.position.x, m_hitBuffer[i].point.y + m_collider.bounds.extents.y);
                    Debug.Log(currentNormal);
                }
            }

            Debug.DrawLine(m_hitBuffer[i].point, m_hitBuffer[i].point + m_hitBuffer[i].normal * m_hitBuffer[i].distance, Color.red);                   
        }
    }
}