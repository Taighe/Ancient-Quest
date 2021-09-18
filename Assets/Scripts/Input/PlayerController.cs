using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool ConstantJumpHeight = true;

    private Actor m_player;
    public void Awake()
    {
        m_player = GetComponent<Actor>();
    }
    public void OnEnable()
    {

    }

    public void Update()
    {
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        //float speed = ControllerMaster.Input.Player.Movement.ReadValue<Vector2>().x * m_player.Data.MaxSpeed;
       // m_player.SetHorizontalMovement(speed);
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            m_player.Jump();
        }
        else if (context.phase == InputActionPhase.Canceled && !ConstantJumpHeight)
        {
            m_player.EndJump();
        }
    }
}
