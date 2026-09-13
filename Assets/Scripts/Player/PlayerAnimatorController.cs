using UnityEngine;

namespace Biziwe.Player
{
    /// <summary>
    /// Drives the player's Animator smoothly based on actual movement — the
    /// difference between a natural-looking walk and a stiff/robotic one is
    /// mostly in HOW animations blend, not just which clips exist. This
    /// smooths speed changes over time (SmoothDamp) instead of snapping
    /// instantly between Idle/Walk/Run, which is what usually causes that
    /// robotic look even with good animation clips.
    ///
    /// SETUP:
    /// 1. Get real walk/run/idle/jump animations — Mixamo (mixamo.com, free)
    ///    is the practical source: pick a character, download Walking,
    ///    Running, Idle, Jump animations set to "Without Skin" if using your
    ///    own model, or with the Mixamo character directly.
    /// 2. Import into Unity, set the model's Rig to Humanoid.
    /// 3. Create an Animator Controller with a Blend Tree: one parameter
    ///    "Speed" (float), blending Idle (0) → Walk (~0.5) → Run (1).
    /// 4. Add an Animator component to the player, assign the controller.
    /// 5. Add this script alongside PlayerController — it reads movement
    ///    state and drives the Animator's parameters automatically.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimatorController : MonoBehaviour
    {
        public PlayerController player;

        [Header("Blend Smoothing")]
        [Tooltip("Lower = snappier transitions, higher = smoother/floatier. 0.1-0.15 usually feels natural.")]
        public float speedSmoothTime = 0.12f;

        [Header("Animator Parameter Names")]
        public string speedParam = "Speed";
        public string groundedParam = "IsGrounded";
        public string jumpTrigger = "Jump";
        public string crouchParam = "IsCrouching";

        private Animator animator;
        private float currentAnimSpeed;
        private float speedVelocity;
        private CharacterController controller;
        private Vector3 lastPosition;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (player == null) player = GetComponent<PlayerController>();
            controller = GetComponent<CharacterController>();
            lastPosition = transform.position;
        }

        private void Update()
        {
            if (player == null || animator == null) return;

            // Derive actual horizontal movement speed from position delta —
            // more reliable than trusting input alone, since it reflects what
            // really happened (blocked by a wall, sliding, etc).
            Vector3 delta = transform.position - lastPosition;
            delta.y = 0f;
            float rawSpeed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            lastPosition = transform.position;

            // Normalize roughly against run speed so the blend tree's 0-1
            // range lines up with actual walk/run thresholds.
            float targetAnimSpeed = player.runSpeed > 0f ? Mathf.Clamp01(rawSpeed / player.runSpeed) : 0f;

            // The actual fix for "robotic" movement — smooth the transition
            // instead of setting it instantly every frame.
            currentAnimSpeed = Mathf.SmoothDamp(currentAnimSpeed, targetAnimSpeed, ref speedVelocity, speedSmoothTime);

            animator.SetFloat(speedParam, currentAnimSpeed);
            animator.SetBool(groundedParam, player.IsGrounded);
        }

        /// <summary>Call this from PlayerController's jump logic (or hook via an event later).</summary>
        public void TriggerJump()
        {
            animator.SetTrigger(jumpTrigger);
        }

        public void SetCrouching(bool crouching)
        {
            animator.SetBool(crouchParam, crouching);
        }
    }
}
