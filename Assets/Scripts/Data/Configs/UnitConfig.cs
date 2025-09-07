using Data.Configs;
using UnityEngine;

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
    [Range(1, 10f)] public float moveSpeed;
    [Range(1, 100f)] public float stopDistance;

    [Header("Weapon")]
    [SerializeField] private WeaponConfig weaponConfig;
    [SerializeField] private BulletConfig bulletConfig;

    [Header("Other")] public string spawnMessageUnit;

    public WeaponConfig Weapon => weaponConfig;
    public BulletConfig Bullet => bulletConfig;

    private void OnValidate()
    {
        if (faceDirection == EFaceDirection.NotSet)
            Debug.LogError("Face direction can't be NotSet. " + name, this);

        if (!weaponConfig)
            Debug.LogError($"Weapon data is null. {name}", this);

        if (!bulletConfig)
            Debug.LogError($"Bullet data is null. {name}", this);

        if (stopDistance > weaponConfig.Range)
            Debug.LogError(
                $"StopDistance ({stopDistance}) can't be greater than weapon range ({weaponConfig.Range}). {name}",
                this);
    }
}
