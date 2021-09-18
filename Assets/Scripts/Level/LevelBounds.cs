using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelBounds: MonoBehaviour
{
    [Range(0, 9999)]
    public float Width;
    [Range(0, 9999)]
    public float Height;
    public Vector3 Min
    {
        get
        {
            return transform.position;
        }
    }

    public LevelBounds SetBounds(float width, float height)
    {
        Width = width;
        Height = height;
        return this;
    }

    private void Update()
    {
        transform.position = Vector3.zero;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        transform.position = Vector3.zero;
        Vector3 botL = transform.position;
        float z = botL.z;
        Vector3 botR = new Vector3(botL.x + Width, botL.y, z);
        Vector3 topL = new Vector3(botL.x, botL.y + Height, z);
        Vector3 topR = new Vector3(botL.x + Width, botL.y + Height, z);
        Handles.color = Color.cyan;
        Handles.DrawLine(botL, botR);
        Handles.DrawLine(botR, topR);
        Handles.DrawLine(topL, topR);
        Handles.DrawLine(topL, botL);
    }
#endif
}
