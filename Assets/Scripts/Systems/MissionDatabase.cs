using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Biziwe.Systems
{
    /// <summary>
    /// Loads and looks up MissionData assets by id — the piece that lets
    /// MissionTrigger/MissionManager work with "hundreds of missions" without
    /// every scene needing manual references wired to every mission asset.
    ///
    /// SETUP:
    /// 1. Create a folder: Assets/Resources/Missions/
    /// 2. Put every MissionData asset you create (see Docs/STORY.md for the
    ///    Act 1-3 mission list) inside that folder — Unity's Resources.LoadAll
    ///    picks up everything there automatically, no manual list to maintain.
    /// 3. Add this script to the GameManager object.
    /// </summary>
    public class MissionDatabase : MonoBehaviour
    {
        private Dictionary<string, MissionData> missionsById = new Dictionary<string, MissionData>();

        private void Awake()
        {
            var all = Resources.LoadAll<MissionData>("Missions");
            foreach (var mission in all)
            {
                if (string.IsNullOrEmpty(mission.id))
                {
                    Debug.LogWarning($"MissionData asset '{mission.name}' has no id set — skipping.");
                    continue;
                }
                missionsById[mission.id] = mission;
            }
        }

        public MissionData GetById(string missionId)
        {
            return missionsById.TryGetValue(missionId, out var data) ? data : null;
        }

        public bool IsUnlocked(string missionId, MissionManager missionManager)
        {
            var data = GetById(missionId);
            if (data == null || string.IsNullOrEmpty(data.requiresMissionId)) return true;

            return missionManager.GetState(data.requiresMissionId) == MissionState.Completed;
        }

        public List<MissionData> GetAllByType(MissionType type)
        {
            return missionsById.Values.Where(m => m.type == type).ToList();
        }
    }
}
