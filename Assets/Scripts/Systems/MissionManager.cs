using UnityEngine;
using System;
using System.Collections.Generic;

namespace Biziwe.Systems
{
    public enum MissionState { NotStarted, Active, Completed, Failed }

    /// <summary>
    /// Simple mission system — enough to run the first 3 missions
    /// (The Errand, The Setup, The Choice). Phase 1/2.
    ///
    /// SETUP:
    /// 1. Add to the GameManager object alongside WantedSystem and EconomySystem.
    /// 2. Place a MissionTrigger (below) in the scene at each objective location.
    /// 3. Define missions in code or later via a ScriptableObject data file —
    ///    starting simple here so it's easy to reason about.
    /// </summary>
    public class MissionManager : MonoBehaviour
    {
        public EconomySystem economy;

        private Dictionary<string, MissionState> missionStates = new Dictionary<string, MissionState>();
        public event Action<string> OnMissionStarted;
        public event Action<string> OnMissionCompleted;
        public event Action<string> OnMissionFailed;

        public string ActiveMissionId { get; private set; }

        public void StartMission(string missionId)
        {
            missionStates[missionId] = MissionState.Active;
            ActiveMissionId = missionId;
            OnMissionStarted?.Invoke(missionId);
        }

        public void CompleteMission(string missionId, int rewardMoney = 0)
        {
            if (GetState(missionId) != MissionState.Active) return;

            missionStates[missionId] = MissionState.Completed;
            if (ActiveMissionId == missionId) ActiveMissionId = null;

            if (rewardMoney > 0) economy?.AddMoney(rewardMoney);
            OnMissionCompleted?.Invoke(missionId);
        }

        public void FailMission(string missionId)
        {
            if (GetState(missionId) != MissionState.Active) return;

            missionStates[missionId] = MissionState.Failed;
            if (ActiveMissionId == missionId) ActiveMissionId = null;
            OnMissionFailed?.Invoke(missionId);
        }

        public MissionState GetState(string missionId)
        {
            return missionStates.TryGetValue(missionId, out var state) ? state : MissionState.NotStarted;
        }
    }
}
