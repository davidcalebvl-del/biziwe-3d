using UnityEngine;
using Biziwe.Systems;

namespace Biziwe.Missions
{
    /// <summary>
    /// MISSION 3 — "Made Man"
    /// Chairman notices you personally — you're officially in. Closes Act 1.
    /// Simpler than the first two — a meeting/dialogue beat rather than a
    /// driving/combat objective, per Docs/STORY.md.
    ///
    /// SETUP:
    /// 1. Place an empty "Mission_MadeMan" GameObject.
    /// 2. Assign meetingPoint (Chairman's location — e.g. the dock office).
    /// 3. Requires "the_setup" completed first.
    /// </summary>
    public class Mission_MadeMan : MonoBehaviour
    {
        public MissionManager missionManager;
        public string missionId = "made_man";
        public string requiresMissionId = "the_setup";

        public Transform meetingPoint;
        public float meetingRadius = 5f;

        [Header("Dialogue")]
        public string[] conversationLines = new string[]
        {
            "Chairman: Femi tells me you handled yourself well out there.",
            "Chairman: Most people don't walk away from a setup like that.",
            "Chairman: You're in now. Welcome to the family.",
        };

        [Header("Reward — this one pays in standing, not just cash")]
        public int rewardMoney = 1000;

        private bool available;
        private bool started;

        private void Update()
        {
            if (started) return;

            var gm = GameManager.Instance;
            if (gm == null || gm.Player == null || meetingPoint == null) return;

            if (!available)
            {
                available = !string.IsNullOrEmpty(requiresMissionId)
                    ? missionManager.GetState(requiresMissionId) == MissionState.Completed
                    : true;
                if (!available) return;

                missionManager.StartMission(missionId);
            }

            float dist = Vector3.Distance(gm.Player.position, meetingPoint.position);
            if (dist <= meetingRadius)
            {
                started = true;
                PlayConversation();
                missionManager.CompleteMission(missionId, rewardMoney);
                // Act 1 complete — Act 2 missions (Wire's Play, BB's Offer, etc.)
                // pick up from here via their own requiresMissionId = "made_man".
            }
        }

        private void PlayConversation()
        {
            if (UI.DialogueUI.Instance != null)
            {
                UI.DialogueUI.Instance.ShowSequence(conversationLines);
            }
            else
            {
                foreach (var line in conversationLines)
                    Debug.Log(line); // DialogueUI not in the scene yet — falls back safely
            }
        }
    }
}
