using Assets.Scripts.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingPellet : Object3D, IInstanceObject
{
    public float Speed;
    public float ImpactForce;
    public float ResetDelay;
    public new Vector3 CollisionBounds = new Vector3(1,1,1);
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Vector3 _positionOrigin;
    private Quaternion _rotationOrigin;
    private Vector3 _moveDirection = Vector3.right;
    public int ProjectileMaxInstancesAlive;
    public float ProjectileSpawnRate;
    private const float _flashDelay = 0.5f;

    public int MaxInstancesAlive { get { return ProjectileMaxInstancesAlive; } set { ProjectileMaxInstancesAlive = value; } }
    public float SpawnRate { get { return ProjectileSpawnRate; } set { ProjectileSpawnRate = value; } }

    public GameObject GameObject => gameObject;
    int _ownerID;
    public int OwnerID => _ownerID;
    IInstanceObject _prefab;
    public IInstanceObject Prefab => _prefab;

    public override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        SetRigidBodyActive(false);
        _positionOrigin = transform.position;
        _rotationOrigin = transform.rotation;
    }

    public void ResetProjectile()
    {
        StopAllCoroutines();
        ResetDamageDelay();
        PhysicsMode(false);
        transform.position = _positionOrigin;
        transform.rotation = _rotationOrigin;
    }

    public override void GameUpdate()
    {
        if(!IsRigidBodyActive())
        {
            base.CollisionBounds = CollisionBounds;
            var dir = _velocity.normalized;
            if (DetectCollisonCast(dir, 0.1f, 1 << 0))
            {
                PhysicsMode(true);
                return;
            }

            Move(_moveDirection * Speed);
            base.GameUpdate();
        }
    }

    public void PhysicsMode(bool value)
    {
        var dir = _velocity.normalized;
        if (!IsRigidBodyActive() && value)
        {
            StartCoroutine(TriggerFlashDelay());
            StartCoroutine(TriggerResetDelay());
            Death();
        }

        SetRigidBodyActive(value);
        if (IsRigidBodyActive())
        {
            var dirForce = new Vector3(-dir.x, 1, Random.Range(-1.0f, 1.0f));
            _rigidbody.AddForce(dirForce * ImpactForce, ForceMode.Impulse);
        }
    }

    IEnumerator TriggerFlashDelay()
    {
        var flashDelay = ResetDelay * _flashDelay;
        yield return new WaitForSeconds(flashDelay);
        Damaged(0);
    }

    IEnumerator TriggerResetDelay()
    {
        yield return new WaitForSeconds(ResetDelay);
        ResetProjectile();
        ResetDamageDelay();
        DeSpawn();
    }

    protected override void Flash(bool visible)
    {
        _animator.Animator.gameObject.SetActive(visible);
    }

    public bool IsRigidBodyActive()
    {
        return _rigidbody.isKinematic == false && _rigidbody.useGravity;
    }

    public void SetRigidBodyActive(bool value)
    {
        _rigidbody.isKinematic = !value;
        _rigidbody.useGravity = value;
        _collider.enabled = value;
    }

    public IInstanceObject SetInstance(int ownerID)
    {
        _ownerID = ownerID;
        var inst = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<SlingPellet>();
        inst._prefab = this;
        inst._ownerID = ownerID;
        return inst;
    }

    public void Spawn(Vector3 position, Vector3 dir)
    {
        gameObject.SetActive(true);
        _positionOrigin = position;
        _rotationOrigin = Quaternion.identity;
        ResetProjectile();
        _moveDirection = dir;
    }

    public void Death()
    {
        if (Prefab != null)
        {
            GameEvents.Instance.InstanceManager_OnDeath(this, new DeathEventArgs(this));
        }
    }

    public void DeSpawn()
    {
        if (Prefab != null)
        {
            gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        base.CollisionBounds = CollisionBounds;
        base.OnDrawGizmos();
    }
#endif
}
