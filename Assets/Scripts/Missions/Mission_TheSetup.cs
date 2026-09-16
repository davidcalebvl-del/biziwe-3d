using UnityEngine;
using Biziwe.Systems;
using Biziwe.AI;

namespace Biziwe.Missions
{
    /// <summary>
    /// MISSION 2 — "The Setup"
    /// First real job goes wrong — an ambush. Teaches combat + escape, per
    /// Docs/STORY.md. Requires "the_errand" completed first.
    ///
    /// SETUP:
    /// 1. Place an empty "Mission_TheSetup" GameObject.
    /// 2. Assign jobLocation (trigger where the job "starts", ambush spawns here).
    /// 3. Assign ambushSpawnPoints (a few positions around jobLocation) and an
    ///    enemy prefab (an NpcController with isPolice = false, a Weapon or
    ///    MeleeWeapon attached — these are rival gang members, not police).
    /// 4. Assign escapeZone (trigger marking a safe point to reach after the fight).
    /// </summary>
    public class Mission_TheSetup : MonoBehaviour
    {
        public MissionManager missionManager;
        public string missionId = "the_setup";
        public string requiresMissionId = "the_errand";

        [Header("Ambush")]
        public Transform jobLocation;
        public float triggerRadius = 8f;
        public Transform[] ambushSpawnPoints;
        public GameObject enemyPrefab;

        [Header("Escape")]
        public Transform escapeZone;
        public float escapeRadius = 6f;

        [Header("Dialogue")]
        public string ambushLine = "This was a setup! Get out of here!";
        public string escapeLine = "You made it. Chairman needs to hear about this.";

        [Header("Reward")]
        public int rewardMoney = 400;

        private enum Stage { WaitingToStart, Ambushed, Escaping, Done }
        private Stage stage = Stage.WaitingToStart;

        private void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.Player == null) return;

            switch (stage)
            {
                case Stage.WaitingToStart:
                    CheckShouldStart(gm);
                    break;
                case Stage.Ambushed:
                    CheckShouldTriggerEscapePhase();
                    break;
                case Stage.Escaping:
                    CheckReachedEscapeZone(gm);
                    break;
            }
        }

        private void CheckShouldStart(GameManager gm)
        {
            if (missionManager.GetState(missionId) != MissionState.NotStarted) return;
            if (!string.IsNullOrEmpty(requiresMissionId) &&
                missionManager.GetState(requiresMissionId) != MissionState.Completed) return;
            if (jobLocation == null) return;

            float dist = Vector3.Distance(gm.Player.position, jobLocation.position);
            if (dist <= triggerRadius)
            {
                missionManager.StartMission(missionId);
                SpawnAmbush();
                stage = Stage.Ambushed;
                Debug.Log(ambushLine);
            }
        }

        private void SpawnAmbush()
        {
            if (ambushSpawnPoints == null || enemyPrefab == null) return;

            foreach (var point in ambushSpawnPoints)
            {
                if (point == null) continue;
                Instantiate(enemyPrefab, point.position, point.rotation);
            }
        }

        private void CheckShouldTriggerEscapePhase()
        {
            // Simple version: the fight itself is handled by the spawned
            // enemies' own AI/combat (NpcController + Weapon), no special
            // logic needed here — once the player starts moving toward the
            // escape zone, that's phase 2.
            stage = Stage.Escaping;
        }

        private void CheckReachedEscapeZone(GameManager gm)
        {
            if (escapeZone == null) return;

            float dist = Vector3.Distance(gm.Player.position, escapeZone.position);
            if (dist <= escapeRadius)
            {
                stage = Stage.Done;
                Debug.Log(escapeLine);
                missionManager.CompleteMission(missionId, rewardMoney);
            }
        }
    }
}
