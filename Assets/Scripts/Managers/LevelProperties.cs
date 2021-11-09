using Assets.Scripts.Events;
using Assets.Scripts.Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class LevelProperties : SingletonObject<LevelProperties>
{
    public LevelPropertiesData Data;
    public GameData GameData;
    public string LevelName;
    public AudioClip BackgroundMusic;
    public float FadeOutTime = 0.3f;
    private AudioSource _audSource;
    private Camera _camera;
    private float _activeWidth = 35;
    private float _activeHeight = 15;
    private int _lastNumberActive;
    private Player _player;
    private bool _isWarping;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
        Data.PersistantCollectables = Data.PersistantCollectables == null ? new Dictionary<string, bool>() : Data.PersistantCollectables;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        GameGUI.GetInstance().UpdateLevelName(LevelName);
        GameGUI.GetInstance().UpdateLives(GameData.Lives);

        Application.targetFrameRate = 60;

        if (Data == null)
        {
            Data = new LevelPropertiesData();
        }

        _audSource = GetComponent<AudioSource>();
        _audSource.clip = BackgroundMusic;

        if (_audSource.clip != null)
        {
            if (string.IsNullOrEmpty(Data.PreviousBackgroundMusic) || _audSource.clip.name != Data.PreviousBackgroundMusic)
            {
                _audSource.time = 0;
            }
            else
            {
                _audSource.time = Data.PreviousAudioTime;
            }

            _audSource.Play();
            Data.PreviousBackgroundMusic = _audSource.clip.name;
            Data.PreviousAudioTime = _audSource.time;
        }
        else
        {
            Data.PreviousBackgroundMusic = null;
        }

        StartCoroutine(FadeOut());
    }

    public void AddGameDataLives(int lives)
    {
        GameData.Lives += lives;
        GameGUI.GetInstance().UpdateLives(GameData.Lives);
    }

    public void UpdateGameDataCheckpoint(Vector3 point, string sceneName, Direction facing)
    {
        GameData.Checkpoint.Position = point;
        GameData.Checkpoint.SceneName = sceneName;
        GameData.Checkpoint.Facing = facing;
    }

    private IEnumerator FadeOut()
    {        
        StartCoroutine(GameGUI.GetInstance().TransitionFade(FadeOutTime, true));
        while (!GameGUI.GetInstance().IsTransitionDone)
        {
            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = 1;
    }

    public void WarpLevelProperties()
    {
        if (_audSource.clip != null)
        {
            Data.PreviousBackgroundMusic = _audSource.clip.name;
            Data.PreviousAudioTime = _audSource.time;
        }
    }

    public void RetryFromCheckPoint()
    {
        TransistionToScene(GameData.RetryTransistionTime, GameData.RetryTransistionDelayTime, GameData.Checkpoint.SceneName, GameData.Checkpoint.Position, GameData.Checkpoint.Facing);
        StartCoroutine(RetryFromCheckPointAsync());
    }

    private IEnumerator RetryFromCheckPointAsync()
    {
        var ui = GameGUI.GetInstance();
        while (!ui.IsTransitionDone)
        {
            yield return null;
        }
        // Fully heal the player. HP is clamped to Maximum for player.
        _player.Editor_SetHP(100);
    }

    public void GameOver()
    {
        TransistionToScene(0.3f, 1, "GameOver", new Vector3(8.5f, 9.5f, 0), Direction.RIGHT, true);
        StartCoroutine(GameOverAsync());
    }

    private IEnumerator GameOverAsync()
    {
        var ui = GameGUI.GetInstance();
        while (!ui.IsTransitionDone)
        {
            yield return null;
        }
        // Fully heal the player. HP is clamped to Maximum for player.
        _player.Editor_SetHP(100);
        GameData.Lives = 3;
    }

    public void TransistionToScene(float transitionTime, float transitionDelayTime, string sceneName, Vector3 exitPoint, Direction direction = Direction.RIGHT, bool updateCheckPoint = false)
    {
        if (!string.IsNullOrEmpty(sceneName) && !_isWarping)
        {
            if (updateCheckPoint)
            {
                UpdateGameDataCheckpoint(exitPoint, sceneName, direction);
            }

            StartCoroutine(TransistionToSceneAsync(transitionTime, transitionDelayTime, sceneName, exitPoint, direction, updateCheckPoint));
            _isWarping = true;
        }
    }

    IEnumerator TransistionToSceneAsync(float transitionTime, float transitionDelayTime, string sceneName, Vector3 exitPoint, Direction direction, bool updateCheckPoint)
    {
        Time.timeScale = 0;
        var ui = GameGUI.GetInstance();
        StartCoroutine(ui.TransitionFade(transitionTime));
        while (!ui.IsTransitionDone)
        {
            yield return null;
        }

        // Simulate extra delay
        yield return new WaitForSecondsRealtime(transitionDelayTime);

        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        _player.WarpToPointNextScene(exitPoint, direction);

        while (!asyncLoad.isDone)
        {
            LevelProperties.GetInstance().WarpLevelProperties();
            yield return null;
        }
    }

    public bool CloseToCamera(Vector3 instancePosition)
    {
        var activeSize = new Vector2(_activeWidth, _activeHeight);
        var minX = _camera.transform.position.x - activeSize.x * 0.5f;
        var maxX = _camera.transform.position.x + activeSize.x * 0.5f;
        var minY = _camera.transform.position.y - activeSize.y * 0.5f;
        var maxY = _camera.transform.position.y + activeSize.y * 0.5f;

        var pos = instancePosition;
        if ((pos.x >= minX && pos.x <= maxX) && (pos.y >= minY && pos.y <= maxY))
        {
            return true;
        }

        return false;
    }

    public bool HasBeenCollected(Collectable collectable)
    {
        var key = $"{collectable.PersistantId}";
        
        if (Data.PersistantCollectables.ContainsKey(key))
        {
            return Data.PersistantCollectables[key];
        }
        else
        {
            Data.PersistantCollectables.Add(key, false);
        }

        return false;
    }

    public void UpdatePersistantCollectable(int id, bool collected)
    {
        string key = id.ToString();
        if (Data.PersistantCollectables.ContainsKey(key))
            Data.PersistantCollectables[key] = collected;
        else
            Data.PersistantCollectables.Add(key, collected);
    }

    public bool CloseToCamera(Vector3 instancePosition, Vector2 size)
    {
        var activeSize = size == Vector2.zero ? new Vector2(_activeWidth, _activeHeight) : size;
        var minX = _camera.transform.position.x - activeSize.x * 0.5f;
        var maxX = _camera.transform.position.x + activeSize.x * 0.5f;
        var minY = _camera.transform.position.y - activeSize.y * 0.5f;
        var maxY = _camera.transform.position.y + activeSize.y * 0.5f;

        var pos = instancePosition;
        if ( (pos.x >= minX && pos.x <= maxX) && (pos.y >= minY && pos.y <= maxY) )
        {
            return true;
        }

        return false;
    }

    public void Update()
    {
        int layerMask = LayerHelper.LayerMask(Layers.Object, Layers.Kinematic);
        var pos = _camera.transform.position;
        var colliders = Physics.OverlapBox(new Vector3(pos.x, pos.y, 0), new Vector3(_activeWidth * 0.5f, _activeHeight * 0.5f, 1), Quaternion.identity, layerMask, QueryTriggerInteraction.Collide);

        foreach (var collider in colliders)
        {
            GameEvents.Instance.OnActive(new ActiveEventArgs(collider.gameObject));
        }

        _lastNumberActive = colliders.Length;
    }
}
