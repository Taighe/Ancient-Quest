using Assets.Scripts.Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
    public Vector2 Min;
    public Vector2 Max = new Vector2(2, 2);
    public List<string> Message;
    public float MessageDisplayRate;
    public float MessageDelay;

    private Player _player;
    private GameGUI _gui;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        _gui = GameGUI.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        if (_player != null)
        {
            Vector3 min = new Vector3(transform.position.x + Min.x, transform.position.y + Min.y, 0);
            Vector3 max = new Vector3(transform.position.x + Max.x, transform.position.y + Max.y, 0);
            if (SharedFunctions.PointWithinBox(_player.transform.position, min, max))
            {
                if (ControllerMaster.Input.GetInteractButton() && _player.IsGrounded)
                {
                    _gui.StartMessage(Message.ToArray(), MessageDisplayRate, MessageDelay);
                }
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Vector3 min = new Vector3(transform.position.x + Min.x, transform.position.y + Min.y, 0);
        Vector3 max = new Vector3(transform.position.x + Max.x, transform.position.y + Max.y, 0);
        Vector3 center = new Vector3((min.x + max.x) * 0.5f, (min.y + max.y) * 0.5f);
        Handles.color = Color.red;
        Handles.DrawWireCube(center, max - min);
    }
#endif
}
