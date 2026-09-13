using UnityEngine;
using UnityEngine.AI;

namespace Biziwe.AI
{
    public enum RoutineState { AtHome, TravelingToWork, AtWork, TravelingHome, FreeRoam }

    /// <summary>
    /// Gives a civilian an actual daily routine — home in the early morning
    /// and night, travels to a workplace, stays there through the day, travels
    /// home in the evening. This is what makes NPCs feel like they have lives
    /// rather than just wandering forever — the "go to work" / "have a job"
    /// behaviour from the vision doc, scoped to something genuinely buildable.
    ///
    /// Works alongside CivilianNpc rather than replacing it — during
    /// FreeRoam hours (after work, before bed) this hands control back to
    /// CivilianNpc's normal wander/flee/talk behaviour.
    ///
    /// SETUP:
    /// 1. Add to a civilian prefab alongside CivilianNpc and NavMeshAgent.
    /// 2. Assign homePoint and workPoint transforms placed in the scene.
    /// 3. Assign a DayNightCycle reference (same one everything else uses).
    /// 4. Tune the hour thresholds to taste — defaults are a fairly typical routine.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NpcSchedule : MonoBehaviour
    {
        public Transform homePoint;
        public Transform workPoint;
        public Systems.DayNightCycle dayNightCycle;

        [Header("Schedule (24h)")]
        public float workStartHour = 8f;
        public float workEndHour = 17f;
        public float bedHour = 22f;

        private NavMeshAgent agent;
        private CivilianNpc civilian;
        private RoutineState state = RoutineState.AtHome;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            civilian = GetComponent<CivilianNpc>();
        }

        private void Update()
        {
            if (dayNightCycle == null || homePoint == null || workPoint == null) return;

            // Let CivilianNpc keep full control during a flee/talk reaction —
            // don't fight it for movement control.
            if (civilian != null && !civilian.IsAvailableToTalk) return;

            float hour = dayNightCycle.CurrentHour;
            RoutineState desired = GetDesiredState(hour);

            if (desired != state)
            {
                state = desired;
                ActOnStateChange();
            }
        }

        private RoutineState GetDesiredState(float hour)
        {
            if (hour >= bedHour || hour < workStartHour - 1f) return RoutineState.AtHome;
            if (hour < workStartHour) return RoutineState.TravelingToWork;
            if (hour < workEndHour) return RoutineState.AtWork;
            if (hour < workEndHour + 1f) return RoutineState.TravelingHome;
            return RoutineState.FreeRoam;
        }

        private void ActOnStateChange()
        {
            switch (state)
            {
                case RoutineState.TravelingToWork:
                    agent.speed = 1.6f;
                    agent.SetDestination(workPoint.position);
                    if (civilian != null) civilian.enabled = false; // schedule has priority — don't fight wander
                    break;

                case RoutineState.AtWork:
                    agent.SetDestination(workPoint.position);
                    break;

                case RoutineState.TravelingHome:
                    agent.speed = 1.6f;
                    agent.SetDestination(homePoint.position);
                    break;

                case RoutineState.AtHome:
                    agent.SetDestination(homePoint.position);
                    if (civilian != null) civilian.enabled = false;
                    break;

                case RoutineState.FreeRoam:
                    if (civilian != null) civilian.enabled = true; // hand back to normal wander/talk behaviour
                    break;
            }
        }
    }
}
