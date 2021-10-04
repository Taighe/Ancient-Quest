using Assets.Scripts.Events;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(AnimatorController))]
[RequireComponent(typeof(AudioSource))]
public class Object3D : MonoBehaviour
{
    public float DamageDelay = 0.5f;
    public bool IsInvulnerableDuringDelay = true;
    public Direction Direction;
    public Vector3 CollisionBounds { get; set; }

    public virtual Vector2 Velocity
    {
        get
        {
            return _velocity;
        }
    }

    public bool FlashDuringDelay 
    {
        get
        {
            return FlashRate > 0;
        }
    }
    public float FlashRate = 0.2f;
    public bool IsInvulnerable
    {
        get
        {
            return IsInvulnerableDuringDelay && !DamageFinished;
        }
    }
    private bool _isFlashing;
    public bool DamageFinished
    {
        get
        {
            return _time == -1 || _time >= DamageDelay;
        }
    }

    protected AnimatorController _animator;
    protected Vector2 _velocity;
    protected AudioSource _audioSource;
    protected float _zPos;

    private float _time = -1;


    public virtual void Awake()
    {
        _animator = GetComponent<AnimatorController>();
        _audioSource = GetComponent<AudioSource>();
        var cb = CollisionBounds;
    }

    protected virtual void OnDamaged(int damage)
    {

    }
    /// <summary>
    /// Reverses the current Direction of the object. From Left to Right, Right to Left.
    /// </summary>
    /// <returns>The current flipped Direction.</returns>
    public Direction FlipDirection()
    {
        return Direction = Direction == Direction.LEFT ? Direction.RIGHT : Direction.LEFT;
    }

    /// <summary>
    /// Reverses the current Direction of the object. From Left to Right, Right to Left.
    /// </summary>
    /// <returns>The current flipped Direction.</returns>
    public Vector3 GetDirectionVector()
    {
        return Direction == Direction.LEFT ? new Vector3(-1, 0) : new Vector3(1, 0);
    }

    public void Damaged(int damage)
    {
        if (DamageFinished)
        {
            OnDamaged(damage);

            if (DamageDelay != 0 && gameObject.activeSelf)
            {
                _time = 0;
                StartCoroutine(DamagedCoroutine());
            }
        }
    }

    public void Instance_Damaged(object sender, DamagedEventArgs e)
    {
        Damaged(e.Damage);
    }

    private IEnumerator DamagedCoroutine()
    {
        if (FlashDuringDelay && !_isFlashing)
        {
            StartCoroutine(FlashCoroutine());
        }

        while (_time < DamageDelay)
        {
            yield return new WaitForEndOfFrame();
            _time += 1.0f * Time.deltaTime;
        }

        _isFlashing = false;
    }

    private IEnumerator FlashCoroutine()
    {
        _isFlashing = true;
        bool visible = true;
        while(_isFlashing)
        {
            visible = !visible;
            Flash(visible);
            yield return new WaitForSeconds(FlashRate);
        }

        Flash(true);
    }

    protected virtual void Update()
    {
        GameUpdate();
        AnimationUpdate();
        PropertiesOverrideUpdate();
    }

    public virtual void GameUpdate()
    {
        var pos = transform.position;
        pos.x += _velocity.x * Time.deltaTime;
        pos.y += _velocity.y * Time.deltaTime;

        transform.position = pos;
    }

    public virtual void AnimationUpdate()
    {
        _animator.UpdateAnimations();
    }

    public virtual void PropertiesOverrideUpdate()
    {
        
    }

    public virtual void Move(Vector3 move)
    {
        _velocity.x = move.x;
        _velocity.y = move.y;
    }

    protected virtual void Flash(bool visible)
    {

    }

    // Start is called before the first frame update
    public virtual void Start()
    {

    }
}