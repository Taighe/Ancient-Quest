using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class LevelTools : EditorWindow
{
    static int _boundaryWidth;
    static int _boundaryHeight;
    static float _cameraDist;
    static float _fieldOfView;
    private static LevelSettingsData _asset;
    private static string _path = "Assets/Scripts/Editor/Tools/LevelSettings.asset";
    private static Object _playerPrefab;
    private static Object _canvasPrefab;
    private static Object _eventPrefab;
    private static Object _skyPrefab;

    private static void InitData()
    {
        string[] guids1 = AssetDatabase.FindAssets("t:LevelSettingsData");

        foreach (string guid1 in guids1)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid1);
            _asset = AssetDatabase.LoadAssetAtPath<LevelSettingsData>(path);
        }

        if (_asset == null)
        {
            _asset = CreateInstance<LevelSettingsData>();
            AssetDatabase.CreateAsset(_asset, _path);
        }

        // Init fields
        _boundaryWidth = _asset.BoundaryWidth;
        _boundaryHeight = _asset.BoundaryHeight;
        _playerPrefab = _asset.PlayerPrefab;
        _cameraDist = _asset.CameraDistance;
        _fieldOfView = _asset.FieldOfView;
        _canvasPrefab = _asset.CanvasPrefab;
        _eventPrefab = _asset.EventSystemPrefab;
        _skyPrefab = _asset.SkyPrefab;
    }

    [MenuItem("Tools/Level Tools/Level Settings")]
    public static void LevelSettings()
    {
        InitData();

        // Get existing open window or if none, make a new one:
        var window = GetWindow<LevelTools>("Level Settings");
        window.Show();
    }

    private void OnGUI()
    {
        // Update fields
        _boundaryWidth = (int)EditorGUILayout.Slider("Width", _boundaryWidth, 29, 9999);
        _boundaryHeight = (int)EditorGUILayout.Slider("Height", _boundaryHeight, 9, 9999);
        _cameraDist = EditorGUILayout.Slider("Camera Distance", _cameraDist, 0, 9999);
        _fieldOfView = EditorGUILayout.Slider("Field Of View", _fieldOfView, 0, 9999);
        _playerPrefab = EditorGUILayout.ObjectField("Player Prefab", _playerPrefab, typeof(GameObject), true);
        _canvasPrefab = EditorGUILayout.ObjectField("Canvas Prefab", _canvasPrefab, typeof(GameObject), true);
        _eventPrefab = EditorGUILayout.ObjectField("EventSystem Prefab", _eventPrefab, typeof(GameObject), true);
        _skyPrefab = EditorGUILayout.ObjectField("Sky Prefab", _skyPrefab, typeof(GameObject), true);

        if (GUILayout.Button("Save"))
        {
            SaveAsset();
        }
    }
    
    private bool SaveAsset()
    {
        if (_asset != null)
        {
            LevelSettingsData newData = CreateInstance<LevelSettingsData>();
            newData.SetLevelSettingsData(_boundaryWidth, _boundaryHeight, _cameraDist, _fieldOfView, _playerPrefab, _canvasPrefab, _eventPrefab, _skyPrefab);
            AssetDatabase.DeleteAsset(_path);
            AssetDatabase.CreateAsset(newData, _path);
            _asset = newData;
            return true;
        }
        else
        {
            InitData();
        }

        return false;
    }

    private static void UpdateDisplayProgress(string title, string info, float progress)
    {
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayProgressBar(title, info, progress);
    }

    private static T ObjectExists<T>(object obj, string message, Action actions = null)
    {
        if(obj != null)
        {
            Debug.LogWarning(message);
            if(actions != null)
            {
                actions();
            }
            return (T)obj;
        }

        return default;
    }

    private static bool AddObjectComponent<T>(string name, out T obj)
    {
        GameObject gameObj = GameObject.Find(name);
        bool noWarnings = true;
        if (gameObj != null)
        {
            Debug.LogWarning($"{gameObj.name} already exists.");
            noWarnings = false;
        }
        else
        {
            gameObj = new GameObject(name);
        }

        Component component = gameObj.GetComponent(typeof(T));

        if (component != null)
        {
            Debug.LogWarning($"{typeof(T).Name} component already exists.");
            noWarnings = false;
        }
        else
        {
            component = gameObj.AddComponent(typeof(T));
        }

        obj = component.gameObject.GetComponent<T>();

        return noWarnings;
    }

    [MenuItem("Tools/Level Tools/Setup Level")]
    public static void SetupLevel()
    {
        try
        {
            // Display progress for setting up level elements/components.
            string title = "Setup Level Progress";
            string info = "Please wait...";
            float progress = 0;
            EditorUtility.DisplayProgressBar(title, info, progress);
            InitData();
            UpdateDisplayProgress(title, info, 0.1f);

            // Add Camera Component
            var cam = Camera.main;
            if (cam == null)
                throw new Exception("Camera with MainCamera tag does not exist, please create one.");

            cam.transform.position = new Vector3(0, 0, -_cameraDist);
            cam.fieldOfView = _fieldOfView;

            FollowCamera followCam = cam.gameObject.GetComponent<FollowCamera>();
            followCam = followCam != null ? ObjectExists<FollowCamera>(followCam, "FollowCamera component already exists.") : cam.gameObject.AddComponent<FollowCamera>();
            
            UpdateDisplayProgress(title, info, 0.2f);

            // Add Sky object to main camera.
            GameObject sky = GameObject.Find("Sky");
            sky = sky != null ? ObjectExists<GameObject>(sky, "Sky already exists in scene.") : (GameObject)PrefabUtility.InstantiatePrefab(_skyPrefab);
            sky.transform.SetParent(cam.transform);

            UpdateDisplayProgress(title, info, 0.3f);

            // Create Player Object
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player = player != null ? ObjectExists<GameObject>(player, "Player already exists in scene.") : (GameObject)PrefabUtility.InstantiatePrefab(_playerPrefab);
            followCam.Player = player;
            
            UpdateDisplayProgress(title, info, 0.5f);

            // Add Level Object
            LevelProperties level;  AddObjectComponent<LevelProperties>("Level", out level);
            UpdateDisplayProgress(title, info, 0.6f);

            // Add Level Bounds
            LevelBounds bounds; 
            if(AddObjectComponent<LevelBounds>("LevelBounds", out bounds))
            {
                // Only setup LevelBounds properties if it did not exist before.
                bounds.SetBounds(_boundaryWidth, _boundaryHeight);
            }
            followCam.CameraBounds = bounds;

            UpdateDisplayProgress(title, info, 0.7f);

            // Add GameUI from prefab
            GameObject ui = GameObject.FindGameObjectWithTag("UI");
            ui = ui != null ? ObjectExists<GameObject>(ui, "GameUI already exists in scene.") : (GameObject)PrefabUtility.InstantiatePrefab(_canvasPrefab);
            
            UpdateDisplayProgress(title, info, 0.8f);

            // Add EventSystem from prefab
            GameObject eSystem = GameObject.Find("EventSystem");
            eSystem = eSystem != null ? ObjectExists<GameObject>(eSystem, "EventSystem already exists in scene.") : (GameObject)PrefabUtility.InstantiatePrefab(_eventPrefab);

            UpdateDisplayProgress(title, info, 1.0f);

            EditorUtility.ClearProgressBar();
            Debug.Log("Setup Level complete.");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError("Setup Level failed due to exception: " + e.Message);
            Debug.LogException(e);
        }
    }

#if UNITY_EDITOR

    [MenuItem("Tools/Level Tools/Unload Assets")]
    static void UnloadAssets()
    {
        Resources.UnloadUnusedAssets();
    }

#endif

}
