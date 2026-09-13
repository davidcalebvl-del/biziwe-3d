using UnityEngine;
using UnityEngine.AI;

namespace Biziwe.AI
{
    /// <summary>
    /// Same idea as PlayerAnimatorController, for NPCs — smooths the blend
    /// between Idle/Walk/Run based on the NavMeshAgent's actual velocity, so
    /// civilians and police walking around the city look natural instead of
    /// gliding or snapping between poses. Reusable across CivilianNpc,
    /// NpcController, and CarjackVictim — attach alongside whichever of
    /// those is on a given NPC.
    ///
    /// SETUP: same as PlayerAnimatorController — get real walk/run/idle
    /// animations (Mixamo), set up a Blend Tree keyed on a "Speed" float,
    /// add this script alongside the NavMeshAgent.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class NpcAnimatorController : MonoBehaviour
    {
        [Header("Blend Smoothing")]
        public float speedSmoothTime = 0.12f;

        [Header("Animator Parameter Names")]
        public string speedParam = "Speed";

        private Animator animator;
        private NavMeshAgent agent;
        private float currentAnimSpeed;
        private float speedVelocity;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (animator == null || agent == null) return;

            // NavMeshAgent.velocity reflects actual current movement (including
            // being stopped/blocked), so this stays in sync with what's really
            // happening rather than just the agent's target speed setting.
            float rawSpeed = agent.velocity.magnitude;
            float targetAnimSpeed = agent.speed > 0f ? Mathf.Clamp01(rawSpeed / agent.speed) : 0f;

            currentAnimSpeed = Mathf.SmoothDamp(currentAnimSpeed, targetAnimSpeed, ref speedVelocity, speedSmoothTime);
            animator.SetFloat(speedParam, currentAnimSpeed);
        }
    }
}
