using UnityEngine;

namespace Data.Configs
{
    [CreateAssetMenu(
        fileName = nameof(MovementConfig),
        menuName = "Config/" + nameof(MovementConfig)
    )]
    public class MovementConfig : ScriptableObject
    {
        [SerializeField] private bool canMove = true;
        [Range(1, 10f)] [SerializeField] private float moveSpeed;
        [Range(1, 100f)] [SerializeField] private float stopDistance;

        public bool CanMove => canMove;
        public float MoveSpeed => moveSpeed;
        public float StopDistance => stopDistance;
    }
}
