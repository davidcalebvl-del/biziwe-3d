using UnityEngine;

namespace Biziwe.World
{
    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public int price;
        public enum ItemType { Ammo, Weapon, Healing }
        public ItemType type;

        [Header("If type is Weapon — the weapon prefab to give the player")]
        public GameObject weaponPrefab;

        [Header("If type is Ammo — how much to add to the equipped weapon")]
        public int ammoAmount = 30;

        [Header("If type is Healing — how much health to restore")]
        public float healAmount = 50f;
    }

    /// <summary>
    /// A shop's actual purchasing logic — walk in (via BuildingEntrance), buy
    /// ammo, a new weapon, or healing, money actually leaves your balance.
    /// Ties together EconomySystem, WeaponHolder, and Health — real economy
    /// use, not just a number going up with nothing to spend it on.
    ///
    /// SETUP:
    /// 1. Add to a shop interior GameObject (the room the player teleports
    ///    into via BuildingEntrance).
    /// 2. Fill in the items array with what this particular shop sells.
    /// 3. Wire a "Buy" UI button per item to call TryPurchase(index) — pass
    ///    the item's index in the array.
    /// </summary>
    public class ShopInterior : MonoBehaviour
    {
        public string shopName = "Shop";
        public ShopItem[] items;

        public System.Action<int, bool> OnPurchaseAttempt; // (itemIndex, success) — for UI feedback

        /// <summary>Call from a UI "Buy" button, passing the item's index.</summary>
        public void TryPurchase(int itemIndex, GameObject player)
        {
            if (items == null || itemIndex < 0 || itemIndex >= items.Length || player == null) return;
            var item = items[itemIndex];

            var economy = Systems.GameManager.Instance != null ? Systems.GameManager.Instance.Economy : null;
            if (economy == null || !economy.SpendMoney(item.price))
            {
                OnPurchaseAttempt?.Invoke(itemIndex, false); // couldn't afford it
                return;
            }

            switch (item.type)
            {
                case ShopItem.ItemType.Healing:
                    var health = player.GetComponent<Systems.Health>();
                    health?.Heal(item.healAmount);
                    break;

                case ShopItem.ItemType.Ammo:
                    var holder = player.GetComponent<Weapons.WeaponHolder>();
                    var equipped = holder?.EquippedWeaponObject;
                    var weapon = equipped != null ? equipped.GetComponent<Weapons.Weapon>() : null;
                    weapon?.Reload(item.ammoAmount);
                    break;

                case ShopItem.ItemType.Weapon:
                    var weaponHolder = player.GetComponent<Weapons.WeaponHolder>();
                    if (weaponHolder != null && item.weaponPrefab != null)
                    {
                        GameObject newWeapon = Instantiate(item.weaponPrefab, weaponHolder.transform);
                        newWeapon.SetActive(false);
                        weaponHolder.weapons.Add(newWeapon);
                    }
                    break;
            }

            OnPurchaseAttempt?.Invoke(itemIndex, true);
        }
    }
}
