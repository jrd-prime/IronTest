using System;
using UnityEngine;

namespace Components
{
    public class Health : MonoBehaviour
    {
        public Action OnHPChange = delegate { };
        public Action OnDie = delegate { };

        public float HP { get; private set; }
        public int MaxHP { get; private set; }

        public void Initialize(int maxHP)
        {
            if (maxHP <= 0)
            {
                Debug.LogError($"MaxHP can't be less than or equal to 0. MaxHP: {maxHP}");
                enabled = false;
                return;
            }

            MaxHP = maxHP;
            HP = maxHP;
        }

        public void DoDamage(float damage)
        {
            if (damage < 0)
            {
                Debug.LogWarning($"Damage can't be less than 0. Damage: {damage}");
                return;
            }

            if (HP - damage > 0)
            {
                DecreaseHP(damage);
                return;
            }

            OnDie?.Invoke();
        }

        private void DecreaseHP(float damage)
        {
            HP -= damage;
            OnHPChange?.Invoke();
        }

        public float GetPercentage() => HP / MaxHP;
    }
}
