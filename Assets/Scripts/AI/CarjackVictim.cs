using UnityEngine;
using UnityEngine.AI;
using Biziwe.Systems;

namespace Biziwe.AI
{
    public enum VictimState { Shouting, Fleeing, SeekingVehicle, Chasing, Attacking }

    /// <summary>
    /// What happens to an NPC after the player carjacks their car — this is the
    /// "owner shouts, runs, finds another car, chases you down" behaviour.
    /// Phase 2.
    ///
    /// Combat note: uses the shared Health component (Systems/Health.cs) for
    /// damage/death, same as the player and regular NPCs — kept consistent
    /// rather than each script tracking its own health value.
    ///
    /// SETUP:
    /// 1. Spawn/activate this on the ejected driver when CarjackHandler.EjectDriver()
    ///    runs (hook it up there instead of just disabling the driver visual).
    /// 2. Requires a NavMeshAgent for the on-foot phases, same as NpcController.
    /// 3. Add a Health component (Assets/Scripts/Systems/Health.cs) alongside this.
    /// 4. Assign nearbyVehicleSearchRadius and a layer mask for detecting parked cars.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class CarjackVictim : MonoBehaviour
    {
        [Header("Reaction Timing")]
        public float shoutDuration = 1.2f;
        public float fleeDuration = 2.5f;

        [Header("Chase")]
        public float nearbyVehicleSearchRadius = 20f;
        public LayerMask vehicleLayer;

        [Header("Combat")]
        public float stopAndShootDistance = 12f; // once this close behind, victim can turn and fight

        public VictimState State { get; private set; }
        private NavMeshAgent agent;
        private Health health;
        private Transform player;
        private float stateTimer;
        private Vehicle.VehicleController stolenAlternateVehicle;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            if (health != null)
                health.OnDeath += HandleDeath;
        }

        private void OnDestroy()
        {
            if (health != null)
                health.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            player = null; // stop reacting
            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = true;
        }

        public void BeginReaction(Transform playerTransform)
        {
            player = playerTransform;
            State = VictimState.Shouting;
            stateTimer = 0f;
            // Hook: play a shout audio clip / angry voice line here.
        }

        private void Update()
        {
            if (player == null) return;
            stateTimer += Time.deltaTime;

            switch (State)
            {
                case VictimState.Shouting:
                    if (stateTimer >= shoutDuration) TransitionTo(VictimState.Fleeing);
                    break;

                case VictimState.Fleeing:
                    FleeFromPlayer();
                    if (stateTimer >= fleeDuration) TransitionTo(VictimState.SeekingVehicle);
                    break;

                case VictimState.SeekingVehicle:
                    SeekNearbyVehicle();
                    break;

                case VictimState.Chasing:
                    ChasePlayer();
                    break;

                case VictimState.Attacking:
                    // Victim has stopped and is shooting/fighting — handled by a
                    // Weapon/MeleeWeapon component on this same NPC, triggered externally
                    // once close enough (stopAndShootDistance). Hook combat AI here.
                    break;
            }
        }

        private void TransitionTo(VictimState newState)
        {
            State = newState;
            stateTimer = 0f;
        }

        private void FleeFromPlayer()
        {
            Vector3 fleeDir = (transform.position - player.position).normalized;
            agent.SetDestination(transform.position + fleeDir * 10f);
        }

        private void SeekNearbyVehicle()
        {
            Collider[] nearby = Physics.OverlapSphere(transform.position, nearbyVehicleSearchRadius, vehicleLayer);
            if (nearby.Length > 0)
            {
                var vehicle = nearby[0].GetComponent<Vehicle.VehicleController>();
                if (vehicle != null)
                {
                    stolenAlternateVehicle = vehicle;
                    // Simplified: victim "enters" the found vehicle and continues the
                    // chase using its position directly. A fuller version would play an
                    // enter animation and hand off movement to VehicleController AI-driven.
                    TransitionTo(VictimState.Chasing);
                    return;
                }
            }
            // No vehicle found nearby — continue chasing on foot instead.
            TransitionTo(VictimState.Chasing);
        }

        private void ChasePlayer()
        {
            Transform chaseSource = stolenAlternateVehicle != null ? stolenAlternateVehicle.transform : transform;
            float distance = Vector3.Distance(chaseSource.position, player.position);

            if (distance <= stopAndShootDistance)
            {
                // Close enough — stop, turn to face the player, switch to combat.
                agent.SetDestination(transform.position); // hold position
                transform.LookAt(player);
                TransitionTo(VictimState.Attacking);
                return;
            }

            agent.SetDestination(player.position);
        }
    }
}
