using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(SetExitPoint), true)]
public class SetExitPointEditor : Editor 
{
    private SetExitPoint _setExitPoint;

    private void Awake()
    {
        _setExitPoint = (SetExitPoint)serializedObject.targetObject;
    }
    public override void OnInspectorGUI()
    {
        
        if(GUILayout.Button("Save Exit Point"))
        {
            _setExitPoint.GetExitPoint();
            _setExitPoint.IsSaved = true;
        }
    }
}