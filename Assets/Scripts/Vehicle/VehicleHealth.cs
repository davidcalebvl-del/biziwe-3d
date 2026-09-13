using UnityEngine;
using Biziwe.UI;

namespace Biziwe.Vehicle
{
    /// <summary>
    /// Vehicle health — cars take damage from weapons, explosions, and heavy
    /// impacts, show visual distress as they get low, and explode when
    /// destroyed (matching "vehicle health" and "damage" from the vision doc).
    /// Implements IDamageable so Weapon.cs and Explosive.cs already work
    /// against it with zero changes to those scripts.
    ///
    /// SETUP:
    /// 1. Add to every drivable/traffic car prefab, alongside VehicleController.
    /// 2. Assign smokeEffect (plays once damaged below smokeThreshold) and
    ///    explosionPrefab (an Explosive.cs prefab — reuses the same blast/area
    ///    damage logic bombs use, so a destroyed car damages what's around it too).
    /// </summary>
    [RequireComponent(typeof(VehicleController))]
    public class VehicleHealth : MonoBehaviour, Systems.IDamageable
    {
        public float maxHealth = 200f;
        public float CurrentHealth { get; private set; }
        public bool IsDestroyed { get; private set; }

        [Header("Vehicle Type (optional)")]
        [Tooltip("If this vehicle also has a VehicleController with a Vehicle Stats asset assigned, its maxHealth value is used instead of the field above.")]
        public VehicleStats stats;

        [Header("Damage Feedback")]
        [Range(0f, 1f)] public float smokeThreshold = 0.4f; // starts smoking below 40% health
        public GameObject smokeEffect;
        private bool smokeStarted;

        [Header("Destruction")]
        public GameObject explosionPrefab; // an Explosive.cs prefab, reused for the death blast
        public GameObject wreckPrefab;      // optional burnt-out wreck visual to swap in

        private VehicleController vehicleController;

        private void Awake()
        {
            if (stats != null) maxHealth = stats.maxHealth;
            CurrentHealth = maxHealth;
            vehicleController = GetComponent<VehicleController>();
        }

        public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDirection)
        {
            if (IsDestroyed) return;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);

            if (!smokeStarted && CurrentHealth <= maxHealth * smokeThreshold)
            {
                smokeStarted = true;
                if (smokeEffect != null) smokeEffect.SetActive(true);
            }

            if (CurrentHealth <= 0f)
            {
                DestroyVehicle();
            }
        }

        private void DestroyVehicle()
        {
            IsDestroyed = true;
            vehicleController.enabled = false; // no longer drivable

            // Force the driver out if the player is currently inside this car.
            var touchInput = FindObjectOfType<TouchInputManager>();
            if (touchInput != null && touchInput.activeVehicle == vehicleController)
            {
                touchInput.ExitVehicle();
            }

            if (explosionPrefab != null)
            {
                var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                var explosive = explosion.GetComponent<Weapons.Explosive>();
                explosive?.Detonate(); // immediate — a destroyed car doesn't wait on a fuse timer
            }

            if (wreckPrefab != null)
            {
                Instantiate(wreckPrefab, transform.position, transform.rotation);
            }

            gameObject.SetActive(false); // simple version — swap for object pooling later if needed
        }
    }
}
