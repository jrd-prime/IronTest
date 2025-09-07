using System;
using System.Collections.Generic;
using Data.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public sealed class SpawnManagerUi : MonoBehaviour
    {
        public event Action<EUnit, ESpawnSide> OnUnitSpawn = delegate { };

        [Header("Spawn Buttons")]
        [SerializeField] private Button spawnPlayerUnitLeftBtn;
        [SerializeField] private Button spawnPlayerUnitRightBtn;
        [SerializeField] private Button spawnEnemyUnitLeftBtn;
        [SerializeField] private Button spawnEnemyUnitRightBtn;

        private readonly Dictionary<Button, (EUnit unit, ESpawnSide side)> _buttonConfigs = new();

        private void Awake()
        {
            InitializeButtonConfigs();
            Validate();
            Subscribe();
        }

        private void InitializeButtonConfigs()
        {
            _buttonConfigs.Add(spawnPlayerUnitLeftBtn, (EUnit.Player, ESpawnSide.Left));
            _buttonConfigs.Add(spawnPlayerUnitRightBtn, (EUnit.Player, ESpawnSide.Right));
            _buttonConfigs.Add(spawnEnemyUnitLeftBtn, (EUnit.Enemy, ESpawnSide.Left));
            _buttonConfigs.Add(spawnEnemyUnitRightBtn, (EUnit.Enemy, ESpawnSide.Right));
        }

        private void Subscribe()
        {
            foreach (var (button, config) in _buttonConfigs)
                button.onClick.AddListener(() => OnUnitSpawn?.Invoke(config.unit, config.side));
        }

        private void Unsubscribe()
        {
            foreach (var (button, config) in _buttonConfigs)
                button.onClick.RemoveListener(() => OnUnitSpawn?.Invoke(config.unit, config.side));
        }

        private void OnDestroy() => Unsubscribe();

        private void Validate()
        {
            var messages = new List<string>();

            if (!spawnPlayerUnitLeftBtn)
                messages.Add("No spawnPlayerUnitLeftBtn is assigned.");

            if (!spawnPlayerUnitRightBtn)
                messages.Add("No spawnPlayerUnitRightBtn is assigned.");

            if (!spawnEnemyUnitLeftBtn)
                messages.Add("No spawnEnemyUnitLeftBtn is assigned.");

            if (!spawnEnemyUnitRightBtn)
                messages.Add("No spawnEnemyUnitRightBtn is assigned.");

            if (messages.Count <= 0)
                return;

            Debug.LogError(string.Join("\n", messages), this);
            enabled = false;
        }
    }
}