using UnityEngine;
using System;

namespace Biziwe.Systems
{
    /// <summary>
    /// Player's persistent money balance — earn from missions/jobs, spend on
    /// weapons, vehicles, upgrades. Phase 3.
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
        public int CurrentBalance { get; private set; }

        public event Action<int> OnBalanceChanged;

        private void Awake()
        {
            CurrentBalance = startingBalance;
        }

        public void AddMoney(int amount)
        {
            if (amount <= 0) return;
            CurrentBalance += amount;
            OnBalanceChanged?.Invoke(CurrentBalance);
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
