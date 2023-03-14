using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using AQEngine.Level;
using UEditor = UnityEditor.Editor;

namespace AQEngine.Editor
{
    [CustomEditor(typeof(LevelWarp), true)]
    public class LevelWarpEditor : UEditor
    {
        private LevelWarp _warp;
        private string _warpName;
        private string _previousScenePath;
        private Vector3 _exitPoint;
        private SerializedProperty _exitPointProp;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _exitPointProp = serializedObject.FindProperty("ExitPoint");
            _warp = (LevelWarp)serializedObject.targetObject;
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_exitPointProp, new GUIContent("Exit Point"));
            if (EditorGUI.EndChangeCheck())
            {
                var sceneName = serializedObject.FindProperty("ToSceneName");
                sceneName.stringValue = _warp.ToScene.name;
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button(new GUIContent("Set Point", "Set the exit point in the specified scene.")))
            {
                _exitPoint = _warp.ExitPoint;
                _previousScenePath = EditorSceneManager.GetActiveScene().path;
                _warpName = _warp.name;
                var path = AssetDatabase.GetAssetPath(_warp.ToScene);
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                Debug.Log("Scene Go");
                EditorCoroutineUtility.StartCoroutine(SetPoint(), this);
            }

            EditorGUILayout.EndHorizontal();
        }

        private IEnumerator SetPoint()
        {
            var setExitPoint = new GameObject("SetExitPoint");
            setExitPoint.transform.position = _exitPoint;
            var exit = setExitPoint.AddComponent<SetExitPoint>().Initialize(_previousScenePath);
            Selection.activeObject = setExitPoint;
            SceneView.lastActiveSceneView.FrameSelected();

            EditorCoroutineUtility.StartCoroutine(exit.GetSavedExitPoint(), this);

            while (!exit.IsSaved)
            {
                yield return null;
            }

            Vector3 savedPoint = exit.GetExitPoint();
            EditorSceneManager.OpenScene(exit.PreviousScenePath, OpenSceneMode.Single);
            LevelWarp warp = GameObject.Find(_warpName)?.GetComponent<LevelWarp>();
            if (warp != null)
            {
                SerializedObject so = new SerializedObject(warp);
                var p = so.FindProperty("ExitPoint");
                p.vector3Value = savedPoint;
                so.ApplyModifiedProperties();
                Selection.activeObject = warp;
            }
            else
                Debug.LogError($"LevelWarp: {_warpName} not found.");
        }
    }
}