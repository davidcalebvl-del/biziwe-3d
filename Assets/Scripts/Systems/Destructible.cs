using UnityEngine;

namespace Biziwe.Systems
{
    /// <summary>
    /// Generic destructible object — attach to shopfronts, houses, or cars so
    /// explosions and weapon fire can damage them. Phase 2/3.
    ///
    /// SETUP:
    /// 1. Add to any GameObject you want destructible (a shop, a house, a parked car).
    /// 2. Optionally assign a "destroyedPrefab" (broken/rubble version) to swap in
    ///    when health reaches zero, and a particle effect for the moment of destruction.
    /// </summary>
    public class Destructible : MonoBehaviour, IDamageable
    {
        public float maxHealth = 100f;
        public GameObject destroyedPrefab;
        public GameObject destructionEffect;

        private float currentHealth;
        private bool isDestroyed;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDirection)
        {
            if (isDestroyed) return;

            currentHealth -= amount;
            if (currentHealth <= 0f)
            {
                DestroyObject();
            }
        }

        private void DestroyObject()
        {
            isDestroyed = true;

            if (destructionEffect != null)
                Instantiate(destructionEffect, transform.position, Quaternion.identity);

            if (destroyedPrefab != null)
                Instantiate(destroyedPrefab, transform.position, transform.rotation);

            gameObject.SetActive(false); // simple version — swap for a proper destroy/pool later
        }
    }
}
