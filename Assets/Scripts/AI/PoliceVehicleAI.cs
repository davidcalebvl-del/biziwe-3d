using UnityEngine;
using Biziwe.Vehicle;

namespace Biziwe.AI
{
    /// <summary>
    /// Drives a police car toward the player during a chase, using the same
    /// VehicleController every player-driven car uses — so police cars handle
    /// exactly like player cars, just AI-steered. Phase 2/3.
    ///
    /// This uses simple proportional steering (angle-to-target controls steer,
    /// distance controls throttle) rather than NavMesh, since NavMeshAgent isn't
    /// built for wheeled vehicle physics. Good enough for a real chase feel;
    /// road-following/pathing around obstacles is a later refinement.
    ///
    /// SETUP:
    /// 1. Add to a police car prefab alongside VehicleController.
    /// 2. Leave inactive/disabled until WantedSystem or PoliceDispatcher calls
    ///    BeginChase() on it.
    /// </summary>
    [RequireComponent(typeof(VehicleController))]
    public class PoliceVehicleAI : MonoBehaviour
    {
        public float chaseSpeed = 1f; // throttle strength while chasing
        public float steerSensitivity = 2f;
        public float stoppingDistance = 4f; // gets this close then rams/parks rather than colliding forever

        private VehicleController vehicle;
        private Transform target;
        private bool isChasing;

        private void Awake()
        {
            vehicle = GetComponent<VehicleController>();
        }

        public void BeginChase(Transform playerTransform)
        {
            target = playerTransform;
            isChasing = true;
        }

        public void StopChase()
        {
            isChasing = false;
            target = null;
            vehicle.SetThrottle(0f);
            vehicle.SetSteer(0f);
        }

        private void FixedUpdate()
        {
            if (!isChasing || target == null) return;

            Vector3 toTarget = target.position - transform.position;
            float distance = toTarget.magnitude;

            if (distance <= stoppingDistance)
            {
                vehicle.SetThrottle(0.3f); // keep light pressure — a ram/block rather than a hard stop
            }
            else
            {
                vehicle.SetThrottle(chaseSpeed);
            }

            // Angle between the car's forward direction and the direction to the target.
            Vector3 localTarget = transform.InverseTransformDirection(toTarget.normalized);
            float steerAmount = Mathf.Clamp(localTarget.x * steerSensitivity, -1f, 1f);
            vehicle.SetSteer(steerAmount);
        }
    }
}
