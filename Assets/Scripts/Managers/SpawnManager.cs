using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class SpawnManager : GenericSingletonClass<SpawnManager>
{
    [Header("Main")] [SerializeField] private SpawnManagerUi spawnManagerUi;

    [FormerlySerializedAs("spawnPointsData")] [SerializeField]
    private SpawnPointsConfig spawnPointsConfig;

    [Header("Building")] [SerializeField] private BuildingItem baseBuildingPrefab;
    [SerializeField] private BuildingConfig baseConfig;

     [FormerlySerializedAs("aUnitPrefab")] [Header("Units")] [SerializeField] private UnitItem unitPrefab;

    [SerializeField] private UnitConfig playerUnitConfig;
    [SerializeField] private UnitConfig enemyUnitConfig;

    private readonly List<UnitItem> _units = new();

    //тут сделать весь контроль
    //  спаун юнитов через кнопки юай, врагов и своих и их инит
    // спаун базы ровно по центру. в координатах по Х 0
    protected override void Awake()
    {
        base.Awake();
        Validate();

        CreateAndPlaceBase();

        spawnManagerUi.OnUnitSpawn += CreateAndPlaceUnit;
    }

    private void CreateAndPlaceUnit(EUnit unitType, ESpawnSide spawnSide)
    {
        Debug.Log($"{nameof(CreateAndPlaceUnit)}: {unitType}, {spawnSide}");

        var spawnPoint = spawnPointsConfig.GetSpawnPoint(unitType, spawnSide);
        var config = unitType == EUnit.Player ? playerUnitConfig : enemyUnitConfig;

        UnitItem unit = CreateUnit(unitType,spawnSide, config, spawnPoint);
        
        
    }

    private UnitItem CreateUnit(EUnit unitType, ESpawnSide spawnSide, UnitConfig config, Vector3 spawnPoint)
    {
        var unit = unitType switch
        {
            EUnit.Player => Instantiate(unitPrefab, spawnPoint, Quaternion.identity),
            EUnit.Enemy => Instantiate(unitPrefab, spawnPoint, Quaternion.identity),
            _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
        };

        _units.Add(unit);


        unit.Initialize(unitType, spawnSide, config);
        return unit;
    }

    private void CreateAndPlaceBase()
    {
        var baseBuilding = Instantiate(baseBuildingPrefab, spawnPointsConfig.GetBaseSpawnPoint(), Quaternion.identity);
        baseBuilding.Initialize(baseConfig);
    }

    private void OnDestroy()
    {
        foreach (var unit in _units)
        {
            if (unit)
                Destroy(unit.gameObject);
        }
    }

    private void Validate()
    {
        var messages = new List<string>();
        if (!spawnManagerUi)
            messages.Add($"{nameof(spawnManagerUi)} is null");

        if (!baseConfig)
            messages.Add($"{nameof(baseConfig)} is null");

        if (!spawnPointsConfig)
            messages.Add($"{nameof(spawnPointsConfig)} is null");

        if (!baseBuildingPrefab)
            messages.Add($"{nameof(baseBuildingPrefab)} is null");

        if (!baseConfig)
            messages.Add($"{nameof(baseConfig)} is null");

        if (messages.Count == 0)
            return;

        Debug.LogError(string.Join("\n", messages), this);
        enabled = false;
    }
}
