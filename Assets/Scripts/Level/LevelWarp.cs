using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Globals;

[SelectionBase]
public class LevelWarp : MonoBehaviour
{
    public float Width = 3;
    public float Height = 3;
    public bool ActivatedByButton;
    public bool UpdateCheckpoint = true;
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

    private void WarpToScene()
    {
        LevelProperties.GetInstance().TransistionToScene(TransitionTime, TransitionDelayTime, ToSceneName, ExitPoint, ExitDirection, UpdateCheckpoint);
    }

    // Update is called once per frame
    void Update()
    {
        if (_player != null)
        {
            Vector2 max = new Vector2(transform.position.x + Width, transform.position.y + Height);
            if (SharedFunctions.PointWithinBox(_player.transform.position, transform.position, max))
            {
                if(ActivatedByButton && ControllerMaster.Input.GetInteractButton() && _player.IsGrounded)
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
