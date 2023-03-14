using AQEngine.Level;
using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace AQEngine.Editor
{
    [CustomEditor(typeof(SetExitPoint), true)]
    public class SetExitPointEditor : UEditor
    {
        private SetExitPoint _setExitPoint;

        private void Awake()
        {
            _setExitPoint = (SetExitPoint)serializedObject.targetObject;
        }
        public override void OnInspectorGUI()
        {

            if (GUILayout.Button("Save Exit Point"))
            {
                _setExitPoint.GetExitPoint();
                _setExitPoint.IsSaved = true;
            }
        }
    }
}