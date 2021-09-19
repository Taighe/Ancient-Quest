using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PlayerAnimator))]
public class Player : KinematicObject3D
{
    private PlayerData _playerData;
    private PlayerAnimator _animator;
    // Crouching
    public bool IsCrouching => _isCrouching;
    private bool _isCrouching;
    private float _originHeight;
    private float _originOffsetY;
    private float _crouchHeight;
    private float _crouchOffsetY;
    //

    public override void Awake()
    {
        if(Data != null)
        {
            _playerData = (PlayerData)Data;
        }

        _animator = GetComponent<PlayerAnimator>();
        base.Awake();
    }

    public override void Start()
    {
        _originOffsetY = _cController.center.y;
        _originHeight = _cController.height;
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
        _crouchHeight = _originHeight;
        _crouchOffsetY = _originOffsetY;

        bool isGrounded = _cController.isGrounded;
        _isCrouching = false;
        _velocity = Velocity;
        var axis = ControllerMaster.Input.GetAxis();
        // Crouching
        // Make sure pressing downwards has priority over pressing sidewards while crouching
        Debug.Log(isGrounded);
        if (axis.y < 0 && isGrounded) 
        {
            axis.x = 0;
            _isCrouching = true;
            _crouchHeight = _originHeight * 0.5f;
            _crouchOffsetY = (_originOffsetY * 0.5f) - 0.01f;
        }

        // Horizontal Movement
        _velocity.x = axis.x * Data.Speed;

        float duration = 0;
        // Jumping
        if (ControllerMaster.Input.GetJumpButton(out duration) && isGrounded)
        {
            Vector3 pos = transform.position;
            StartCoroutine(Jump(duration, pos.y + Data.MinJumpHeight, pos.y + Data.MaxJumpHeight));
            PlaySFX(_playerData.JumpSfx);
        }

        _cController.height = _crouchHeight;
        _cController.center = new Vector3(_cController.center.x, _crouchOffsetY, _cController.center.z);
        base.Update();
        // Update animation loop state machine.
        _animator.UpdateAnimations();
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
