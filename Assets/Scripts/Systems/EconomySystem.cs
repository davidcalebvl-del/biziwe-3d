using UnityEngine;
using System;

namespace Biziwe.Systems
{
    /// <summary>
    /// Player's persistent money balance — earn from missions/jobs, spend on
    /// weapons, vehicles, upgrades. Phase 3.
    ///
    /// Capped at 1,000,000 by design — reaching the cap fires OnMaxBalanceReached,
    /// which the UI can treat as a "win"/milestone moment (fireworks, a trophy
    /// screen, whatever fits the game's tone).
    ///
    /// SETUP:
    /// 1. Add to the same GameManager object as WantedSystem.
    /// 2. Other scripts (MissionManager, shops) call AddMoney()/SpendMoney().
    /// 3. Persistence (saving between sessions) is a later addition — this starts
    ///    as an in-memory balance for a single play session.
    /// </summary>
    public class EconomySystem : MonoBehaviour
    {
        public int startingBalance = 500;
        public const int MaxBalance = 1_000_000;
        public int CurrentBalance { get; private set; }

        public event Action<int> OnBalanceChanged;
        public event Action OnMaxBalanceReached;
        private bool maxReachedFired;

        private void Awake()
        {
            CurrentBalance = startingBalance;
        }

        public void AddMoney(int amount)
        {
            if (amount <= 0) return;
            CurrentBalance = Mathf.Min(CurrentBalance + amount, MaxBalance);
            OnBalanceChanged?.Invoke(CurrentBalance);

            if (CurrentBalance >= MaxBalance && !maxReachedFired)
            {
                maxReachedFired = true;
                OnMaxBalanceReached?.Invoke();
            }
        }

        /// <returns>True if the purchase succeeded (enough balance), false otherwise.</returns>
        public bool SpendMoney(int amount)
        {
            if (amount <= 0 || amount > CurrentBalance) return false;
            CurrentBalance -= amount;
            OnBalanceChanged?.Invoke(CurrentBalance);
            return true;
        }
    }
}
