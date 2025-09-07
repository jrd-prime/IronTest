using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using DefaultNamespace;
using UnityEngine;

public class UnitItem : MonoBehaviour, IDamageable
{
    public readonly Action<UnitItem> OnUnitDie = delegate { };

    [Header("Unit Personal Ref")] public GameObject visualGO;
    public AudioSource unitAudioSource;
    public SpriteRenderer spriteRenderer;
    public Collider2D unitCollider;
    public Rigidbody2D rb;
    public Health health;
    public ParticleSystem hitEffect;
    public Weapon weapon;

    private UnitConfig _unitConfig;
    private bool _isInitialized;
    private bool _isDead;
    private GameObject _currentTarget;
    private LayerMask _targetLayer;

    private EUnit _unitType;

    private bool HasTarget => _currentTarget;

    private Vector2 ForwardDirection => !spriteRenderer.flipX ? Vector2.left : Vector2.right;

    private void Awake() => Validate();

    public void Initialize(EUnit unitType, ESpawnSide spawnSide, UnitConfig config)
    {
        _unitType = unitType;
        _unitConfig = config;

        health.Init(config.hp);

        weapon.Initialize(config.Weapon, config.Bullet);

        spriteRenderer.color = unitType == EUnit.Enemy ? Color.red : Color.white;
        spriteRenderer.flipX = ShouldFaceRight(spawnSide, unitType);
        SetTargetLayer(unitType);

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

        FindTarget();

        if (HasTarget)
        {
            if (_unitType != EUnit.Player)
                MoveToTarget();
            TryFire();
        }
        else
        {
            if (_unitType != EUnit.Player)
                MoveForward();
        }
    }

    private void FindTarget()
    {
        // Определение маски слоев для поиска целей
        int layerMask = _targetLayer;
        if (_unitType == EUnit.Enemy)
            layerMask |= 1 << 17;

        var hit = Physics2D.Raycast(transform.position, ForwardDirection, _unitConfig.Weapon.Range, layerMask);

        _currentTarget = hit.collider ? hit.collider.gameObject : null;
    }

    private void MoveForward() =>
        rb.MovePosition(rb.position + ForwardDirection * (_unitConfig.moveSpeed * Time.fixedDeltaTime));


    private void MoveToTarget()
    {
        if (!_currentTarget)
            return;

        var targetPosition = _currentTarget.transform.position;
        var sqrDistToTarget = (targetPosition - transform.position).sqrMagnitude;

        if (!(sqrDistToTarget > _unitConfig.stopDistance * _unitConfig.stopDistance))
            return;

        Vector2 direction = (targetPosition - transform.position).normalized;
        rb.MovePosition(rb.position + direction * (_unitConfig.moveSpeed * Time.fixedDeltaTime));
    }

    private void TryFire()
    {
        if (!HasTarget || !_currentTarget)
            return;

        var distanceToTarget = Vector2.Distance(transform.position, _currentTarget.transform.position);

        if (distanceToTarget > _unitConfig.Weapon.Range)
            return;

        var damageable = _currentTarget.GetComponent<IDamageable>();
        if (damageable == null)
            return;

        int layerMask = _targetLayer;
        if (_unitType == EUnit.Enemy)
            layerMask |= 1 << 17;

        try
        {
            _ = weapon.TryFireAsync(ForwardDirection, layerMask, ForwardDirection);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message, this);
        }
    }

    public void TakeDamage(float damage)
    {
        health.DoDamage(damage);
        hitEffect.Play();
    }

    private void SetTargetLayer(EUnit unitType)
    {
        _targetLayer = unitType == EUnit.Enemy ? SomeConst.PlayerLayerMask : SomeConst.EnemyLayerMask;
        gameObject.layer = unitType == EUnit.Enemy ? SomeConst.EnemyLayerIndex : SomeConst.PlayerLayerIndex;
    }

    private static bool ShouldFaceRight(ESpawnSide spawnSide, EUnit unitType) =>
        unitType == EUnit.Player ? spawnSide == ESpawnSide.Right : spawnSide == ESpawnSide.Left;


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

        if (messages.Count <= 0)
            return;

        Debug.LogError(string.Join("\n", messages), this);
        enabled = false;
    }
}
