using UnityEngine;
using System.Collections.Generic;

namespace Biziwe.World
{
    [System.Serializable]
    public class GarageVehicle
    {
        public string vehicleName = "Sedan";
        public GameObject vehiclePrefab;
        public int price = 5000;
        public Vehicle.VehicleStats stats; // shown in the buy UI (top speed, etc.) for player info
    }

    /// <summary>
    /// A garage — buy new vehicles (real money leaves the balance, same
    /// pattern as ShopInterior), and spawn/retrieve owned ones at this
    /// location. Pairs with BuildingEntrance like a shop does.
    ///
    /// SETUP:
    /// 1. Add to a garage location in the world (doesn't need to be an
    ///    interior — garages can be an open-air lot with a spawn point).
    /// 2. Assign a spawnPoint (where purchased/retrieved vehicles appear).
    /// 3. Fill in the catalog with what this garage sells.
    /// </summary>
    public class VehicleGarage : MonoBehaviour
    {
        public string garageName = "Garage";
        public Transform spawnPoint;
        public GarageVehicle[] catalog;

        private HashSet<int> ownedVehicleIndices = new HashSet<int>();
        public System.Action<int, bool> OnPurchaseAttempt; // (catalogIndex, success)

        /// <summary>Call from a UI "Buy" button, passing the catalog index.</summary>
        public void TryPurchase(int catalogIndex)
        {
            if (catalog == null || catalogIndex < 0 || catalogIndex >= catalog.Length) return;
            if (ownedVehicleIndices.Contains(catalogIndex))
            {
                SpawnOwnedVehicle(catalogIndex); // already owned — just pull it out instead of buying again
                return;
            }

            var entry = catalog[catalogIndex];
            var economy = Systems.GameManager.Instance != null ? Systems.GameManager.Instance.Economy : null;
            if (economy == null || !economy.SpendMoney(entry.price))
            {
                OnPurchaseAttempt?.Invoke(catalogIndex, false);
                return;
            }

            ownedVehicleIndices.Add(catalogIndex);
            SpawnOwnedVehicle(catalogIndex);
            OnPurchaseAttempt?.Invoke(catalogIndex, true);
        }

        public bool IsOwned(int catalogIndex) => ownedVehicleIndices.Contains(catalogIndex);

        private void SpawnOwnedVehicle(int catalogIndex)
        {
            if (spawnPoint == null || catalog[catalogIndex].vehiclePrefab == null) return;

            Instantiate(catalog[catalogIndex].vehiclePrefab, spawnPoint.position, spawnPoint.rotation);
            // Note: doesn't check if the spawn point is already occupied by
            // another car — a fuller version would clear/offset the spot first.
            // Fine for Phase 2; revisit if it causes overlap issues in testing.
        }
    }
}
