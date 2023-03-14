using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AQEngine.Events;
using AQEngine.Globals;
using AQEngine.Animators;
using AQEngine.Data;
using AQEngine.Level;
using AQEngine.Input;
using AQEngine.GUI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AQEngine.Objects.KinematicObjects
{
    [SelectionBase]
    [RequireComponent(typeof(PlayerAnimator))]
    public class Player : KinematicObject3D
    {
        [Header("Game Properties")]
        [Range(0, 9999)]
        public int HP = 1;
        [Range(0, 9999)]
        public int MaxHP;
        public bool SlingPowerUp;
        public bool SwordPowerUp;
        public bool ShieldPowerUp;

        public override bool IsAlive
        {
            get
            {
                return _playerData.HP > 0;
            }
        }

        public bool IsStandingStill
        {
            get
            {
                return _velocity.x == 0 && IsGrounded;
            }
        }

        // Crouching
        public bool IsCrouching => _isCrouching;
        public bool IsUsing => _isUsing;

        private PlayerData _playerData;
        private bool _isCrouching;
        private float _originHeight;
        private float _originOffsetY;
        private float _crouchHeight;
        private float _crouchOffsetY;
        private int _id;
        private bool _isUsing;

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (LayerHelper.LayerMask(hit.gameObject.layer) == (int)Layers.Hazard)
                GameEvents.Instance.OnHit(new DamagedEventArgs(hit.gameObject, gameObject, 0));
        }

        public override void Awake()
        {
            if (Data != null)
            {
                _playerData = (PlayerData)Data;
            }

            base.Awake();
            Init();
        }

        private void Init()
        {
            HP = _playerData.HP;
            MaxHP = _playerData.MaxHP;
            SlingPowerUp = HasPowerUp(PowerUps.Sling);
            SwordPowerUp = HasPowerUp(PowerUps.Sword);
            ShieldPowerUp = HasPowerUp(PowerUps.Shield);

            // Player Events
            GameEvents.Instance.Damaged += Instance_Damaged;
            GameEvents.Instance.Hit += Instance_Hit;
            GameEvents.Instance.Player_Collect += Instance_Player_Collect;
            GameEvents.Instance.Game_Load += Instance_Game_Load;
            GameEvents.Instance.Game_Save += Instance_Game_Save;
            _id = gameObject.GetInstanceID();
        }

        private void Instance_Game_Save(object sender, SaveEventArgs e)
        {
            e.SaveData.HP = _playerData.HP;
            e.SaveData.MaxHP = _playerData.MaxHP;
            e.SaveData.PowerUpMask = _playerData.PowerUpMask;
        }

        protected override void OnGameLoad(SaveData data)
        {
            _playerData.PowerUpMask = data.PowerUpMask;
            Editor_SetHP(data.HP);
            Editor_SetMaxHP(data.MaxHP);
            SetPosition(new Vector3(data.CheckpointX, data.CheckpointY, data.CheckpointZ), data.CheckpointFacing);
        }

        public override void Instance_Damaged(object sender, DamagedEventArgs e)
        {
            // PowerUp Shield front protection (only works if standing still, but can crouch.)
            Vector3 offset = IsCrouching ? new Vector3(0.1f, 0.1f) : new Vector3(0.1f, 0.25f);
            if (ShieldFrontProtection(offset, e.Attacker))
            {
                return;
            }

            Damaged(e.Damage);
            if (_playerData.HP <= 0)
            {
                TriggerDeath();
            }
        }

        public override void OnDeath()
        {
            _playerData.PowerUpMask = 0;
            base.OnDeath();
            _isFlashing = false;
            Flash(true);
            Move(Vector3.zero);
            _zPos = -5;
            Jump(15);
        }

        protected override void OnAfterDeathDelay()
        {
            var level = LevelProperties.GetInstance();
            level.GameData.Lives = Mathf.Clamp(level.GameData.Lives - 1, 0, 99);
            if (level.GameData.Lives > 0)
                level.RetryFromCheckPoint();
            else
                level.GameOver();
        }

        private void Instance_Player_Collect(object sender, CollectEventArgs e)
        {
            if (e.Instance.PowerUpType != PowerUps.None)
            {
                AddPowerUp(e.Instance.PowerUpType);
            }

            if (e.Instance.Lives != 0)
            {
                LevelProperties.GetInstance().AddGameDataLives(e.Instance.Lives);
            }
        }

        private void Instance_Hit(object sender, DamagedEventArgs e)
        {
            Jump(_playerData.BounceHeight);
        }

        public void Editor_SetHP(int value)
        {
            _playerData = _playerData != null ? _playerData : (PlayerData)Data;
            HP = _playerData.HP = value;
            if (Application.isPlaying)
            {
                GameGUI.GetInstance().UpdateHitPoints(_playerData.HP, _playerData.MaxHP);
            }
        }

        public void Editor_SetMaxHP(int value)
        {
            _playerData = _playerData != null ? _playerData : (PlayerData)Data;
            MaxHP = _playerData.MaxHP = value;
            if (Application.isPlaying)
            {
                GameGUI.GetInstance().UpdateHitPoints(_playerData.HP, _playerData.MaxHP);
            }
        }

        protected override void OnDamaged(int damage)
        {
            _playerData.HP -= damage;
            RemovePowerUp();
            GameGUI.GetInstance().UpdateHitPoints(_playerData.HP, _playerData.MaxHP);
        }

        public override void Start()
        {
            base.Start();
            GameGUI.GetInstance().UpdateHitPoints(_playerData.HP, _playerData.MaxHP);
            _originOffsetY = _cController.center.y;
            _originHeight = _cController.height;
            AfterWarp();
        }

        protected override void Flash(bool visible)
        {
            _animator.Animator.transform.GetChild(0).gameObject.SetActive(visible);
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

        void SetPosition(Vector3 position, Direction facing)
        {
            _cController.enabled = false;
            transform.position = position;
            _cController.transform.position = transform.position;
            SetDirectionInstant(facing);
            _velocity.x = 0;
            _cController.enabled = true;
        }

        public void AddPowerUp(PowerUps powerUp)
        {
            _playerData.PowerUpMask |= (int)powerUp;
            if (powerUp == PowerUps.Sling)
                _playerData.PowerUpMask &= ~(int)PowerUps.Sword;

            if (powerUp == PowerUps.Sword)
                _playerData.PowerUpMask &= ~(int)PowerUps.Sling;
        }

        public void Editor_RemoveAllPowerUps()
        {
            _playerData = _playerData != null ? _playerData : (PlayerData)Data;
            _playerData.PowerUpMask = 0;
        }

        public bool HasPowerUp(PowerUps powerUp)
        {
            bool check = ((byte)_playerData.PowerUpMask & (int)powerUp) != 0;
            return check;
        }

        public void Use()
        {
            if (HasPowerUp(PowerUps.Sling))
            {
                var pos = transform.position;
                Vector3 spawnOffset = _isCrouching ? new Vector3(pos.x, pos.y + 0.6f, pos.z) : new Vector3(pos.x, pos.y + 1.5f, pos.z);
                _isUsing = SpawnInstance(_id, 0, spawnOffset, GetDirectionVector());
            }
        }

        public void RemovePowerUp()
        {
            if (HasPowerUp(PowerUps.Shield))
            {
                _playerData.PowerUpMask &= ~(int)PowerUps.Shield;
                return;
            }

            _playerData.PowerUpMask = 0;
        }

        public override void GameUpdate()
        {
            if (_playerData.HP > 0)
            {
                _crouchHeight = _originHeight;
                _crouchOffsetY = _originOffsetY;
                _isUsing = false;
                bool isGrounded = IsGrounded;
                _isCrouching = false;
                _velocity = Velocity;
                var axis = ControllerMaster.Input.GetAxis();

                // Crouching
                // Make sure pressing downwards has priority over pressing sidewards while crouching
                if (axis.y < 0 && isGrounded)
                {
                    axis.x = 0;
                    _isCrouching = true;
                    _crouchHeight = _originHeight * 0.5f;
                    _crouchOffsetY = (_originOffsetY * 0.5f) - 0.01f;
                }

                // Use powerup
                if (ControllerMaster.Input.GetUseButton())
                {
                    Use();
                }

                // Horizontal Movement
                _velocity.x = axis.x * Data.Speed;

                float duration = 0;
                // Jumping
                if (ControllerMaster.Input.GetJumpButton(out duration) && isGrounded)
                {
                    Vector3 pos = transform.position;
                    StartCoroutine(Jump(duration, pos.y + Data.MinJumpHeight, pos.y + Data.MaxJumpHeight));
                }

                _cController.height = _crouchHeight;
                _cController.center = new Vector3(_cController.center.x, _crouchOffsetY, _cController.center.z);

                PlatformDetection();
            }

            base.GameUpdate();
        }

        public void PlatformDetection()
        {
            float distance = 0.25f;
            int layerMask = (int)Layers.Platform;
            bool left = false;
            bool right = false;
            RaycastHit hitLeft;
            // Left side
            Physics.Raycast(new Vector3(_collider.bounds.min.x, transform.position.y, transform.position.z), Vector3.down, out hitLeft, distance, layerMask);
            if (hitLeft.collider != null)
            {
                left = (hitLeft.collider.ClosestPointOnBounds(transform.position) - transform.position).normalized == new Vector3(0, -1, 0);
            }

            RaycastHit hitRight;
            // Right side
            Physics.Raycast(new Vector3(_collider.bounds.max.x, transform.position.y, transform.position.z), Vector3.down, out hitRight, distance, layerMask);
            if (hitRight.collider != null)
                right = (hitRight.collider.ClosestPointOnBounds(transform.position) - transform.position).normalized == new Vector3(0, -1, 0);

            bool ignoreCollision = !left && !right;
            Physics.IgnoreLayerCollision((int)LayersIndex.Platform, (int)LayersIndex.Player, ignoreCollision);
        }

        public bool ShieldFrontProtection(Vector3 offset, GameObject projectile)
        {
            if (HasPowerUp(PowerUps.Shield) && IsStandingStill)
            {
                var layerMask = LayerHelper.LayerMask(projectile.layer);
                var center = _collider.bounds.center + new Vector3(offset.x * GetDirectionVector().x, offset.y, offset.z);
                var colliders = Physics.OverlapBox(center, new Vector3(_collider.bounds.extents.x * 0.5f, _collider.bounds.extents.y * 0.5f, 1),
                    transform.rotation, layerMask, QueryTriggerInteraction.Collide);

                foreach (var collider in colliders)
                {
                    if (collider.gameObject.GetInstanceID() == projectile.GetInstanceID())
                        return true;
                }
            }

            return false;
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
                float deltaY = diffY * Time.fixedDeltaTime;
                float moveY = Mathf.Sqrt(diffY * (-3.0f * Data.GravityModifier) * Physics.gravity.y);
                _velocity.y = moveY;
                yield return new WaitForEndOfFrame();
                _audioSource.pitch = Mathf.Lerp(1.5f, 1.0f, duration);
            } while (ControllerMaster.Input.GetJumpButton(out duration) && varJumpHeight);

            PlaySFX(_playerData.JumpSfx);
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            if (_drawGizmos)
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
}
