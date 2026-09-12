using UnityEngine;
using Biziwe.Player;

namespace Biziwe.Systems
{
    /// <summary>
    /// Place in the scene as a trigger zone — walking/driving into it starts or
    /// completes a mission objective. Used for "The Errand" (drive to a delivery
    /// point), "The Setup", and "The Choice" faction meeting points.
    ///
    /// SETUP:
    /// 1. Create an empty GameObject, add a Collider, check "Is Trigger".
    /// 2. Drag this script on, set missionId and whether this trigger starts or
    ///    completes that mission.
    /// 3. Assign the scene's MissionManager reference.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class MissionTrigger : MonoBehaviour
    {
        public MissionManager missionManager;
        public string missionId;
        public bool startsMission = false;
        public bool completesMission = false;
        public int rewardMoney = 0;

        private void OnTriggerEnter(Collider other)
        {
            bool isPlayer = other.GetComponent<PlayerController>() != null;
            if (!isPlayer) return;

            if (startsMission && missionManager.GetState(missionId) == MissionState.NotStarted)
            {
                missionManager.StartMission(missionId);
            }
            else if (completesMission && missionManager.GetState(missionId) == MissionState.Active)
            {
                missionManager.CompleteMission(missionId, rewardMoney);
            }
        }
    }
}
