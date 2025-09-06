using System.Collections.Generic;
using UnityEngine;

public class BuildingItem : MonoBehaviour, IDamageable
{
    public string id = "NotInitialized";
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer hpBarSprite;
    public Collider2D buildingCollider;
    public Health health;

    private void Awake()
    {
        Validate();
        MoveToGround();
    }

    public void Initialize(BuildingConfig baseConfig)
    {
        id = baseConfig.id;
        health.Init(baseConfig.maxHp);
        SetHpBarScale(health.GetPercentage());
    }

    private void SetHpBarScale(float value)
    {
        hpBarSprite.transform.localScale = new Vector3(value, 1, 1);
    }

    private void MoveToGround()
    {
        LayerMask groundMask = 1 << 16;
        var rayStart = new Vector2(transform.position.x, transform.position.y + buildingCollider.bounds.extents.y);
        var hit = Physics2D.Raycast(rayStart, Vector2.down, Mathf.Infinity, groundMask);

        if (!hit.collider)
            return;

        var offset = buildingCollider.bounds.extents.y;
        transform.position = new Vector3(transform.position.x, hit.point.y + offset, transform.position.z);
    }

    private void Validate()
    {
        var messages = new List<string>();

        if (!spriteRenderer)
            messages.Add($"{nameof(spriteRenderer)} is null");

        if (!hpBarSprite)
            messages.Add($"{nameof(hpBarSprite)} is null");

        if (!buildingCollider)
            messages.Add($"{nameof(buildingCollider)} is null");

        if (!health)
            messages.Add($"{nameof(health)} is null");

        if (messages.Count == 0)
            return;

        Debug.LogError(string.Join("\n", messages), this);
        enabled = false;
    }

    public void TakeDamage(float damage) => health.DoDamage(damage);
}
