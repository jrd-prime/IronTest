using System;
using UnityEngine;

namespace Data.Configs
{
    [Serializable]
    [CreateAssetMenu(
        fileName = nameof(WeaponConfig),
        menuName = "Config/" + nameof(WeaponConfig)
    )]
    public sealed class WeaponConfig : ScriptableObject
    {
        [Range(1, 100f)] [SerializeField] private float range;
        [Range(1, 100)] [SerializeField] private int ammoCount;
        [Range(0.1f, 10f)] [SerializeField] private float rateFire;
        [Range(1f, 20f)] [SerializeField] private float reload;

        /// <summary>
        /// Дальность стрельбы
        /// </summary>
        public float Range => range;

        /// <summary>
        /// Кол-во патронов в магазине
        /// </summary>
        public int AmmoCount => ammoCount;

        /// <summary>
        /// Частота выстрелов в одном магазине
        /// </summary>
        public float RateFire => rateFire;

        /// <summary>
        /// Перезарядка
        /// </summary>
        public float Reload => reload;
    }
}
