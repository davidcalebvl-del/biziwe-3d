using UnityEngine;

namespace Biziwe.Weapons
{
    public enum WeaponType { Pistol, SMG, Rifle, Shotgun, Sniper, RocketLauncher, Melee }

    /// <summary>
    /// Base weapon behaviour — covers pistols through the bazooka (RocketLauncher type).
    /// Phase 2.
    ///
    /// SETUP:
    /// 1. Create a prefab per weapon (or reuse one with different stat presets).
    /// 2. Attach to a "WeaponHolder" child on the player, positioned at hand height.
    /// 3. Assign a muzzlePoint (empty Transform at the barrel tip) and fireEffect/impactEffect.
    /// 4. Wire the on-screen fire button to Fire(), and a weapon-switch UI to enable/disable
    ///    the active weapon GameObject.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [Header("Identity")]
        public WeaponType type = WeaponType.Pistol;
        public string weaponName = "Pistol";

        [Header("Stats")]
        public float damage = 25f;
        public float fireRate = 0.25f; // seconds between shots
        public int magazineSize = 12;
        public float range = 60f;

        [Header("Rocket Launcher only")]
        public GameObject rocketProjectilePrefab; // an Explosive prefab, launched instead of a hitscan shot
        public float rocketSpeed = 25f;

        [Header("Refs")]
        public Transform muzzlePoint;
        public GameObject fireEffect;
        public GameObject impactEffect;
        public LayerMask hitLayers = ~0;

        private int currentAmmo;
        private float lastFireTime;

        private void Awake()
        {
            currentAmmo = magazineSize;
        }

        public bool CanFire => Time.time - lastFireTime >= fireRate && currentAmmo > 0;

        /// <summary>Call from the on-screen fire button.</summary>
        public void Fire(Vector3 aimDirection)
        {
            if (!CanFire) return;
            lastFireTime = Time.time;
            currentAmmo--;

            if (fireEffect != null && muzzlePoint != null)
                Instantiate(fireEffect, muzzlePoint.position, muzzlePoint.rotation);

            if (type == WeaponType.RocketLauncher)
            {
                FireRocket(aimDirection);
            }
            else
            {
                FireHitscan(aimDirection);
            }
        }

        private void FireHitscan(Vector3 aimDirection)
        {
            if (Physics.Raycast(muzzlePoint.position, aimDirection, out RaycastHit hit, range, hitLayers))
            {
                var damageable = hit.collider.GetComponent<Systems.IDamageable>();
                damageable?.TakeDamage(damage, hit.point, aimDirection);

                if (impactEffect != null)
                    Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        private void FireRocket(Vector3 aimDirection)
        {
            if (rocketProjectilePrefab == null || muzzlePoint == null) return;

            GameObject rocket = Instantiate(rocketProjectilePrefab, muzzlePoint.position,
                Quaternion.LookRotation(aimDirection));
            var rb = rocket.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = aimDirection.normalized * rocketSpeed;

            var explosive = rocket.GetComponent<Explosive>();
            explosive?.Arm(); // detonates on its own fuse, or add an impact-trigger variant later
        }

        public void Reload(int newAmmoCount)
        {
            currentAmmo = Mathf.Min(magazineSize, newAmmoCount);
        }

        public int CurrentAmmo => currentAmmo;
    }
}
