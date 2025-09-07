using Data.Configs;
using Interfaces;
using UnityEngine;

namespace MonoItems
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class BulletItem : MonoBehaviour
    {
        [SerializeField] private float lifeTimeSec = 3f;
        private float _speed;
        private LayerMask _targetMask;
        private float _damage;
        private bool _isInitialized;
        private Vector3 _direction;
        private Collider2D _coll;

        public void Launch(BulletConfig data, Vector3 direction, LayerMask mask)
        {
            _speed = data.Speed;
            _damage = data.Damage;
            _direction = direction;
            _targetMask = mask;
            _coll = GetComponent<Collider2D>();
            _isInitialized = true;
            Invoke(nameof(DestroySelf), lifeTimeSec);
        }

        private void DestroySelf() => Destroy(gameObject);

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

            _coll.enabled = false;
            damageable.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
