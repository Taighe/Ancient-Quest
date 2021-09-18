using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Direction
{
    LEFT = -180,
    RIGHT = 0
}

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class KinematicObject3D : MonoBehaviour
{
    public ActorData Data;
    public Direction Direction;
    public Vector2 Velocity 
    {
        get
        {
            if (_cController.isGrounded && _velocity.y < 0)
                _velocity.y = 0;

            return _velocity;
        } 
    }

    public Vector3 Feet
    {
        get
        {
            return new Vector3(transform.position.x, transform.position.y - _cController.bounds.size.y / 2);
        }
    }

    protected CharacterController _cController;
    protected AudioSource _audioSource;
    protected Vector2 _velocity;
    protected float _idleTime;
    protected bool _drawGizmos;
    protected float _zPos;

    public virtual void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _cController = GetComponent<CharacterController>();

        if (Data == null)
            Data = new ActorData();

        _zPos = transform.position.z;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        _drawGizmos = true;
        _velocity = Velocity;
        // Horizontal Movement
        _cController.Move(new Vector3(_velocity.x, 0) * Time.deltaTime);

        // Vertical Movement (Gravity, Jumping)
        _velocity.y += (Physics.gravity.y * Data.GravityModifier) * Time.deltaTime;
        _cController.Move(new Vector3(0, _velocity.y) * Time.deltaTime);

        if(_velocity.x > 0)
            Direction = Direction.RIGHT;
        else if(_velocity.x < 0)
            Direction = Direction.LEFT;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, (float)Direction, 0), Data.TurnSpeed * Time.deltaTime); 

        if (Velocity.x == 0 & Velocity.y == 0)
        {
            _idleTime += 1.0f * Time.deltaTime;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, _zPos);
    }

    public void SetDirectionInstant(Direction direction)
    {
        Direction = direction;
        transform.rotation = Quaternion.Euler(0, (float)Direction, 0);
    }

    public bool IsGrounded()
    {
        return _cController.isGrounded;
    }

    public bool CanFidget()
    {
        bool fidget = Data.FidgetTime > 0 && _idleTime >= Data.FidgetTime;
        _idleTime = fidget ? 0 : _idleTime; // Reset _idleTime if the object can play it's fidget animation.
        return fidget;
    }

    public void PlaySFX(AudioClip sfx)
    {
        _audioSource.clip = sfx;
        _audioSource.Play();
    }

#if UNITY_EDITOR
    public virtual void OnDrawGizmos()
    {
        transform.rotation = Quaternion.Euler(0, (float)Direction, 0);
        Handles.color = Color.green;
        Vector3 size = new Vector3(1, 1, 1);
        float dist = 1;
        Vector3 feet = transform.position - new Vector3(0, 0.5f);
        RaycastHit hit;
        if( Physics.Raycast(feet, Vector3.down, out hit, dist))
        {
            Handles.color = Color.red;
            Handles.DrawLine(feet, hit.point);
        }
        else
            Handles.DrawLine(feet, feet + Vector3.down * dist);

        //if(_drawGizmos)
        //Handles.DrawWireCube(_cController.r (transform.position - new Vector3(0, 0.5f)), size);
    }
#endif
}
