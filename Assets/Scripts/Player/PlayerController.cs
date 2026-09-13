using UnityEngine;

namespace Biziwe.Player
{
    /// <summary>
    /// Basic third-person character controller — Phase 1.
    /// Handles walking, running, jumping and crouching using Unity's CharacterController.
    ///
    /// Combat note: this script doesn't handle damage itself — add a Health
    /// component (Assets/Scripts/Systems/Health.cs) alongside it. That's what
    /// lets police, NPCs, and explosions actually hurt the player. This script
    /// subscribes to it in Awake() to disable movement on death.
    ///
    /// SETUP (do this in the Unity Editor once you have a PC):
    /// 1. Create a 3D capsule (or import a character model) as your player.
    /// 2. Add a CharacterController component to it (Unity adds this automatically
    ///    if you use GameObject > 3D Object > Capsule, but add manually if needed).
    /// 3. Add a Health component (Assets/Scripts/Systems/Health.cs).
    /// 4. Drag this script onto the player object.
    /// 5. Create an empty child GameObject called "CameraTarget" positioned at head height,
    ///    used by CameraFollow.cs.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Speeds")]
        public float walkSpeed = 3.5f;
        public float runSpeed = 7f;
        public float crouchSpeed = 1.8f;

        [Header("Jump & Gravity")]
        public float jumpHeight = 1.2f;
        public float gravity = -18f;

        [Header("Rotation")]
        public float turnSmoothTime = 0.1f;
        public Transform cameraTransform;

        private CharacterController controller;
        private Systems.Health health;
        private Vector3 velocity;
        private float turnSmoothVelocity;
        private bool isCrouching;
        private bool isDead;

        // Public state other scripts (AI, mission triggers) can read.
        public bool IsRunning { get; private set; }
        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;

            health = GetComponent<Systems.Health>();
            if (health != null)
                health.OnDeath += HandleDeath;
                health.OnRevive += HandleRevive;
        }

        private void OnDestroy()
        {
            if (health != null)
                health.OnDeath -= HandleDeath;
                health.OnRevive -= HandleRevive;
        }

        private void HandleDeath()
        {
            isDead = true;
            // Hook: play death/ragdoll animation, trigger a respawn or game-over
            // screen here once that UI exists.
        }

        private void HandleRevive()
        {
            isDead = false;
            velocity = Vector3.zero; // clear any leftover fall speed from before death
        }

        private void Update()
        {
            if (isDead) return;

            IsGrounded = controller.isGrounded;
            if (IsGrounded && velocity.y < 0)
                velocity.y = -2f; // small downward force to keep grounded reliably

            HandleCrouch();
            HandleMovement();
            HandleJump();

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        private void HandleCrouch()
        {
            // Touch UI or keyboard "C" can call ToggleCrouch() — wired up in Phase 1 UI.
        }

        public void ToggleCrouch()
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? 1f : 2f;
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal"); // wire to on-screen joystick later
            float vertical = Input.GetAxisRaw("Vertical");
            IsRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

            Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

            if (inputDir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg
                                     + (cameraTransform != null ? cameraTransform.eulerAngles.y : 0f);
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                    ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                float speed = isCrouching ? crouchSpeed : (IsRunning ? runSpeed : walkSpeed);
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }
        }

        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && IsGrounded && !isCrouching)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
    }
}
