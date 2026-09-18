using UnityEngine;
using Biziwe.Player;
using Biziwe.Systems;

namespace Biziwe.Missions
{
    /// <summary>
    /// MISSION 1 — "The Errand"
    /// First job for Femi: deliver a package across town without drawing
    /// police attention. Teaches driving + the wanted system, per Docs/STORY.md.
    ///
    /// SETUP:
    /// 1. Place an empty "Mission_TheErrand" GameObject in the scene.
    /// 2. Assign startTrigger (a MissionTrigger where Femi gives the job —
    ///    e.g. outside his office) and a deliveryPoint (trigger at the
    ///    drop-off location).
    /// 3. Assign the MissionManager and MissionData (id "the_errand").
    /// </summary>
    public class Mission_TheErrand : MonoBehaviour
    {
        public MissionManager missionManager;
        public string missionId = "the_errand";
        public Transform deliveryPoint;
        public float deliveryRadius = 6f;

        [Header("Dialogue (shown via a DialogueBubble on the player or a UI text)")]
        public string introLine = "Femi: Take this to the drop point. Keep it clean — no police attention.";
        public string successLine = "Femi: Smooth. Chairman's watching now.";
        public string failLine = "Femi: You brought heat. That's not what I asked for.";

        [Header("Reward")]
        public int rewardMoney = 200;

        private bool active;

        public void StartMission()
        {
            if (missionManager.GetState(missionId) != MissionState.NotStarted) return;

            missionManager.StartMission(missionId);
            active = true;
            ShowLine(introLine);
        }

        private void ShowLine(string line)
        {
            if (UI.DialogueUI.Instance != null) UI.DialogueUI.Instance.ShowSequence(new[] { line });
            else Debug.Log(line); // DialogueUI not in the scene yet — falls back safely
        }

        private void Update()
        {
            if (!active || deliveryPoint == null) return;

            var gm = GameManager.Instance;
            if (gm == null || gm.Player == null) return;

            float dist = Vector3.Distance(gm.Player.position, deliveryPoint.position);
            if (dist <= deliveryRadius)
            {
                CompleteDelivery(gm);
            }
        }

        private void CompleteDelivery(GameManager gm)
        {
            active = false;

            bool arrivedClean = gm.Wanted == null || gm.Wanted.wantedLevel == 0;
            if (arrivedClean)
            {
                ShowLine(successLine);
                missionManager.CompleteMission(missionId, rewardMoney);
            }
            else
            {
                ShowLine(failLine);
                missionManager.CompleteMission(missionId, rewardMoney / 2); // still counts, smaller payout — mirrors "brought heat" consequence
            }
        }
    }
}
