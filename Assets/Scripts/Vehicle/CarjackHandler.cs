using UnityEngine;
using Biziwe.Player;
using Biziwe.UI;

namespace Biziwe.Vehicle
{
    /// <summary>
    /// Carjacking — approach a vehicle, prompt appears, pull the driver out and take control.
    /// Phase 2.
    ///
    /// SETUP:
    /// 1. Add this to each drivable car alongside VehicleController.
    /// 2. Add a trigger Collider (slightly larger than the car) so the player can be detected nearby.
    /// 3. Wire an on-screen "Enter/Carjack" button to call TryEnterOrCarjack() when in range.
    /// </summary>
    [RequireComponent(typeof(VehicleController))]
    public class CarjackHandler : MonoBehaviour
    {
        public bool hasDriver = true; // NPC-driven cars start with a driver
        public GameObject driverVisual; // optional NPC model sitting in the seat
        public Systems.WantedSystem wantedSystem; // optional — carjacking in view of witnesses is a crime

        private VehicleController vehicleController;
        private bool playerInRange;
        private TouchInputManager touchInput;

        private void Awake()
        {
            vehicleController = GetComponent<VehicleController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                playerInRange = true;
                touchInput = FindObjectOfType<TouchInputManager>();
                // UI hook: show "Enter" or "Carjack" prompt here depending on hasDriver
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PlayerController>() != null)
                playerInRange = false;
        }

        /// <summary>Call this from the on-screen Enter/Carjack button.</summary>
        public void TryEnterOrCarjack()
        {
            if (!playerInRange || touchInput == null) return;

            if (hasDriver)
            {
                EjectDriver();
            }

            touchInput.EnterVehicle(vehicleController);
        }

        private void EjectDriver()
        {
            hasDriver = false;
            if (driverVisual != null)
            {
                var victim = driverVisual.GetComponent<Biziwe.AI.CarjackVictim>();
                if (victim != null)
                {
                    // Real reaction: shout, flee, seek another car, chase, then stop and fight.
                    driverVisual.transform.SetParent(null); // detach from the car
                    victim.BeginReaction(FindObjectOfType<PlayerController>()?.transform);
                }
                else
                {
                    driverVisual.SetActive(false); // fallback if no reaction script attached
                }

                wantedSystem?.ReportCrime(1); // carjacking in view of witnesses raises the wanted level
            }
        }
    }
}
