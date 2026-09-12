using UnityEngine;
using System.Collections.Generic;

namespace Biziwe.AI
{
    /// <summary>
    /// Population streaming — only keeps civilians active near the player,
    /// pooling and recycling them instead of spawning/destroying constantly.
    /// This is the scaffolding the "millions of NPCs, but only simulate what's
    /// nearby" goal from the vision doc is built from — starts simple here
    /// (one spawner, one radius), designed to extend to per-district spawners
    /// across a full city later without changing this core approach.
    ///
    /// SETUP:
    /// 1. Add to an empty "PopulationManager" GameObject in the scene.
    /// 2. Assign the player transform and a civilian prefab (with NavMeshAgent +
    ///    CivilianNpc + Health components).
    /// 3. Tune poolSize/spawnRadius/despawnRadius to taste — more pool = more
    ///    NPCs visible at once, at a performance cost.
    /// </summary>
    public class NpcSpawner : MonoBehaviour
    {
        public Transform player;
        public GameObject civilianPrefab;

        [Header("Pool")]
        public int poolSize = 20;

        [Header("Streaming Radii")]
        public float spawnRadius = 40f;   // spawn civilians within this distance of the player
        public float despawnRadius = 60f; // recycle civilians once they're farther than this

        [Header("Day/Night Density")]
        [Tooltip("Optional — fewer civilians active at night, matching real city rhythms.")]
        public Systems.DayNightCycle dayNightCycle;
        [Range(0.1f, 1f)] public float nightDensityMultiplier = 0.35f;

        private List<GameObject> pool = new List<GameObject>();
        private float recheckTimer;
        private const float recheckInterval = 1.5f; // don't run this every frame — it's a coarse population check

        private void Start()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject npc = Instantiate(civilianPrefab);
                npc.SetActive(false);
                pool.Add(npc);
            }
        }

        private void Update()
        {
            if (player == null) return;

            recheckTimer += Time.deltaTime;
            if (recheckTimer < recheckInterval) return;
            recheckTimer = 0f;

            int activeTarget = GetTargetActiveCount();
            int currentlyActive = 0;

            foreach (var npc in pool)
            {
                float distance = Vector3.Distance(npc.transform.position, player.position);

                if (npc.activeSelf)
                {
                    currentlyActive++;
                    if (distance > despawnRadius)
                        npc.SetActive(false); // out of range — recycle back into the pool
                }
            }

            // Spawn more if under the current (day/night-adjusted) target.
            foreach (var npc in pool)
            {
                if (currentlyActive >= activeTarget) break;
                if (npc.activeSelf) continue;

                Vector3 spawnPos = GetRandomPointAround(player.position, spawnRadius);
                npc.transform.position = spawnPos;
                npc.SetActive(true);
                currentlyActive++;
            }
        }

        private int GetTargetActiveCount()
        {
            if (dayNightCycle != null && dayNightCycle.IsNight)
                return Mathf.RoundToInt(poolSize * nightDensityMultiplier);
            return poolSize;
        }

        private Vector3 GetRandomPointAround(Vector3 center, float radius)
        {
            Vector2 offset = UnityEngine.Random.insideUnitCircle * radius;
            return center + new Vector3(offset.x, 0f, offset.y);
            // Note: doesn't validate against NavMesh here for simplicity — CivilianNpc's
            // own wander logic uses NavMesh.SamplePosition, so it self-corrects onto
            // walkable ground shortly after spawning.
        }
    }
}
