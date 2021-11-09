using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public bool NoFaceDirection { get; set; }
    public void StartNoFaceAnimEvent()
    {
        NoFaceDirection = true;
    }

    public void EndNoFaceAnimEvent()
    {
        NoFaceDirection = false;
    }
}
