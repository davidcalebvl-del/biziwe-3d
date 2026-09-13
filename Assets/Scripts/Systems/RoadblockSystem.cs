using UnityEngine;
using System.Collections.Generic;

namespace Biziwe.Systems
{
    /// <summary>
    /// Spawns police roadblocks (barrier props + parked police cars) ahead of
    /// the player once the wanted level is high enough — the "block roads"
    /// behaviour from the vision doc. Hooks into WantedSystem.OnWantedLevelChanged,
    /// same pattern as PoliceDispatcher, so it stays in sync automatically.
    ///
    /// SETUP:
    /// 1. Add to the same GameManager object as WantedSystem.
    /// 2. Assign a roadblockPrefab (barriers + a couple of static/parked police
    ///    car visuals — doesn't need VehicleController, these don't drive).
    /// 3. Assign player and pick which wanted level triggers roadblocks
    ///    (2 stars by default — a roadblock at 1 star would feel excessive).
    /// </summary>
    public class RoadblockSystem : MonoBehaviour
    {
        public WantedSystem wantedSystem;
        public Transform player;
        public GameObject roadblockPrefab;

        [Header("Behaviour")]
        public int minWantedLevelForRoadblocks = 2;
        public float spawnAheadDistance = 40f;
        public float spawnInterval = 12f; // don't spawn a new one constantly, keeps it fair
        public int maxActiveRoadblocks = 3;

        private float spawnTimer;
        private List<GameObject> activeRoadblocks = new List<GameObject>();

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
            if (newLevel < minWantedLevelForRoadblocks)
            {
                ClearAllRoadblocks();
            }
        }

        private void Update()
        {
            if (wantedSystem == null || player == null || roadblockPrefab == null) return;
            if (wantedSystem.wantedLevel < minWantedLevelForRoadblocks) return;

            CleanUpPassedRoadblocks();

            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval && activeRoadblocks.Count < maxActiveRoadblocks)
            {
                spawnTimer = 0f;
                SpawnRoadblockAhead();
            }
        }

        private void SpawnRoadblockAhead()
        {
            // Simple version: place ahead of the player's current facing direction.
            // A fuller version would place it specifically along the actual road
            // spline/waypoint the player is driving on — a good later upgrade
            // once the city's road network exists as real data, not needed yet
            // for this to feel functional.
            Vector3 spawnPos = player.position + player.forward * spawnAheadDistance;
            GameObject block = Instantiate(roadblockPrefab, spawnPos, player.rotation);
            activeRoadblocks.Add(block);
        }

        private void CleanUpPassedRoadblocks()
        {
            activeRoadblocks.RemoveAll(b => b == null);

            for (int i = activeRoadblocks.Count - 1; i >= 0; i--)
            {
                var block = activeRoadblocks[i];
                if (block == null) continue;

                float distanceBehind = Vector3.Dot(player.position - block.transform.position, player.forward);
                if (distanceBehind > 15f) // player has driven well past it
                {
                    Destroy(block);
                    activeRoadblocks.RemoveAt(i);
                }
            }
        }

        private void ClearAllRoadblocks()
        {
            foreach (var block in activeRoadblocks)
            {
                if (block != null) Destroy(block);
            }
            activeRoadblocks.Clear();
        }
    }
}
