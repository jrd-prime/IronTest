using System;
using UnityEngine;

namespace DefaultNamespace
{
    public sealed class BulletItem : MonoBehaviour
    {
        private float speed = 10f; // Скорость снаряда
        private LayerMask targetMask; // Маска слоёв для целей
        private float damage = 1f;
        private bool _isInitialized;
        private Vector3 _direction;

        public void Fire(BulletData data, Vector3 direction, LayerMask mask)
        {
            speed = data.Speed;
            damage = data.Damage;
            _direction = direction;
            targetMask = mask;

            _isInitialized = true;
        }

        private void FixedUpdate()
        {
            if (!_isInitialized)
                return;
            transform.position += _direction * (speed * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"<color=green>Collision with {other.gameObject.name}</color>");

            if ((targetMask.value & (1 << other.gameObject.layer)) <= 0)
                return;

            var damageable = other.GetComponent<IDamageable>();
            if (damageable == null)
                return;

            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
