using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using UEditor = UnityEditor.Editor;

namespace AQEngine.Editor
{
    [CustomEditor(typeof(LinearMover), true)]
    public class LinearMoverEditor : UEditor
    {
        private LinearMover _mover;
        public void Awake()
        {
            _mover = (LinearMover)target;
            _mover.PointsInit();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Reset Points"))
            {
                if (_mover != null)
                {
                    _mover.PointsReset();
                }
            }
        }
    }
}