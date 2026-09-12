using UnityEngine;

namespace Biziwe.Systems
{
    /// <summary>
    /// Anything that can take damage implements this — NPCs, police, the player,
    /// cars, and destructible shop/house objects all use the same interface so
    /// weapons and explosives don't need special-case code per object type.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitDirection);
    }
}
