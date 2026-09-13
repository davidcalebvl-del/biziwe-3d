using UnityEngine;
using System.Collections;

namespace Biziwe.Player
{
    /// <summary>
    /// What actually happens when the player's health hits zero — a real
    /// respawn flow, not just movement freezing. Matches the GTA-style
    /// convention: brief "you died" screen, wake up at a hospital (or safe
    /// point), lose some money as the cost, wanted level clears.
    ///
    /// SETUP:
    /// 1. Add to the player alongside PlayerController and Health.
    /// 2. Assign one or more respawnPoints (hospital locations in the city —
    ///    even just one to start is fine).
    /// 3. Assign deathScreen (a UI panel — "YOU DIED" text or similar,
    ///    started inactive).
    /// </summary>
    public class PlayerRespawn : MonoBehaviour
    {
        public Transform[] respawnPoints;
        public GameObject deathScreen;
        public float deathScreenDuration = 3f;

        [Header("Cost of Dying")]
        [Tooltip("Percentage of current money lost on death (hospital bill) — 0 to disable.")]
        [Range(0f, 1f)] public float moneyLossPercent = 0.1f;

        private Systems.Health health;
        private CharacterController controller;

        private void Awake()
        {
            health = GetComponent<Systems.Health>();
            controller = GetComponent<CharacterController>();

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
            StartCoroutine(DeathSequence());
        }

        private IEnumerator DeathSequence()
        {
            if (deathScreen != null) deathScreen.SetActive(true);

            yield return new WaitForSeconds(deathScreenDuration);

            RespawnAtNearestPoint();

            if (deathScreen != null) deathScreen.SetActive(false);
        }

        private void RespawnAtNearestPoint()
        {
            if (respawnPoints == null || respawnPoints.Length == 0)
            {
                Debug.LogWarning("PlayerRespawn: no respawn points assigned — player will stay dead in place.");
                return;
            }

            Transform nearest = GetNearestRespawnPoint();

            // Teleport safely (disable CharacterController briefly, same
            // approach BuildingEntrance uses, to avoid it fighting the move).
            if (controller != null) controller.enabled = false;
            transform.position = nearest.position;
            transform.rotation = nearest.rotation;
            if (controller != null) controller.enabled = true;

            health?.Revive();

            var gm = Systems.GameManager.Instance;
            if (gm != null)
            {
                // Clear the heat — a fresh start at the hospital, matching genre convention.
                if (gm.Wanted != null) gm.Wanted.wantedLevel = 0;

                // Hospital bill — a real cost for dying, not a free reset.
                if (gm.Economy != null && moneyLossPercent > 0f)
                {
                    int loss = Mathf.RoundToInt(gm.Economy.CurrentBalance * moneyLossPercent);
                    if (loss > 0) gm.Economy.SpendMoney(loss);
                }
            }
        }

        private Transform GetNearestRespawnPoint()
        {
            Transform nearest = respawnPoints[0];
            float nearestDist = Vector3.Distance(transform.position, nearest.position);

            for (int i = 1; i < respawnPoints.Length; i++)
            {
                float dist = Vector3.Distance(transform.position, respawnPoints[i].position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = respawnPoints[i];
                }
            }
            return nearest;
        }
    }
}
