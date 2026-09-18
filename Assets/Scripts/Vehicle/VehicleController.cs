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
        [Tooltip("Overridden by 'stats' below if assigned — these fields stay as the fallback/manual defaults.")]
        public float maxMotorTorque = 1500f;
        public float maxSteerAngle = 30f;
        public float brakeTorque = 3000f;

        [Header("Vehicle Type (optional)")]
        [Tooltip("Assign a VehicleStats asset for this vehicle's category to auto-apply its handling numbers on Awake. Leave blank to use the manual fields above as-is.")]
        public VehicleStats stats;

        [Header("Engine Audio (optional)")]
        public AudioSource engineAudioSource; // assign a looping engine sound here
        [Range(0.5f, 1f)] public float minEnginePitch = 0.7f;
        [Range(1f, 3f)] public float maxEnginePitch = 1.8f;
        [Range(0f, 1f)] public float engineBaseVolume = 0.7f; // before the player's SFX Settings slider is applied

        private float steerInput;   // -1 to 1, wire to on-screen left/right buttons
        private float throttleInput; // -1 to 1, wire to on-screen gas/brake buttons
        private bool handbrake;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = new Vector3(0f, -0.5f, 0f); // lower COM for stability

            if (stats != null)
            {
                maxMotorTorque = stats.maxMotorTorque;
                maxSteerAngle = stats.maxSteerAngle;
                brakeTorque = stats.brakeTorque;
                rb.mass = stats.mass;
            }

            if (engineAudioSource != null)
            {
                engineAudioSource.loop = true;
                if (!engineAudioSource.isPlaying) engineAudioSource.Play();
            }
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

            UpdateEngineSound();
        }

        private void UpdateEngineSound()
        {
            if (engineAudioSource == null) return;

            // Pitch rises with speed AND throttle input — gives that
            // "revving" feel even from a standstill, not just while moving.
            float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / 25f);
            float throttleFactor = Mathf.Abs(throttleInput) * 0.3f;
            engineAudioSource.pitch = Mathf.Lerp(minEnginePitch, maxEnginePitch, speedFactor + throttleFactor);
            engineAudioSource.volume = engineBaseVolume * Systems.AudioPreferences.SfxVolume;
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
