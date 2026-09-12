using UnityEngine;
using Biziwe.Systems;

namespace Biziwe.Weapons
{
    /// <summary>
    /// Close-range melee weapon (knife, bat, etc). Separate from ranged Weapon.cs
    /// since melee works on proximity + a swing/stab trigger rather than a raycast.
    /// Supports a "finisher" instant-kill when attacking an unaware target from
    /// behind — the classic stealth-knife takedown.
    ///
    /// SETUP:
    /// 1. Attach to the player's weapon holder alongside/instead of a ranged Weapon.
    /// 2. Assign an attackPoint (empty Transform slightly in front of the player)
    ///    and set attackRange/attackRadius to match your character's reach.
    /// 3. Wire the on-screen "Attack" button to Attack().
    /// </summary>
    public class MeleeWeapon : MonoBehaviour
    {
        public string weaponName = "Knife";
        public float damage = 40f;
        public float finisherDamage = 9999f; // effectively instant-kill for a stealth takedown
        public float attackRange = 1.5f;
        public float attackRadius = 0.6f;
        public float attackCooldown = 0.6f;

        public Transform attackPoint;
        public LayerMask hitLayers = ~0;
        public GameObject hitEffect;

        private float lastAttackTime;

        public bool CanAttack => Time.time - lastAttackTime >= attackCooldown;

        /// <summary>Call from the on-screen Attack button.</summary>
        public void Attack()
        {
            if (!CanAttack || attackPoint == null) return;
            lastAttackTime = Time.time;

            Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, hitLayers);
            foreach (var hit in hits)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable == null) continue;

                bool isFinisher = IsBehindTarget(hit.transform);
                float dealtDamage = isFinisher ? finisherDamage : damage;

                damageable.TakeDamage(dealtDamage, hit.ClosestPoint(attackPoint.position),
                    (hit.transform.position - transform.position).normalized);

                if (hitEffect != null)
                    Instantiate(hitEffect, hit.ClosestPoint(attackPoint.position), Quaternion.identity);
            }
        }

        /// <summary>
        /// A finisher triggers when attacking a target's back — the stealth-takedown feel.
        /// </summary>
        private bool IsBehindTarget(Transform target)
        {
            Vector3 toAttacker = (transform.position - target.position).normalized;
            float dot = Vector3.Dot(target.forward, toAttacker);
            return dot > 0.5f; // attacker is roughly behind the target's facing direction
        }
    }
}
