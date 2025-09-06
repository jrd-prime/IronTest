using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Config/Unit", order = 1)]
public class UnitConfig : ScriptableObject
{
    public string id
    {
        get { return name; }
    }

    public EFaceDirection FaceDirection => faceDirection;

    [Header("Main")] [SerializeField] private EFaceDirection faceDirection = EFaceDirection.NotSet;

    [Header("Sounds")] public AudioClip soundDeath;
    public AudioClip soundSpawn;

    [Header("Stats")] public float damage = 10;
   [Range(10, 10000)] public int hp;
    [Range(1, 10f)] public float moveSpeed;
    [Range(1, 100f)] public float stopDistance;
    [Range(1, 100f)]public float range;

    [Header("Weapon")] public float ammoCount;
    public float rateFire;
    public float reload;

    [Header("Other")] public string spawnMessageUnit;

    private void OnValidate()
    {
        if (faceDirection == EFaceDirection.NotSet)
            Debug.LogError("Face direction can't be NotSet. " + name, this);
        
        
        
        if(range < stopDistance)
            Debug.LogError("Range can't be less than stop distance. " + name, this);
    }
}
