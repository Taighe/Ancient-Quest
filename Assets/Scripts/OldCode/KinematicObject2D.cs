using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KinematicObject2D : MonoBehaviour
{
    public float Width;
    public float Height;
    public float Speed;
    public float Distance;
    private RaycastHit2D _hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        Vector2 move = ControllerMaster.Input.GetAxis() * Speed;
        pos += move * Time.deltaTime;
        transform.position = pos;
        int layer = LayerMask.NameToLayer("Terrain");
        _hit = Physics2D.BoxCast(transform.position, new Vector2(Width, Height), 0, Vector2.right, Distance);
    }

    void DebugDrawBox(Color color)
    {
        float halfWidth = Width / 2;
        float halfHeight = Height / 2;
        Vector2 pos = transform.position;

        Debug.DrawLine(new Vector2(pos.x - halfWidth, pos.y - halfHeight), new Vector2(pos.x + halfWidth, pos.y - halfHeight));
        Debug.DrawLine(new Vector2(pos.x + halfWidth, pos.y - halfHeight), new Vector2(pos.x + halfWidth, pos.y + halfHeight));
        Debug.DrawLine(new Vector2(pos.x + halfWidth, pos.y + halfHeight), new Vector2(pos.x - halfWidth, pos.y + halfHeight));
        Debug.DrawLine(new Vector2(pos.x - halfWidth, pos.y + halfHeight), new Vector2(pos.x - halfWidth, pos.y - halfHeight));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(_hit.collider != null)
        {
            RaycastHit2D hit = _hit;
            Debug.Log(hit.collider.name);
            Gizmos.DrawRay(new Ray(transform.position, transform.right * hit.distance));
            Gizmos.DrawWireCube(transform.position + transform.right * hit.distance, new Vector3(Width, Height, 1));
        }
        else
        {
            Gizmos.DrawRay(new Ray(transform.position, transform.right * Distance));
            Gizmos.DrawWireCube(transform.position + transform.right * Distance, new Vector3(Width, Height, 1));
        }
    }
}
