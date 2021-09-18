using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController : MonoBehaviour
{
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
    protected RaycastOrigins m_rayOrigins;
    protected BoxCollider2D m_collider;
    protected const float m_skinWidth = .015f;
    protected float m_hRaySpacing;
    protected float m_vRaySpacing;
    public int HorizontalRayCount = 4;
    public int VerticalRayCount = 4;

    protected virtual void Awake()
    {
        m_collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    protected virtual void UpdateRayCastOrigins()
    {
        Bounds bounds = m_collider.bounds;
        bounds.Expand(m_skinWidth * -2);

        m_rayOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        m_rayOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        m_rayOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        m_rayOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    protected void CalculateRaySpacing()
    {
        Bounds bounds = m_collider.bounds;
        bounds.Expand(m_skinWidth * -2);

        HorizontalRayCount = Mathf.Clamp(HorizontalRayCount, 2, int.MaxValue);
        VerticalRayCount = Mathf.Clamp(VerticalRayCount, 2, int.MaxValue);

        m_hRaySpacing = bounds.size.y / (HorizontalRayCount - 1);
        m_vRaySpacing = bounds.size.x / (VerticalRayCount - 1);
    }

}
