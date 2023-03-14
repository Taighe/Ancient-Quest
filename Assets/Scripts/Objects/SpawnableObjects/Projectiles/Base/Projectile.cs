using AQEngine.Events;
using AQEngine.Globals;
using AQEngine.Level;
using System.Collections;
using UnityEngine;

namespace AQEngine.Objects.SpawnableObjects
{
    /// <summary>
    /// Base class for creating a projectile like object. For eg. fireball, grenade etc.
    /// </summary>
    public class Projectile : Object3D, ISpawnableObject
    {
        public LayerMask CollisionMask;
        public float Speed;
        public float ImpactForce;
        public float ResetDelay;
        public bool CompareTags = false;
        public new Vector3 CollisionBounds = new Vector3(1, 1, 1);
        private Rigidbody _rigidbody;
        private Vector3 _positionOrigin;
        private Quaternion _rotationOrigin;
        private Vector3 _moveDirection = Vector3.right;
        public int ProjectileMaxInstancesAlive;
        public float ProjectileSpawnRate;
        public int ProjectileStrength;
        private const float _flashDelay = 0.5f;
        private const float _width = 19;
        private const float _height = 10;
        private bool _resetTriggered;
        public int MaxInstancesAlive { get { return ProjectileMaxInstancesAlive; } set { ProjectileMaxInstancesAlive = value; } }
        public float SpawnRate { get { return ProjectileSpawnRate; } set { ProjectileSpawnRate = value; } }

        public GameObject GameObject => gameObject;
        int _ownerID;
        public int OwnerID => _ownerID;
        private ISpawnableObject _prefab;
        public ISpawnableObject Prefab => _prefab;

        public int Strength { get { return ProjectileStrength; } set { ProjectileStrength = value; } }

        public override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
            _moveDirection = GetDirectionVector();
        }

        // Start is called before the first frame update
        public override void Start()
        {
            _positionOrigin = transform.position;
            _rotationOrigin = transform.rotation;
        }

        public void ResetProjectile()
        {
            _resetTriggered = false;
            StopAllCoroutines();
            ResetDamageDelay();
            PhysicsMode(false);
            transform.position = _positionOrigin;
            transform.rotation = _rotationOrigin;
        }

        public override void GameUpdate()
        {
            if (!LevelProperties.GetInstance().CloseToCamera(transform.position, new Vector2(_width, _height)))
            {
                Death();
                DeSpawn();
                return;
            }

            if (!IsRigidBodyActive())
            {
                base.CollisionBounds = CollisionBounds;
                var dir = _velocity.normalized;
                Collider hit;

                if (DetectCollisonCast(_ownerID, CollisionMask.value, out hit))
                {
                    int layer;
                    if (!hit.CompareTag("No Projectile"))
                    {
                        if (CompareTags)
                        {
                            if (!CompareTag(hit.tag))
                            {
                                OnHit(hit, out layer);
                            }
                        }
                        else
                        {
                            OnHit(hit, out layer);
                        }
                    }
                }

                Move(_moveDirection * Speed);
                base.GameUpdate();
            }
        }

        public virtual void OnHit(RaycastHit hit, out int layer)
        {
            layer = LayerHelper.LayerMask(hit.collider.gameObject.layer);

            if (layer == (int)Layers.Default)
            {
                PhysicsMode(true);
                return;
            }
        }

        public virtual void OnHit(Collider hit, out int layer)
        {
            layer = LayerHelper.LayerMask(hit.gameObject.layer);
            if (layer == (int)Layers.Default)
            {
                PhysicsMode(true);
                return;
            }
        }

        public void TriggerReset()
        {
            if (_resetTriggered == false)
            {
                _resetTriggered = true;
                StartCoroutine(TriggerResetDelay());
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
            _collider.isTrigger = !value;
            _collider.enabled = true;
        }

        public ISpawnableObject SetInstance(int ownerID)
        {
            _ownerID = ownerID;
            var inst = Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<Projectile>();
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
                GameEvents.Instance.InstanceManager_OnDeath(new DeathEventArgs(this));
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
}
