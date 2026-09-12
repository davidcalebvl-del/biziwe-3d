using UnityEngine;
using UnityEngine.AI;

namespace Biziwe.AI
{
    /// <summary>
    /// Ordinary civilians — the "living world" layer. Wander casually, and react
    /// realistically to nearby danger (gunfire, explosions, a fight): flee the
    /// area rather than standing around ignoring chaos. Lightweight by design
    /// so many of these can exist across the city without heavy cost — this is
    /// the population-simulation building block the full vision's NPC system
    /// scales up from later (streaming/LOD comes once there's an actual city
    /// to stream).
    ///
    /// SETUP:
    /// 1. Requires a baked NavMesh, same as NpcController.
    /// 2. Add a NavMeshAgent + this script to a civilian NPC prefab.
    /// 3. Optionally add a Health component if you want civilians to be
    ///    damageable (they are by default via IDamageable through Health).
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class CivilianNpc : MonoBehaviour
    {
        [Header("Wander")]
        public float wanderRadius = 15f;
        public float wanderInterval = 6f;

        [Header("Danger Reaction")]
        public float dangerAlertRadius = 18f;
        public float fleeDistance = 12f;
        public float calmDownTime = 8f;

        private NavMeshAgent agent;
        private float wanderTimer;
        private bool isFleeing;
        private float fleeTimer;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (isFleeing)
            {
                fleeTimer += Time.deltaTime;
                if (fleeTimer >= calmDownTime)
                {
                    isFleeing = false;
                    wanderTimer = wanderInterval; // wander again immediately once calm
                }
                return;
            }

            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderInterval)
            {
                wanderTimer = 0f;
                WanderToRandomPoint();
            }
        }

        private void WanderToRandomPoint()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.speed = 1.4f; // casual walking pace
                agent.SetDestination(hit.position);
            }
        }

        /// <summary>
        /// Call this on any nearby CivilianNpc when a gunshot, explosion, or fight
        /// happens — e.g. from Explosive.Detonate() or Weapon.Fire() via
        /// Physics.OverlapSphere(position, alertRadius) to find civilians in range.
        /// </summary>
        public void ReactToDanger(Vector3 dangerPosition)
        {
            float distance = Vector3.Distance(transform.position, dangerPosition);
            if (distance > dangerAlertRadius) return;

            isFleeing = true;
            fleeTimer = 0f;

            Vector3 fleeDirection = (transform.position - dangerPosition).normalized;
            Vector3 fleeTarget = transform.position + fleeDirection * fleeDistance;

            if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas))
            {
                agent.speed = 5f; // panic sprint
                agent.SetDestination(hit.position);
            }
        }
    }
}
