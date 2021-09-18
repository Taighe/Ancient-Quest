using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PassengerData
{
    public Transform transform;
    public Vector2 velocity;
    public bool standingOnPlatform;
    public bool moveBefore;

    public PassengerData(Transform trans, Vector2 vel, bool stand, bool move)
    {
        transform = trans;
        velocity = vel;
        standingOnPlatform = stand;
        moveBefore = move;
    }
}

public class LinearMover : RaycastController
{
    public float Speed;
    public Transform Point1;
    public Transform Point2;
    public LayerMask PassengerMask;
    public Vector2 Velocity;
    
    private Rigidbody2D m_body;
    private List<PassengerData> m_passData;
    private Dictionary<Transform, PhysicsObject> m_passengers;
    private float m_currentTime = 0;
    private bool m_backwards = false;
    // Start is called before the first frame update
    void Start()
    {
        m_body = GetComponent<Rigidbody2D>();
        m_passengers = new Dictionary<Transform, PhysicsObject>();
    }

    void Update()
    {
        UpdateRayCastOrigins();
        Vector2 pos = transform.position;
        Vector2 vel = (Vector2.Lerp(Point1.position, Point2.position, m_currentTime / Speed) - pos);
        CalculatePassengers(vel);
        MovePassengers(true);
        transform.Translate(vel);
        MovePassengers(false);

        if (m_backwards == false)
        {
            m_currentTime += 1.0f * Time.deltaTime;
        }
        else
        {
            m_currentTime -= 1.0f * Time.deltaTime;
        }

        m_currentTime = Mathf.Clamp(m_currentTime, 0, Speed);
        if (m_currentTime >= Speed)
            m_backwards = true;
        else if (m_currentTime <= 0)
            m_backwards = false;
    }
    void MovePassengers(bool moveBefore)
    {
        foreach(PassengerData p in m_passData)
        {
            if (!m_passengers.ContainsKey(p.transform))
            {
                m_passengers.Add(p.transform, p.transform.GetComponent<PhysicsObject>());
            }

            if (p.moveBefore == moveBefore)
            {
                if (p.standingOnPlatform && p.velocity.y < 0)
                {
                    m_passengers[p.transform].transform.Translate(p.velocity);
                }
                else
                {
                    m_passengers[p.transform].Move(p.velocity, p.standingOnPlatform);
                }
            }
        }
    }

    void MovePassengers()
    {
        foreach (PassengerData p in m_passData)
        {
            Vector2 pos = p.transform.position;
            p.transform.Translate(p.velocity);
        }
    }
    void CalculatePassengers(Vector2 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        m_passData = new List<PassengerData>();
        float dirX = Mathf.Sign(velocity.x);
        float dirY = Mathf.Sign(velocity.y);

        // Vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + m_skinWidth;
            for (int i = 0; i < VerticalRayCount; i++)
            {
                Vector2 rayOrigin = (dirY == -1) ? m_rayOrigins.bottomLeft : m_rayOrigins.topLeft;
                rayOrigin += Vector2.right * (m_vRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, PassengerMask);
                if(hit)
                {
                    if(!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (dirY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - m_skinWidth) * dirY;
                        m_passData.Add(new PassengerData(hit.transform, new Vector2(pushX, pushY), dirY == 1, true));
                    }

                }
            }
        }

        // Horizontal moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + m_skinWidth;
            for (int i = 0; i < HorizontalRayCount; i++)
            {
                Vector2 rayOrigin = (dirX == -1) ? m_rayOrigins.bottomLeft : m_rayOrigins.bottomRight;
                rayOrigin += Vector2.up * (m_hRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, PassengerMask);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - m_skinWidth) * dirX;
                        float pushY = 0;
                        m_passData.Add(new PassengerData(hit.transform, new Vector2(pushX, pushY), false, true));
                    }

                }
            }
        }

        // Passenger is on top of a horizontal or downward moving platform
        if(dirY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = m_skinWidth * 2;
            for (int i = 0; i < VerticalRayCount; i++)
            {
                Vector2 rayOrigin = m_rayOrigins.topLeft + Vector2.right * (m_vRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, PassengerMask);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        m_passData.Add(new PassengerData(hit.transform, new Vector2(pushX, pushY), true, false));
                    }

                }
            }
        }
    }
}
