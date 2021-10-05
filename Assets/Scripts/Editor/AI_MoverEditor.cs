using GameAI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AI_Mover)), CanEditMultipleObjects]
public class AI_MoverEditor : Editor 
{
    AI_Mover _mover;
    float _startPoint;
    float _moveRange;
    Vector3 _position;
    Vector3 _min;
    Vector3 _max;

    public void Init()
    {
        _mover = serializedObject.targetObject as AI_Mover;
        _startPoint = _mover.StartPoint;
        _position = _mover.transform.position;
        _min = _mover.Min.position;
        _max = _mover.Max.position;
        if(_max.x < _min.x)
        {
            var min = _min;
            _min = _max;
            _max = min;
        }

        _moveRange = _mover.MoveRange;
        UpdateStartPoint(_startPoint);
    }

    public void UpdateMoveRange(float range)
    {
        var halfRange = range * 0.5f;
        _mover.Min.position = _mover.transform.position - Vector3.right * halfRange;
        _mover.Max.position = _mover.transform.position + Vector3.right * halfRange;
        _mover.MoveRange = range;
        SerializedProperty prop = serializedObject.FindProperty("MoveRange");
        prop.floatValue = range;
        serializedObject.ApplyModifiedProperties();
    }

    public void UpdateStartPoint(float range)
    {
        if(_mover.MoveRange > 0)
        {
            Vector3 min = _mover.Min.position;
            Vector3 max = _mover.Max.position;
            _mover.transform.position = Vector3.Lerp(_mover.Min.position, _mover.Max.position, range);
            _mover.Min.position = min;
            _mover.Max.position = max;
            _mover.StartPoint = range;
            SerializedProperty prop = serializedObject.FindProperty("StartPoint");
            prop.floatValue = range;
            serializedObject.ApplyModifiedProperties();
        }
    }

    public override void OnInspectorGUI()
    {
        Init();
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        _moveRange = EditorGUILayout.Slider("Move Range", _moveRange, 0, 999);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateMoveRange(_moveRange);
        }

        EditorGUI.BeginChangeCheck();
        _startPoint = EditorGUILayout.Slider("Start Point", _startPoint, 0, 1);
        if (EditorGUI.EndChangeCheck() && _mover.MoveRange > 0)
        {
            UpdateStartPoint(_startPoint);
        }
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.InSelectionHierarchy)]
    static void DrawCustomGizmo(AI_Mover mover, GizmoType gizmoType)
    {
        if (mover.MoveRange > 0)
        {
            Handles.color = Color.blue;
            if(gizmoType.HasFlag(GizmoType.InSelectionHierarchy))
                Handles.color = Color.red;

            if(mover.Min != null && mover.Max != null)
            {
                Handles.DrawLine(mover.Min.position, mover.Max.position);
            }
        }
    }
}