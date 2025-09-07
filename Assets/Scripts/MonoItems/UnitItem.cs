using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Components;
using Data;
using Data.Configs;
using Data.Enums;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonoItems
{
    public class UnitItem : MonoBehaviour, IDamageable
    {
        public readonly Action<UnitItem> OnUnitDie = delegate { };

        [Header("Unit Personal Ref")] public GameObject visualGO;
        public AudioSource unitAudioSource;
        public SpriteRenderer spriteRenderer;
        public Collider2D unitCollider;
        public Rigidbody2D rb;
        public ParticleSystem hitEffect;
        public Weapon weapon;
        public Health health;
        public Movement movement;
        public SharpEye sharpEye;

        private UnitConfig _unitConfig;
        private bool _isInitialized;
        private bool _isDead;
        private GameObject _currentTarget;
        private LayerMask _targetLayer;
        private EUnit _unitType;

        private bool HasTarget => _currentTarget;
        private bool CanMove => _unitConfig.Movement.CanMove;
        private Vector2 ForwardDirection => spriteRenderer.flipX ? Vector2.left : Vector2.right;
        private Vector3 TargetPosition => _currentTarget.transform.position;

        private void Awake() => Validate();

        public void Initialize(EUnit unitType, ESpawnSide spawnSide, UnitConfig config)
        {
            _unitType = unitType;
            _unitConfig = config;

            if (_unitConfig.Movement.StopDistance > _unitConfig.Weapon.Range)
            {
                Debug.LogError(
                    $"StopDistance ({_unitConfig.Movement.StopDistance}) can't be greater than weapon range ({_unitConfig.Weapon.Range}). {name}",
                    this);
                enabled = false;
                return;
            }

            health.Initialize(config.hp);
            weapon.Initialize(config.Weapon, config.Bullet);
            movement.Initialize(config.Movement, rb);
            sharpEye.Initialize(unitType);

            spriteRenderer.color = unitType == EUnit.Enemy ? Color.red : Color.white;
            spriteRenderer.flipX = !(ShouldFaceRight(spawnSide, unitType) ^
                                     (_unitConfig.FaceDirection == EFaceDirection.Right));
            
            SetTargetLayer(unitType);

            name = $"{unitType}_{spawnSide}";
            Debug.Log($"<color=green>{config.spawnMessageUnit}</color>");
            var spawnSound = _unitConfig.soundSpawn;
            unitAudioSource.PlayOneShot(spawnSound);

            health.OnDie += OnDie;
            _isInitialized = true;
        }

        private void FixedUpdate()
        {
            if (!_isInitialized || _isDead)
                return;

            _currentTarget = sharpEye.FindTarget(ForwardDirection, GetTargetLayer());

            if (HasTarget)
            {
                if (CanMove)
                    movement.MoveToTarget(TargetPosition);

                weapon.Fire(GetTargetLayer(), GetDistanceToTarget(), ForwardDirection);
            }
            else
            {
                if (CanMove)
                    movement.MoveForward(ForwardDirection);
            }
        }

        public void TakeDamage(float damage)
        {
            health.DoDamage(damage);
            hitEffect.Play();
        }

        private async void OnDie()
        {
            try
            {
                _isDead = true;
                health.OnDie -= OnDie;
                var deathSound = _unitConfig.soundDeath;
                unitAudioSource.PlayOneShot(deathSound);

                await Task.Delay((int)(deathSound.length * 1000));
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message, this);
            }
            finally
            {
                OnUnitDie?.Invoke(this);
                Destroy(gameObject);
            }
        }

        public void ShiftSlightly()
        {
            var newPosition = transform.position;
            newPosition.x += Random.Range(-0.2f, 0.2f);
            transform.position = newPosition;
        }

        private void SetTargetLayer(EUnit unitType)
        {
            _targetLayer = unitType == EUnit.Enemy ? SomeConst.PlayerLayerMask : SomeConst.EnemyLayerMask;
            gameObject.layer = unitType == EUnit.Enemy ? SomeConst.EnemyLayerIndex : SomeConst.PlayerLayerIndex;
        }

        private static bool ShouldFaceRight(ESpawnSide spawnSide, EUnit unitType) =>
            unitType == EUnit.Player ? spawnSide == ESpawnSide.Right : spawnSide == ESpawnSide.Left;

        private float GetDistanceToTarget() =>
            Vector2.Distance(transform.position, _currentTarget.transform.position);

        private LayerMask GetTargetLayer()
        {
            int mask = _targetLayer;
            if (_unitType == EUnit.Enemy)
                mask |= 1 << 17;
            return mask;
        }

        private void Validate()
        {
            var messages = new List<string>();

            if (!visualGO)
                messages.Add($"{nameof(visualGO)} is null");
            if (!unitAudioSource)
                messages.Add($"{nameof(unitAudioSource)} is null");
            if (!spriteRenderer)
                messages.Add($"{nameof(spriteRenderer)} is null");
            if (!unitCollider)
                messages.Add($"{nameof(unitCollider)} is null");
            if (!rb)
                messages.Add($"{nameof(rb)} is null");
            if (!health)
                messages.Add($"{nameof(health)} is null");
            if (!hitEffect)
                messages.Add($"{nameof(hitEffect)} is null");
            if (!weapon)
                messages.Add($"{nameof(weapon)} is null");
            if (!movement)
                messages.Add($"{nameof(movement)} is null");
            if (!sharpEye)
                messages.Add($"{nameof(sharpEye)} is null");

            if (messages.Count <= 0)
                return;

            Debug.LogError(string.Join("\n", messages), this);
            enabled = false;
        }
    }
}
