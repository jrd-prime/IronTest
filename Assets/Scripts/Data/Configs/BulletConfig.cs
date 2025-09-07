using System;
using UnityEngine;

namespace Data.Configs
{
    [Serializable]
    [CreateAssetMenu(
        fileName = nameof(BulletConfig),
        menuName = "Config/" + nameof(BulletConfig)
    )]
    public sealed class BulletConfig : ScriptableObject
    {
        [Range(1, 1000)] [SerializeField] private float damage;
        [Range(1f, 30f)] [SerializeField] private float speed;

        /// <summary>
        /// Урон снаряда
        /// </summary>
        public float Damage => damage;

        /// <summary>
        /// Скорость снаряда
        /// </summary>
        public float Speed => speed;
    }
}
