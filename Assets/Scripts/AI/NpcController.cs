using UnityEngine;
using UnityEngine.AI;

namespace Biziwe.AI
{
    /// <summary>
    /// Minimal NPC behaviour for Phase 1 — patrols between waypoints,
    /// and can be flagged into "chase" mode by the WantedSystem.
    ///
    /// SETUP:
    /// 1. Requires Unity's NavMesh: Window > AI > Navigation, bake a NavMesh for your city floor.
    /// 2. Add a NavMeshAgent component to the NPC.
    /// 3. Drag this script onto the NPC, assign patrol waypoints (empty GameObjects placed in the scene).
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NpcController : MonoBehaviour
    {
        public Transform[] patrolPoints;
        public float waitTimeAtPoint = 2f;
        public bool isPolice = false;

        private NavMeshAgent agent;
        private int currentPoint;
        private float waitTimer;
        private Transform chaseTarget;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            if (patrolPoints != null && patrolPoints.Length > 0)
                GoToNextPoint();
        }

        private void Update()
        {
            if (chaseTarget != null)
            {
                agent.SetDestination(chaseTarget.position);
                agent.speed = isPolice ? 6f : 4f;
                return;
            }

            if (patrolPoints == null || patrolPoints.Length == 0) return;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitTimeAtPoint)
                {
                    waitTimer = 0f;
                    GoToNextPoint();
                }
            }
        }

        private void GoToNextPoint()
        {
            agent.speed = 2f; // patrol pace
            agent.destination = patrolPoints[currentPoint].position;
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }

        /// <summary>Called by WantedSystem when this NPC (if police) should chase the player.</summary>
        public void StartChase(Transform target)
        {
            chaseTarget = target;
        }

        public void StopChase()
        {
            chaseTarget = null;
            if (patrolPoints != null && patrolPoints.Length > 0)
                GoToNextPoint();
        }
    }
}
