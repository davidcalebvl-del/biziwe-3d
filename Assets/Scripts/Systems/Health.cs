using UnityEngine;
using System;

namespace Biziwe.Systems
{
    /// <summary>
    /// Shared health/damage component — used by the player, civilian NPCs, and
    /// police so combat (weapons, melee, explosives) works consistently against
    /// anything in the world without each script reimplementing damage logic.
    ///
    /// SETUP:
    /// 1. Add to the player, and to any NPC/police prefab.
    /// 2. Other scripts (PlayerController, NpcController) can subscribe to
    ///    OnDeath to react (ragdoll, disable movement, respawn, etc).
    /// </summary>
    public class Health : MonoBehaviour, IDamageable
    {
        public float maxHealth = 100f;
        public float CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }

        public event Action<float, float> OnHealthChanged; // (current, max)
        public event Action OnDeath;
        public event Action OnRevive;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDirection)
        {
            if (IsDead) return;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (CurrentHealth <= 0f)
            {
                IsDead = true;
                OnDeath?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void Revive(float reviveHealth = -1f)
        {
            IsDead = false;
            CurrentHealth = reviveHealth > 0f ? Mathf.Min(reviveHealth, maxHealth) : maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
            OnRevive?.Invoke();
        }
    }
}
