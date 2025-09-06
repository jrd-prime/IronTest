using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using DefaultNamespace;
using UnityEngine;

public class AUnitItem : MonoBehaviour, IDamageable
{
    public Action<AUnitItem> OnUnitDie = delegate { };

    [Header("Unit Personal Ref")] public GameObject visualGO;
    public AudioSource unitAudioSource;
    public SpriteRenderer spriteRenderer;
    public Collider2D unitCollider;
    public Rigidbody2D rb;
    public Health health;
    public ParticleSystem hitEffect;
    public Weapon weapon;

    protected UnitConfig UnitConfig;

    private bool _isInitialized;
    private bool _isDead;

    private GameObject _currentTarget;
    private LayerMask _targetLayer;
    private EUnit _unitType;
    private bool _hasTarget;

    private void Awake() => Validate();

    private void FixedUpdate()
    {
        if (!_isInitialized || _isDead)
            return;

        FindTarget();

        if (_hasTarget)
        {
            MoveToTarget();
            TryFire();
        }
        else
        {
            MoveForward();
        }
    }

    private bool FindTarget()
    {
        int layerMask = _targetLayer;
        if (_unitType == EUnit.Enemy)
            layerMask |= 1 << 17;

        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, UnitConfig.range, layerMask);
        float closestDistance = float.MaxValue;
        Collider2D closestTarget = null;

        foreach (var target in targets)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        _currentTarget = closestTarget != null ? closestTarget.gameObject : null;
        _hasTarget = _currentTarget != null;
        return _hasTarget;
    }

    private void MoveForward()
    {
        if (_unitType == EUnit.Player)
            return;

        rb.MovePosition(rb.position + ForwardDirection * UnitConfig.moveSpeed * Time.fixedDeltaTime);
    }

    private void MoveToTarget()
    {
        if (_unitType == EUnit.Player)
            return;

        float distanceToTarget = Vector2.Distance(transform.position, _currentTarget.transform.position);
        if (distanceToTarget > UnitConfig.stopDistance)
        {
            Vector2 direction = (_currentTarget.transform.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * UnitConfig.moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void TryFire()
    {
        if (!_hasTarget || !_currentTarget)
            return;

        float distanceToTarget = Vector2.Distance(transform.position, _currentTarget.transform.position);
        if (distanceToTarget > UnitConfig.range)
            return;

        var damageable = _currentTarget.GetComponent<IDamageable>();
        if (damageable == null)
            return;

        int layerMask = _targetLayer;
        if (_unitType == EUnit.Enemy)
            layerMask |= 1 << 17;

        weapon.TryFire(ForwardDirection, layerMask, UnitConfig.damage);
    }

    public void TakeDamage(float unitConfigDamage)
    {
        health.DoDamage(unitConfigDamage);
        hitEffect.Play();
    }

    public async void Initialize(EUnit unitType, ESpawnSide spawnSide, UnitConfig config)
    {
        _unitType = unitType;
        UnitConfig = config;

        health.Init(config.hp);

        var weaponData = new WeaponData()
        {
            AmmoCount = config.ammoCount,
            RateFire = config.rateFire,
            Reload = config.reload
        };
        var bulletData = new BulletData()
        {
            Damage = config.damage,
            Speed = 10f
        };

        weapon.Initialize(weaponData, bulletData);

        spriteRenderer.color = unitType == EUnit.Enemy ? Color.red : Color.white;
        spriteRenderer.flipX = ShouldFaceRight(spawnSide, unitType);
        SetTargetLayer(unitType);

        Debug.Log($"<color=green>{config.spawnMessageUnit}</color>");
        var spawnSound = UnitConfig.soundSpawn;
        unitAudioSource.PlayOneShot(spawnSound);

        health.OnDie += OnDie;
        _isInitialized = true;
    }

    private void SetTargetLayer(EUnit unitType)
    {
        _targetLayer = unitType == EUnit.Enemy ? SomeConst.PlayerLayerMask : SomeConst.EnemyLayerMask;
        gameObject.layer = unitType == EUnit.Enemy ? SomeConst.EnemyLayerIndex : SomeConst.PlayerLayerIndex;
    }

    private bool ShouldFaceRight(ESpawnSide spawnSide, EUnit unitType) =>
        unitType == EUnit.Player ? spawnSide == ESpawnSide.Right : spawnSide == ESpawnSide.Left;

    public Vector2 ForwardDirection => !spriteRenderer.flipX ? Vector2.left : Vector2.right;

    public async void OnDie()
    {
        _isDead = true;
        // unitCollider.enabled = false;
        health.OnDie -= OnDie;
        var deathSound = UnitConfig.soundDeath;
        unitAudioSource.PlayOneShot(deathSound);

        await Task.Delay((int)(deathSound.length * 1000));

        OnUnitDie?.Invoke(this);
        Destroy(gameObject);
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
