using Data.Configs;
using UnityEngine;

namespace Components
{
    public class Movement : MonoBehaviour
    {
        private MovementConfig _config;
        private bool _isInitialized;
        private Rigidbody2D _rb;

        private Vector2 RbPosition => _rb.position;
        private Vector3 Position => transform.position;
        private float MoveSpeed => _config.MoveSpeed;
        private float StopDistance => _config.StopDistance;
        private bool CanExecute => _isInitialized && _config.CanMove;

        public void Initialize(MovementConfig config, Rigidbody2D rb)
        {
            _rb = rb;
            _config = config;
            _isInitialized = true;
        }

        public void MoveForward(Vector2 forwardDirection)
        {
            if (!CanExecute)
                return;
            _rb.MovePosition(RbPosition + forwardDirection * (MoveSpeed * Time.fixedDeltaTime));
        }

        public void MoveToTarget(Vector3 targetPosition)
        {
            if (!CanExecute)
                return;

            var sqrDistToTarget = (targetPosition - Position).sqrMagnitude;

            if (sqrDistToTarget <= StopDistance * StopDistance)
                return;

            Vector2 direction = (targetPosition - Position).normalized;
            _rb.MovePosition(RbPosition + direction * (MoveSpeed * Time.fixedDeltaTime));
        }
    }
}
