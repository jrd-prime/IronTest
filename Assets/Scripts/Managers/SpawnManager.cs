using System;
using System.Collections.Generic;
using Data.Configs;
using Data.Enums;
using MonoItems;
using UI;
using UnityEngine;

namespace Managers
{
    public sealed class SpawnManager : GenericSingletonClass<SpawnManager>
    {
        [Header("Main")]
        [SerializeField] private SpawnManagerUi spawnManagerUi;
        [SerializeField] private SpawnPointsConfig spawnPointsConfig;

        [Header("Building")]
        [SerializeField] private BuildingItem baseBuildingPrefab;
        [SerializeField] private BuildingConfig baseConfig;

        [Header("Units")]
        [SerializeField] private UnitItem unitPrefab;
        [SerializeField] private UnitConfig playerUnitConfig;
        [SerializeField] private UnitConfig enemyUnitConfig;

        private readonly List<UnitItem> _units = new();
        private BuildingItem _baseBuilding;

        protected override void Awake()
        {
            base.Awake();
            Validate();
            CreateAndPlaceBase();

            spawnManagerUi.OnUnitSpawn += CreateAndPlaceUnit;
        }

        private void CreateAndPlaceUnit(EUnit unitType, ESpawnSide spawnSide)
        {
            var spawnPoint = spawnPointsConfig.GetSpawnPoint(unitType, spawnSide);
            var config = unitType == EUnit.Player ? playerUnitConfig : enemyUnitConfig;

            var unit = CreateUnit(unitType, spawnSide, config, spawnPoint);

            unit.ShiftSlightly();
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
            _baseBuilding = Instantiate(baseBuildingPrefab, spawnPointsConfig.GetBaseSpawnPoint(),
                Quaternion.identity);
            _baseBuilding.Initialize(baseConfig);
            _baseBuilding.OnBaseFall += OnBaseFall;
        }

        private void OnBaseFall()
        {
            foreach (var unit in _units)
            {
                if (unit)
                    Destroy(unit.gameObject);
            }

            _units.Clear();
        }

        private void OnDestroy()
        {
            spawnManagerUi.OnUnitSpawn -= CreateAndPlaceUnit;
            _baseBuilding.OnBaseFall -= OnBaseFall;
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
}
