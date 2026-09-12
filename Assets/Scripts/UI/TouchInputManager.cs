using UnityEngine;
using UnityEngine.EventSystems;
using Biziwe.Player;
using Biziwe.Vehicle;
using Biziwe.Weapons;

namespace Biziwe.UI
{
    /// <summary>
    /// Connects on-screen touch buttons to PlayerController / VehicleController /
    /// WeaponHolder. Phase 1 — mobile-first, this is how the game is actually
    /// meant to be played.
    ///
    /// SETUP:
    /// 1. Create a Canvas (Screen Space - Overlay).
    /// 2. Add a Joystick graphic (or use Unity's free "Joystick Pack" asset from
    ///    the Asset Store — much faster than building one from scratch) for movement.
    /// 3. Add UI Buttons for: Jump, Crouch, Run (toggle), Enter/Exit Vehicle, Fire,
    ///    Switch Weapon, Handbrake. Use EventTrigger components (PointerDown/PointerUp)
    ///    or Button OnClick for taps.
    /// 4. Drag this script onto an empty "TouchInputManager" GameObject, assign the
    ///    player, weaponHolder, and (when in a vehicle) the active VehicleController.
    /// </summary>
    public class TouchInputManager : MonoBehaviour
    {
        public PlayerController player;
        public WeaponHolder weaponHolder;
        public VehicleController activeVehicle; // null when on foot

        [Header("Joystick (assign from your joystick asset)")]
        public float joystickX; // set by your joystick component's output
        public float joystickY;

        public bool InVehicle => activeVehicle != null;

        // ---- Called from UI Button events ----
        public void OnJumpPressed() => player?.SendMessage("HandleJump", SendMessageOptions.DontRequireReceiver);
        public void OnCrouchToggle() => player?.ToggleCrouch();
        public void OnFirePressed() => weaponHolder?.UseEquippedWeapon();
        public void OnSwitchWeaponPressed() => weaponHolder?.SwitchNext();

        public void OnSteerLeftHeld(bool held) => activeVehicle?.SetSteer(held ? -1f : 0f);
        public void OnSteerRightHeld(bool held) => activeVehicle?.SetSteer(held ? 1f : 0f);
        public void OnGasHeld(bool held) => activeVehicle?.SetThrottle(held ? 1f : 0f);
        public void OnBrakeHeld(bool held) => activeVehicle?.SetThrottle(held ? -1f : 0f);
        public void OnHandbrakeHeld(bool held) => activeVehicle?.SetHandbrake(held);

        public void EnterVehicle(VehicleController vehicle)
        {
            activeVehicle = vehicle;
            if (player != null) player.gameObject.SetActive(false);
        }

        public void ExitVehicle()
        {
            if (activeVehicle != null && player != null)
            {
                player.transform.position = activeVehicle.transform.position + activeVehicle.transform.right * 2f;
                player.gameObject.SetActive(true);
            }
            activeVehicle = null;
        }
    }
}
