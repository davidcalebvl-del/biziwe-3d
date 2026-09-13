using UnityEngine;

namespace Biziwe.World
{
    /// <summary>
    /// One checkpoint in a race — place these along a route in order. Pairs
    /// with RaceManager, which tracks progress through them.
    ///
    /// SETUP:
    /// 1. Create an empty GameObject with a trigger Collider at each
    ///    checkpoint along the route.
    /// 2. Assign checkpointIndex in order (0, 1, 2, ...).
    /// 3. Assign the same RaceManager to every checkpoint in the race.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class RaceCheckpoint : MonoBehaviour
    {
        public RaceManager raceManager;
        public int checkpointIndex;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (raceManager == null) return;
            // Works whether the player is on foot or in a vehicle — checks either.
            bool isPlayer = other.GetComponent<Player.PlayerController>() != null
                || other.GetComponentInParent<Vehicle.VehicleController>() != null;
            if (!isPlayer) return;

            raceManager.CheckpointReached(checkpointIndex);
        }
    }

    /// <summary>
    /// Tracks progress through a checkpoint race, timing, and completion —
    /// the actual system behind the "Racing" MissionType. Rewards money
    /// through EconomySystem on finish, same as any other mission.
    ///
    /// SETUP:
    /// 1. Add to an empty "RaceManager" GameObject in the scene.
    /// 2. Set totalCheckpoints to match how many RaceCheckpoint objects you
    ///    place (checkpoint 0 through totalCheckpoints - 1 is the finish).
    /// 3. Optionally link to a MissionData id via missionId so completing
    ///    the race also completes/rewards through MissionManager.
    /// </summary>
    public class RaceManager : MonoBehaviour
    {
        public int totalCheckpoints = 5;
        public string missionId; // optional — ties this race to a MissionData/MissionManager entry
        public int rewardMoney = 500;

        private int nextExpectedCheckpoint = 0;
        private float raceStartTime;
        private bool raceActive;

        public event System.Action OnRaceStarted;
        public event System.Action<float> OnRaceFinished; // passes final time in seconds
        public event System.Action<int, int> OnCheckpointProgress; // (reached, total)

        public void StartRace()
        {
            nextExpectedCheckpoint = 0;
            raceStartTime = Time.time;
            raceActive = true;
            OnRaceStarted?.Invoke();
        }

        public void CheckpointReached(int checkpointIndex)
        {
            if (!raceActive || checkpointIndex != nextExpectedCheckpoint) return;

            nextExpectedCheckpoint++;
            OnCheckpointProgress?.Invoke(nextExpectedCheckpoint, totalCheckpoints);

            if (nextExpectedCheckpoint >= totalCheckpoints)
            {
                FinishRace();
            }
        }

        private void FinishRace()
        {
            raceActive = false;
            float finalTime = Time.time - raceStartTime;
            OnRaceFinished?.Invoke(finalTime);

            var gm = Systems.GameManager.Instance;
            if (gm != null)
            {
                if (!string.IsNullOrEmpty(missionId) && gm.Missions != null)
                {
                    gm.Missions.CompleteMission(missionId, rewardMoney);
                }
                else
                {
                    gm.Economy?.AddMoney(rewardMoney); // no linked mission — still pays out directly
                }
            }
        }
    }
}
