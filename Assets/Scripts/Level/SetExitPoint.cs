﻿using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class SetExitPoint : MonoBehaviour
{
    [HideInInspector]
    public Vector3 ExitPoint;
    [HideInInspector]
    public bool IsSaved;
    [HideInInspector]
    public string PreviousScenePath;

    public SetExitPoint Initialize(string previousScenePath)
    {
        PreviousScenePath = previousScenePath;
        return this;
    }

    public Vector3 GetExitPoint()
    {
        ExitPoint = transform.position;
        return ExitPoint;
    }

    public IEnumerator GetSavedExitPoint()
    {
        while (!IsSaved)
        {
            yield return null;
        }
    }
}