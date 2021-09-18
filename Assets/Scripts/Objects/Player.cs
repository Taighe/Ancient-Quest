using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player : KinematicObject3D
{
    private PlayerData _playerData;

    public override void Awake()
    {
        if(Data != null)
        {
            _playerData = (PlayerData)Data;
        }

        base.Awake();
    }

    public override void Start()
    {
        Debug.Log("Player Start " + _playerData.IsWarping);
        AfterWarp();
    }

    void AfterWarp()
    {
        if (_playerData.IsWarping)
        {
            _cController.enabled = false;
            transform.position = _playerData.WarpPoint;
            _cController.transform.position = transform.position;
            SetDirectionInstant(_playerData.ExitDirection);
            _velocity.x = 0;
            _cController.enabled = true;
            _playerData.IsWarping = false;
        }
    }
    public override void Update()
    {
        bool isGrounded = _cController.isGrounded;
        _velocity = Velocity;

        // Horizontal Movement
        _velocity.x = ControllerMaster.Input.GetAxis().x * Data.Speed;

        float duration = 0;
        // Jumping
        if (ControllerMaster.Input.GetJumpButton(out duration) && isGrounded)
        {
            Vector3 pos = transform.position;
            StartCoroutine(Jump(duration, pos.y + Data.MinJumpHeight, pos.y + Data.MaxJumpHeight));
            PlaySFX(_playerData.JumpSfx);
        }

        base.Update();
    }

    public void WarpToPointNextScene(Vector3 point, Direction direction)
    {
        _playerData.WarpPoint = point;
        _playerData.ExitDirection = direction;
        _playerData.IsWarping = true;
    }

    IEnumerator Jump(float duration, float minPosY, float maxPosY)
    {
        bool varJumpHeight = Data.MinJumpHeight != Data.MaxJumpHeight;
        do
        {
            Vector3 pos = transform.position;
            float minMaxY = Mathf.Lerp(minPosY, maxPosY, duration);
            float diffY = Mathf.Abs(minMaxY - pos.y);
            float deltaY = diffY * Time.deltaTime;
            float moveY = Mathf.Sqrt(diffY * (-3.0f * Data.GravityModifier) * Physics.gravity.y);
            _velocity.y = moveY;
            yield return new WaitForEndOfFrame();

        } while (ControllerMaster.Input.GetJumpButton(out duration) && varJumpHeight);
    }
#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        if(_drawGizmos)
        {
            Handles.color = Color.blue;
            Handles.DrawLine(Feet, Feet + Vector3.up * Data.MaxJumpHeight);

            Handles.color = Color.cyan;
            Handles.DrawLine(Feet, Feet + Vector3.up * Data.MinJumpHeight);
        }
        transform.rotation = Quaternion.Euler(0, (float)Direction, 0);

    }
#endif
}
