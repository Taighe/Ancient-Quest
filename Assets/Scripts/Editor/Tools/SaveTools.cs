using AQEngine.Data;
using AQEngine.Globals;
using Assets.Scripts.Managers;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AQEngine.Editor.Tools
{
    public class SaveTools : EditorWindow
    {
        private int _slot;
        private int _hp;
        private int _hpMax;
        private int _lives;
        private int _score;
        private string _sceneName;
        private string _sceneDisplayName;
        private Vector3 _checkpoint;
        private Object _sceneAsset;

        private void OnGUI()
        {
            // Update fields
            _slot = EditorGUILayout.IntField("Save Slot", _slot);
            _hp = EditorGUILayout.IntField("HP", _hp);
            _hpMax = EditorGUILayout.IntField("HP Max", _hpMax);
            _lives = EditorGUILayout.IntField("Lives", _lives);
            _checkpoint = EditorGUILayout.Vector3Field("Checkpoint", _checkpoint);
            _score = EditorGUILayout.IntField("Score", _score);
            _sceneAsset = EditorGUILayout.ObjectField("Scene", _sceneAsset, typeof(SceneAsset), true);
            _sceneDisplayName = EditorGUILayout.TextField("Scene Display Name", _sceneDisplayName);

            if (GUILayout.Button("Create"))
            {
                _sceneName = _sceneAsset.name;

                SaveData data = new SaveData
                {
                    HP = _hp,
                    MaxHP = _hpMax,
                    CheckpointSceneDisplayName = _sceneDisplayName,
                    CheckpointSceneName = _sceneName,
                    CheckpointFacing = Direction.RIGHT,
                    CheckpointX = _checkpoint.x,
                    CheckpointY = _checkpoint.y,
                    CheckpointZ = _checkpoint.z,
                    Lives = _lives,
                    PowerUpMask = 0,
                    Score = _score
                };

                SaveManager.Editor_SaveTest(_slot, data);
            }
        }

        [MenuItem("Tools/Save Tools/Create Save")]
        public static void CreateSave()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<SaveTools>("Create Save");
            window.Show();
        }
    }
}
