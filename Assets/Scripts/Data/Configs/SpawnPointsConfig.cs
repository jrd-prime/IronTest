using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SpawnPointsConfig), menuName = "Config/" + nameof(SpawnPointsConfig), order = 1)]
public class SpawnPointsConfig : ScriptableObject
{
    [SerializeField] private Transform leftEnemyPoint;
    [SerializeField] private Transform rightEnemyPoint;
    [SerializeField] private Transform leftPlayerPoint;
    [SerializeField] private Transform rightPlayerPoint;
    [SerializeField] private Transform basePoint;

    private void OnValidate()
    {
        if (!leftEnemyPoint || !rightEnemyPoint || !leftPlayerPoint || !rightPlayerPoint || !basePoint)
            throw new Exception("All or some spawn points are not assigned. " + name);
    }

    public Vector3 GetSpawnPoint(EUnit unitType, ESpawnSide spawnSide) =>
        unitType switch
        {
            EUnit.Player => spawnSide == ESpawnSide.Left ? leftPlayerPoint.position : rightPlayerPoint.position,
            EUnit.Enemy => spawnSide == ESpawnSide.Left ? leftEnemyPoint.position : rightEnemyPoint.position,
            _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
        };

    public Vector3 GetBaseSpawnPoint() => basePoint.position;
}
