using Data.Configs;
using UnityEngine;

namespace DefaultNamespace
{
    public sealed class BulletItem : MonoBehaviour
    {
        private float _speed;
        private LayerMask _targetMask;
        private float _damage;
        private bool _isInitialized;
        private Vector3 _direction;

        public void Launch(BulletConfig data, Vector3 direction, LayerMask mask)
        {
            _speed = data.Speed;
            _damage = data.Damage;
            _direction = direction;
            _targetMask = mask;
            _isInitialized = true;
        }

        private void FixedUpdate()
        {
            if (!_isInitialized)
                return;
            transform.position += _direction * (_speed * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((_targetMask.value & (1 << other.gameObject.layer)) <= 0)
                return;

            var damageable = other.GetComponent<IDamageable>();
            if (damageable == null)
                return;

            damageable.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
