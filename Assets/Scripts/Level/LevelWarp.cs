using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelWarp : MonoBehaviour
{
    public float Width = 3;
    public float Height = 3;
    public bool ActivatedByButton;
#if UNITY_EDITOR
    public SceneAsset ToScene;
#endif
    [HideInInspector]
    public string ToSceneName;

    public float TransitionTime = 1.0f;
    public float TransitionDelayTime = 0.1f;
    [HideInInspector]
    public Vector3 ExitPoint;
    public Direction ExitDirection;

    private Player _player;
    private Camera _effects;
    private bool _isWarping;
    private float _transitionCount;
    private Material _material;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        _effects = GameObject.Find("Effects")?.GetComponent<Camera>();
        _material = new Material(Shader.Find("Custom/shader_fade"));
    }

    private bool PointWithinBox(Vector2 point, float width, float height)
    {
        var pos = transform.position;
        if (point.x >= pos.x && point.x <= pos.x + width)
        {
            if (point.y >= pos.y && point.y <= pos.y + height)
            {
                return true;
            }
        }

        return false;
    }

    private void WarpToScene()
    {
        if(!string.IsNullOrEmpty(ToSceneName) && !_isWarping)
        {
            StartCoroutine(TransistionToSceneAsync());
            _isWarping = true;     
        }
    }

    IEnumerator TransistionToSceneAsync()
    {
        Time.timeScale = 0;
        var ui = GameUI.GetInstance();
        StartCoroutine(ui.TransitionFade(TransitionTime));
        while (!ui.IsTransitionDone)
        {
            yield return null;
        }

        // Simulate extra delay
        yield return new WaitForSecondsRealtime(TransitionDelayTime);

        var asyncLoad = SceneManager.LoadSceneAsync(ToSceneName);
        _player.WarpToPointNextScene(ExitPoint, ExitDirection);

        while (!asyncLoad.isDone)
        {
            LevelProperties.GetInstance().WarpLevelProperties();
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_player != null)
        {
            if (PointWithinBox(_player.transform.position, Width, Height))
            {
                if(ActivatedByButton && ControllerMaster.Input.GetAxis().y == 1 && _player.IsGrounded)
                {
                    WarpToScene();
                }
                else if (!ActivatedByButton)
                {
                    WarpToScene();
                }
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Vector3 botL = transform.position;
        float z = botL.z;
        Vector3 botR = new Vector3(botL.x + Width, botL.y, z);
        Vector3 topL = new Vector3(botL.x, botL.y + Height, z);
        Vector3 topR = new Vector3(botL.x + Width, botL.y + Height, z);
        Handles.color = Color.red;
        Handles.DrawLine(botL, botR);
        Handles.DrawLine(botR, topR);
        Handles.DrawLine(topL, topR);
        Handles.DrawLine(topL, botL);
    }
#endif
}
