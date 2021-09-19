using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : RaycastController
{
    public struct CollisonInfo
    {
        public bool above, below;
        public bool left, right;
        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAnglePrevious;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slopeAnglePrevious = slopeAngle;
            slopeAngle = 0;
        }
    }

    protected ContactFilter2D m_contactFilter;
    protected CollisonInfo m_collisonInfo;
    protected Rigidbody2D m_body;
    protected float m_maxClimbAngle;
    protected float m_maxDescendAngle;
    Vector2 m_velocityOld;

    protected virtual void Start()
    {
        m_collider = GetComponent<BoxCollider2D>();
        m_body = GetComponent<Rigidbody2D>();
        m_contactFilter.useTriggers = false;
        m_contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        m_contactFilter.useLayerMask = true;
    }

    public virtual void Update()
    {
        for(int i = 0; i < VerticalRayCount; i++)
        {
            Debug.DrawRay(m_rayOrigins.bottomLeft + Vector2.right * m_vRaySpacing * i, Vector2.up * -2, Color.green);
        }      
    }

    public Vector2 Move(Vector2 moveDelta, bool standingOnPlatform = false)
    {
        Vector2 pos = transform.position;
        UpdateRayCastOrigins();
        m_collisonInfo.Reset();
        m_velocityOld = moveDelta;
        m_collisonInfo.below = standingOnPlatform;

        if (moveDelta.y < 0)
            DescendSlope(ref moveDelta);

        if(moveDelta.x != 0)
            HorizontalCollisions(ref moveDelta);
        if(moveDelta.y != 0)
            VerticalCollisions(ref moveDelta);

        transform.Translate(moveDelta);

        return moveDelta;
    }

    void HorizontalCollisions(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + m_skinWidth;
        for (int i = 0; i < HorizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? m_rayOrigins.bottomLeft : m_rayOrigins.bottomRight;
            rayOrigin += Vector2.up * (m_hRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, m_contactFilter.layerMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength * 10, Color.red);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(i == 0 && slopeAngle <= m_maxClimbAngle)
                {
                    if (m_collisonInfo.descendingSlope)
                    {
                        m_collisonInfo.descendingSlope = false;
                        velocity = m_velocityOld;
                    }

                    float distanceToSlopeStart = 0;
                    if(slopeAngle != m_collisonInfo.slopeAnglePrevious)
                    {
                        distanceToSlopeStart = hit.distance - m_skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }
                if (m_collisonInfo.climbingSlope == false || slopeAngle > m_maxClimbAngle)
                {
                    velocity.x = (hit.distance - m_skinWidth) * directionX;
                    rayLength = hit.distance;
                    if(m_collisonInfo.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(m_collisonInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }
                    m_collisonInfo.left = directionX == -1;
                    m_collisonInfo.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + m_skinWidth;
        for(int i = 0; i < VerticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? m_rayOrigins.bottomLeft:m_rayOrigins.topLeft;
            rayOrigin += Vector2.right * (m_vRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, m_contactFilter.layerMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
            if(hit)
            {
                velocity.y = (hit.distance - m_skinWidth) * directionY;
                rayLength = hit.distance;
                
                if(m_collisonInfo.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(m_collisonInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                m_collisonInfo.below = directionY == -1;
                m_collisonInfo.above = directionY == 1;
            }
        }

        // If the Actor is climbing a slope, make sure to check if there are any slope changes and update the current collision slope angle.
        if (m_collisonInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + m_skinWidth;
            Vector2 ray = Vector2.zero;
            if (directionX == -1)
                ray = m_rayOrigins.bottomLeft + Vector2.up * velocity.y;
            else
                ray = m_rayOrigins.bottomRight + Vector2.up * velocity.y;

            RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.right * directionX, rayLength, m_contactFilter.layerMask);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != m_collisonInfo.slopeAngle)
                {
                    velocity.x = (hit.distance - m_skinWidth) * directionX;
                    m_collisonInfo.slopeAngle = slopeAngle;
                }
            }
        }
    }
    void ClimbSlope(ref Vector2 velocity, float angle)
    {
        float dist = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(angle * Mathf.Deg2Rad) * dist;
        if(velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * dist * Mathf.Sign(velocity.x);
            m_collisonInfo.below = true;
            m_collisonInfo.climbingSlope = true;
            m_collisonInfo.slopeAngle = angle;
        }
    }

    void DescendSlope(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 ray = directionX == -1 ? m_rayOrigins.bottomRight : m_rayOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.down, Mathf.Infinity, m_contactFilter.layerMask);
        if(hit)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if(angle != 0 && angle <= m_maxDescendAngle)
            {
                if(Mathf.Sign(hit.normal.x) == directionX)
                {
                    if(hit.distance - m_skinWidth <= Mathf.Tan(angle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float dist = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(angle * Mathf.Deg2Rad) * dist;
                        velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * dist * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;
                       
                        m_collisonInfo.slopeAngle = angle;
                        m_collisonInfo.descendingSlope = true;
                        m_collisonInfo.below = true;
                    }
                }
            }
        }
    }
}
