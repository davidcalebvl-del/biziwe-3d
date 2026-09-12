using UnityEngine;

namespace Biziwe.AI
{
    /// <summary>
    /// Civilian traffic — drives a loop of waypoints automatically, brakes for
    /// obstacles ahead (other cars, the player). Uses the same VehicleController
    /// every car in the game uses, just AI-steered like PoliceVehicleAI, so
    /// traffic cars behave with the same physics as the player's car. Phase 2.
    ///
    /// SETUP:
    /// 1. Place empty GameObjects along a road as waypoints, in driving order.
    /// 2. Add this script to a traffic car prefab (alongside VehicleController),
    ///    assign the waypoints array.
    /// 3. Spawn/pool these along your roads — pair with NpcSpawner-style
    ///    distance culling once you have a full city (see Docs/LIVING_WORLD.md).
    /// </summary>
    [RequireComponent(typeof(Vehicle.VehicleController))]
    public class TrafficCarAI : MonoBehaviour
    {
        public Transform[] waypoints;
        public float waypointReachDistance = 6f;
        public float driveSpeed = 0.6f; // throttle strength, keep below 1 for a calmer traffic pace
        public float steerSensitivity = 2f;

        [Header("Obstacle Avoidance")]
        public float lookAheadDistance = 8f;
        public LayerMask obstacleLayers;

        private Vehicle.VehicleController vehicle;
        private int currentWaypoint;

        private void Awake()
        {
            vehicle = GetComponent<Vehicle.VehicleController>();
        }

        private void FixedUpdate()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            Transform target = waypoints[currentWaypoint];
            Vector3 toTarget = target.position - transform.position;

            if (toTarget.magnitude < waypointReachDistance)
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                target = waypoints[currentWaypoint];
                toTarget = target.position - transform.position;
            }

            Vector3 localTarget = transform.InverseTransformDirection(toTarget.normalized);
            float steer = Mathf.Clamp(localTarget.x * steerSensitivity, -1f, 1f);
            vehicle.SetSteer(steer);

            float throttle = IsBlockedAhead() ? 0f : driveSpeed;
            vehicle.SetThrottle(throttle);
        }

        private bool IsBlockedAhead()
        {
            return Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward,
                lookAheadDistance, obstacleLayers);
        }
    }
}
