using System;
using System.Collections.Generic;
using Components;
using Data.Configs;
using Interfaces;
using UnityEngine;

namespace MonoItems
{
    public class BuildingItem : MonoBehaviour, IDamageable
    {
        public string id = "NotInitialized";
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer hpBarSprite;
        public Collider2D buildingCollider;
        public Health health;
        public Action OnBaseFall = delegate { };

        private void Awake()
        {
            Validate();
            MoveToGround();
        }

        public void Initialize(BuildingConfig baseConfig)
        {
            id = baseConfig.id;
            health.Initialize(baseConfig.maxHp);
            SetHpBarScale(health.GetPercentage());

            health.OnDie += OnDie;
            health.OnHPChange += OnHpChanged;
        }

        private void OnDie()
        {
            Debug.Log($"<color=red>BASE FALL!</color>");
            health.OnDie -= OnDie;
            health.OnHPChange -= OnHpChanged;
            OnBaseFall?.Invoke();
            Destroy(gameObject);
        }

        private void OnHpChanged() => SetHpBarScale(health.GetPercentage());

        private void SetHpBarScale(float value) => hpBarSprite.transform.localScale = new Vector3(value, 1, 1);
        
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
        
        public void TakeDamage(float damage)
        {
            health.DoDamage(damage);
            SetHpBarScale(health.GetPercentage());
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
    }
}
