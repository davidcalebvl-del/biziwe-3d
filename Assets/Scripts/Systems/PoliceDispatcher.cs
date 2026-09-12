using UnityEngine;
using System.Collections.Generic;

namespace Biziwe.Systems
{
    /// <summary>
    /// Escalates police response as the wanted level rises — more units join
    /// the chase at 2 and 3 stars, matching "call additional units" from the
    /// vision doc. Hooks into WantedSystem.OnWantedLevelChanged rather than
    /// duplicating wanted-level tracking.
    ///
    /// SETUP:
    /// 1. Add to the same GameManager object as WantedSystem.
    /// 2. Assign the WantedSystem reference.
    /// 3. Place police car prefabs (with PoliceVehicleAI + VehicleController) in
    ///    the scene ahead of time, inactive, positioned at police-station spawn
    ///    points, and add them to policeVehiclePool.
    /// </summary>
    public class PoliceDispatcher : MonoBehaviour
    {
        public WantedSystem wantedSystem;
        public Transform player;
        public List<AI.PoliceVehicleAI> policeVehiclePool = new List<AI.PoliceVehicleAI>();

        [Header("Escalation")]
        [Tooltip("How many police vehicles should be actively chasing at each wanted level (index 0 = 1 star).")]
        public int[] vehiclesPerStar = { 1, 2, 3 };

        private List<AI.PoliceVehicleAI> activeVehicles = new List<AI.PoliceVehicleAI>();

        private void OnEnable()
        {
            if (wantedSystem != null)
                wantedSystem.OnWantedLevelChanged += HandleWantedLevelChanged;
        }

        private void OnDisable()
        {
            if (wantedSystem != null)
                wantedSystem.OnWantedLevelChanged -= HandleWantedLevelChanged;
        }

        private void HandleWantedLevelChanged(int newLevel)
        {
            if (newLevel <= 0)
            {
                RecallAllVehicles();
                return;
            }

            int targetCount = newLevel <= vehiclesPerStar.Length ? vehiclesPerStar[newLevel - 1] : vehiclesPerStar[^1];
            EnsureActiveVehicleCount(targetCount);
        }

        private void EnsureActiveVehicleCount(int targetCount)
        {
            while (activeVehicles.Count < targetCount)
            {
                var next = GetAvailableVehicle();
                if (next == null) break; // pool exhausted — fine, just chase with what's available

                next.gameObject.SetActive(true);
                next.BeginChase(player);
                activeVehicles.Add(next);
            }
        }

        private AI.PoliceVehicleAI GetAvailableVehicle()
        {
            foreach (var v in policeVehiclePool)
            {
                if (!activeVehicles.Contains(v)) return v;
            }
            return null;
        }

        private void RecallAllVehicles()
        {
            foreach (var v in activeVehicles)
            {
                if (v == null) continue;
                v.StopChase();
                v.gameObject.SetActive(false);
            }
            activeVehicles.Clear();
        }
    }
}
