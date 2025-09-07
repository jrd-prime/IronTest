using Data.Enums;
using UnityEngine;

namespace Data.Configs
{
    [CreateAssetMenu(
        fileName = nameof(UnitConfig),
        menuName = "Config/" + nameof(UnitConfig)
    )]
    public class UnitConfig : ScriptableObject
    {
        public string Id => name;
        public EFaceDirection FaceDirection => faceDirection;

        [Header("Main")]
        [SerializeField] private EFaceDirection faceDirection = EFaceDirection.NotSet;

        [Header("Sounds")]
        public AudioClip soundDeath;
        public AudioClip soundSpawn;

        [Header("Unit")]
        [Range(10, 10000)] public int hp;
        
        [Header("Movement")]
        [SerializeField] private MovementConfig movementConfig;

        [Header("Weapon")]
        [SerializeField] private WeaponConfig weaponConfig;
        [SerializeField] private BulletConfig bulletConfig;

        [Header("Other")] public string spawnMessageUnit;

        public WeaponConfig Weapon => weaponConfig;
        public BulletConfig Bullet => bulletConfig;
        public MovementConfig Movement => movementConfig;

        private void OnValidate()
        {
            if (faceDirection == EFaceDirection.NotSet)
                Debug.LogError("Face direction can't be NotSet. " + name, this);

            if (!weaponConfig)
                Debug.LogError($"Weapon data is null. {name}", this);

            if (!bulletConfig)
                Debug.LogError($"Bullet data is null. {name}", this);
            
            if (!movementConfig)
                Debug.LogError($"Movement data is null. {name}", this);
        }
    }
}
