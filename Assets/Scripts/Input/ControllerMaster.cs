using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static InputMaster;

public class ControllerMaster : InputMaster.IPlayerActions
{

    private Vector2 _axis;
    private float _jump;
    private static ControllerMaster _controllerMaster;
    private InputMaster _inputMaster;
    private double _jumpStartTime;
    private double _jumpMaxDuration;
    private bool _usePressed;

    public static ControllerMaster Input 
    { 
        get
        {
            if (_controllerMaster == null)
                _controllerMaster = new ControllerMaster();
           
            return _controllerMaster;
        }
    }

    private ControllerMaster()
    {
        _inputMaster = new InputMaster();
        _inputMaster.Player.Enable();
        _inputMaster.Player.SetCallbacks(this);
    }

    public Vector2 GetAxis()
    {
        return _axis;
    }

    public bool GetJumpButton(out float duration)
    {
        duration = 0;
        if(_inputMaster.Player.Jump.phase == InputActionPhase.Started)
        {
            duration = (float)Math.Round(Time.realtimeSinceStartup - _jumpStartTime, 2) / (float)_jumpMaxDuration;
            duration = duration >= 0.95f ? Mathf.Round(duration) : duration;
        }

        return _jump > 0;
    }

    public bool GetUseButton()
    {
        if (_usePressed)
        {
            _usePressed = false;
            return true;
        }
        else
            return false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        _jump = context.ReadValue<float>();
        TapInteraction tap = (TapInteraction)context.interaction;
        _jumpMaxDuration = tap.duration;
        _jumpStartTime = context.startTime;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _axis = Vector2.zero;
        _axis.x =_inputMaster.Player.Movement.ReadValue<Vector2>().x;
        _axis.y = _inputMaster.Player.Movement.ReadValue<Vector2>().y;
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (_inputMaster.Player.Use.phase == InputActionPhase.Started)
        {
            _usePressed = context.ReadValueAsButton();
        }
    }
}
