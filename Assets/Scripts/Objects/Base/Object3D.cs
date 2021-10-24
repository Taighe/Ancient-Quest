using Assets.Scripts.Events;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using Assets.Scripts.Globals;

[RequireComponent(typeof(AnimatorController))]
[RequireComponent(typeof(AudioSource))]
public class Object3D : MonoBehaviour
{
    private bool _alwaysActive;
    public bool AlwaysActive
    {
        get
        {
            return _alwaysActive;
        }
    }
    public virtual bool IsAlive => true;
    [Range(-1, 99)]
    public int SpawnInstanceOnDeathIndex = -1;
    public float DamageDelay = 0.5f;
    public bool IsInvulnerableDuringDelay = true;
    protected Collider _collider;
    public bool CanSpawnInstance 
    {
        get
        {
            if(InstanceObjects.Count > 0)
            {
                return _spawnRateTimer == -1 || _spawnRateTimer >= _spawnRate;
            }

            return false;
        }
    }
    public Direction Direction;
    public Vector3 CollisionBounds { get; set; }
    [Header("Projectiles/Spawner")]
    public List<GameObject> InstanceObjects;
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

    public float DeathDelay;

    protected AnimatorController _animator;
    protected Vector2 _velocity;
    protected AudioSource _audioSource;
    protected float _zPos;
    protected float _spawnRateTimer = -1;
    protected float _spawnRate;
    private Vector3 _origin;
    private float _time = -1;
    private bool _isDying;
    private int _originLayer;

    public virtual bool SpawnInstance(int ownerID, int index, Vector3 origin, Vector3 dir, float spawnRate = 0)
    {
        if (CanSpawnInstance)
        {
            var inst = InstanceManager.Instance.SpawnInstance(ownerID, InstanceObjects[index], origin, dir);
            if(inst != null)
            {
                _spawnRate = spawnRate != 0 ? spawnRate : inst.SpawnRate;
                _spawnRateTimer = 0;
            }

            return true;
        }

        return false;
    }

    public void OnEnable()
    {
        transform.position = _origin;
    }

    public virtual void DisableObject(bool disable)
    {
        _animator.enabled = !disable;
        _animator.Animator.gameObject.SetActive(!disable);
        _audioSource.enabled = !disable;
        enabled = !disable;
        _collider.enabled = true;
    }

    public virtual void Awake()
    {
        _spawnRateTimer = -1;
        _alwaysActive = GetComponent<ActiveObjectByCameraModifier>() != null ? false : true;
        _animator = GetComponent<AnimatorController>();
        _audioSource = GetComponent<AudioSource>();
        _collider = GetComponent<Collider>();

        if (InstanceObjects.Count > 0)
            InstanceManager.Instance.AddInstancePrefabs(gameObject.GetInstanceID(), InstanceObjects);

        _origin = transform.position;
    }

    public void ResetDamageDelay()
    {
        _time = -1;
        _isFlashing = false;
        Flash(true);
    }

    public virtual void TriggerDeath()
    {
        if (!_isDying)
        {
            StartCoroutine(DeathCoroutine());
            _isDying = true;
        }
    }

    public virtual void OnDeathSpawn()
    {

    }

    public virtual void OnDeath()
    {
        gameObject.layer = (int)LayersIndex.NoCollision;
    }

    protected IEnumerator DeathCoroutine()
    {
        OnDeath();
        if(CanSpawnInstance && SpawnInstanceOnDeathIndex >= 0)
        {
            OnDeathSpawn();
        }
        yield return new WaitForSeconds(DeathDelay);
        gameObject.SetActive(false);
    }

    protected float IncreaseTimer(float timer, float min, float max)
    {
        return timer >= min && timer <= max ? 1 * Time.deltaTime : 0;
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
            if(damage > 0)
                OnDamaged(damage);

            if (DamageDelay != 0 && gameObject.activeSelf)
            {
                _time = 0;
                StartCoroutine(DamagedCoroutine());
            }
        }
    }

    public virtual void Instance_Damaged(object sender, DamagedEventArgs e)
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

    }

    protected virtual void FixedUpdate()
    {
        GameUpdate();
        AnimationUpdate();
        PropertiesOverrideUpdate();
    }

    protected bool NotActiveWhenFarFromCamera()
    {
        if (!LevelProperties.GetInstance().CloseToCamera(transform.position) && !AlwaysActive)
        {
            DisableObject(true);
            return true;
        }

        return false;
    }

    public virtual void GameUpdate()
    {
        if (NotActiveWhenFarFromCamera() || !IsAlive)
            return;

        _spawnRateTimer += IncreaseTimer(_spawnRateTimer, 0, _spawnRate);
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
        _originLayer = gameObject.layer;
    }

    public bool DetectCollisonCast(int instanceID, Vector3 dir, float distance, int layerMask, out RaycastHit hitInfo)
    {
        if(Physics.BoxCast(transform.position, CollisionBounds * 0.5f, dir, out hitInfo, transform.rotation, distance, layerMask))
        {
            if (hitInfo.collider.gameObject.GetInstanceID() != instanceID)
                return true;
        }

        return false;
    }

    public bool DetectCollisonCast(int instanceID, int layerMask, out Collider hit)
    {
        var colliders = Physics.OverlapBox(transform.position, CollisionBounds * 0.5f, transform.rotation, layerMask);
        if (colliders.Length > 0)
        {
            hit = colliders[0];
            if(hit.gameObject.gameObject.GetInstanceID() != instanceID)
                return true;
        }

        hit = null;
        return false;
    }

#if UNITY_EDITOR
    public virtual void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawWireCube(transform.position, CollisionBounds);
    }
#endif
}