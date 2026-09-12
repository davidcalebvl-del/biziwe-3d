using UnityEngine;

namespace Biziwe.Weapons
{
    /// <summary>
    /// Bomb / explosive — deals area damage to everything in the blast radius:
    /// cars, shopfronts, houses, NPCs, police, the player. Exactly the "the bomb
    /// blasts the car and the shops/houses around it" behaviour requested.
    ///
    /// SETUP:
    /// 1. Create a prefab for the bomb (a simple model is fine for Phase 1/2).
    /// 2. Attach this script and a Rigidbody (so it can be thrown/dropped/driven over).
    /// 3. Set blastRadius, damage, and fuseTime as desired per bomb type.
    /// 4. Optionally assign an explosionEffect (particle system) and explosionSound.
    /// </summary>
    public class Explosive : MonoBehaviour
    {
        [Header("Blast Settings")]
        public float blastRadius = 8f;
        public float maxDamage = 500f;
        public float upwardsForce = 3f;
        public float explosionForce = 1200f;

        [Header("Timing")]
        public float fuseTime = 3f;
        private bool armed;
        private float fuseTimer;

        [Header("Effects")]
        public GameObject explosionEffect;
        public AudioClip explosionSound;

        [Header("Layers")]
        public LayerMask affectedLayers = ~0; // everything by default

        public void Arm()
        {
            armed = true;
            fuseTimer = 0f;
        }

        private void Update()
        {
            if (!armed) return;
            fuseTimer += Time.deltaTime;
            if (fuseTimer >= fuseTime)
                Detonate();
        }

        /// <summary>Call directly for remote-detonated or impact-triggered bombs.</summary>
        public void Detonate()
        {
            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            if (explosionSound != null)
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);

            Collider[] hits = Physics.OverlapSphere(transform.position, blastRadius, affectedLayers);
            foreach (var hit in hits)
            {
                // Damage — falls off with distance from the blast center.
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float falloff = Mathf.Clamp01(1f - (distance / blastRadius));
                float damage = maxDamage * falloff;

                var damageable = hit.GetComponent<Systems.IDamageable>();
                damageable?.TakeDamage(damage, hit.ClosestPoint(transform.position),
                    (hit.transform.position - transform.position).normalized);

                // Physics push — sends nearby cars/props flying, classic explosion feel.
                var rb = hit.attachedRigidbody;
                if (rb != null)
                    rb.AddExplosionForce(explosionForce, transform.position, blastRadius, upwardsForce);
            }

            Destroy(gameObject);
        }
    }
}
