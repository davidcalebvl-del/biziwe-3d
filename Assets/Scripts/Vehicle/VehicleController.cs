using UnityEngine;

namespace Biziwe.Vehicle
{
    /// <summary>
    /// Basic drivable car using Unity's built-in WheelCollider physics — Phase 1.
    ///
    /// SETUP:
    /// 1. Import or build a simple car model with 4 wheel meshes.
    /// 2. Add a Rigidbody to the car body (set mass ~1200).
    /// 3. Create 4 empty GameObjects positioned at each wheel, add a WheelCollider
    ///    component to each, and assign them to the fields below.
    /// 4. Assign the visual wheel mesh transforms too, so they spin/turn to match.
    /// 5. Drag this script onto the car body.
    /// </summary>
    public class VehicleController : MonoBehaviour
    {
        [Header("Wheel Colliders")]
        public WheelCollider frontLeftCollider;
        public WheelCollider frontRightCollider;
        public WheelCollider rearLeftCollider;
        public WheelCollider rearRightCollider;

        [Header("Wheel Meshes (visual only)")]
        public Transform frontLeftMesh;
        public Transform frontRightMesh;
        public Transform rearLeftMesh;
        public Transform rearRightMesh;

        [Header("Handling")]
        public float maxMotorTorque = 1500f;
        public float maxSteerAngle = 30f;
        public float brakeTorque = 3000f;

        private float steerInput;   // -1 to 1, wire to on-screen left/right buttons
        private float throttleInput; // -1 to 1, wire to on-screen gas/brake buttons
        private bool handbrake;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = new Vector3(0f, -0.5f, 0f); // lower COM for stability
        }

        // Call these from your on-screen touch buttons (see UI setup in Docs/PHASE1_PLAN.md)
        public void SetSteer(float value) => steerInput = Mathf.Clamp(value, -1f, 1f);
        public void SetThrottle(float value) => throttleInput = Mathf.Clamp(value, -1f, 1f);
        public void SetHandbrake(bool value) => handbrake = value;

        private void FixedUpdate()
        {
            float steerAngle = steerInput * maxSteerAngle;
            frontLeftCollider.steerAngle = steerAngle;
            frontRightCollider.steerAngle = steerAngle;

            float motor = throttleInput * maxMotorTorque;
            rearLeftCollider.motorTorque = motor;
            rearRightCollider.motorTorque = motor;

            float brake = handbrake ? brakeTorque : 0f;
            rearLeftCollider.brakeTorque = brake;
            rearRightCollider.brakeTorque = brake;

            UpdateWheelMesh(frontLeftCollider, frontLeftMesh);
            UpdateWheelMesh(frontRightCollider, frontRightMesh);
            UpdateWheelMesh(rearLeftCollider, rearLeftMesh);
            UpdateWheelMesh(rearRightCollider, rearRightMesh);
        }

        private void UpdateWheelMesh(WheelCollider col, Transform mesh)
        {
            if (mesh == null) return;
            col.GetWorldPose(out Vector3 pos, out Quaternion rot);
            mesh.position = pos;
            mesh.rotation = rot;
        }
    }
}
