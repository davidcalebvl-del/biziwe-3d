using UnityEngine;
using System.Collections.Generic;

namespace Biziwe.Weapons
{
    /// <summary>
    /// Manages the player's equipped weapon and switching between owned weapons —
    /// ties together Weapon.cs (ranged) and MeleeWeapon.cs (knife) under one
    /// consistent interface for the UI to call. Phase 2.
    ///
    /// SETUP:
    /// 1. Add to the player, alongside PlayerController.
    /// 2. Child all owned weapon GameObjects (each with a Weapon or MeleeWeapon
    ///    component) under the player's weapon holder, all inactive except the
    ///    starting one.
    /// 3. Assign them to the weapons list in Inspector order — this becomes the
    ///    cycle order for the Switch Weapon button.
    /// 4. Wire the on-screen Fire button to Fire(), and Switch button to
    ///    SwitchNext().
    /// </summary>
    public class WeaponHolder : MonoBehaviour
    {
        [Tooltip("Every weapon GameObject the player owns, each with a Weapon or MeleeWeapon component.")]
        public List<GameObject> weapons = new List<GameObject>();

        public Transform aimReference; // camera or player forward, used as fire direction source

        private int equippedIndex = -1;

        private void Start()
        {
            if (weapons.Count > 0) Equip(0);
        }

        public void Equip(int index)
        {
            if (index < 0 || index >= weapons.Count) return;

            for (int i = 0; i < weapons.Count; i++)
                weapons[i].SetActive(i == index);

            equippedIndex = index;
        }

        public void SwitchNext()
        {
            if (weapons.Count == 0) return;
            int next = (equippedIndex + 1) % weapons.Count;
            Equip(next);
        }

        public void SwitchPrevious()
        {
            if (weapons.Count == 0) return;
            int prev = (equippedIndex - 1 + weapons.Count) % weapons.Count;
            Equip(prev);
        }

        /// <summary>Call from the on-screen Fire/Attack button — works for either weapon type.</summary>
        public void UseEquippedWeapon()
        {
            if (equippedIndex < 0 || equippedIndex >= weapons.Count) return;
            GameObject current = weapons[equippedIndex];

            var ranged = current.GetComponent<Weapon>();
            if (ranged != null)
            {
                Vector3 direction = aimReference != null ? aimReference.forward : transform.forward;
                ranged.Fire(direction);
                return;
            }

            var melee = current.GetComponent<MeleeWeapon>();
            melee?.Attack();
        }

        public GameObject EquippedWeaponObject =>
            (equippedIndex >= 0 && equippedIndex < weapons.Count) ? weapons[equippedIndex] : null;
    }
}
