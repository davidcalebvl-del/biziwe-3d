using UnityEngine;
using Biziwe.Player;

namespace Biziwe.World
{
    /// <summary>
    /// Enter/exit a building — teleports the player between the exterior world
    /// and an interior space. Not every building needs this (per the vision
    /// doc); attach only to the ones meant to be enterable (shops, houses,
    /// police stations, etc).
    ///
    /// Two ways to build the interior, your choice per building:
    /// A) "Interior pocket" — build the interior room somewhere far away in the
    ///    same scene (e.g. 500 units below the city), and this script just
    ///    teleports the player there and back. Simple, no loading screen,
    ///    works great for small interiors. This is what the default setup below
    ///    assumes.
    /// B) Additive scene loading — for bigger interiors, load a separate Unity
    ///    Scene additively (SceneManager.LoadSceneAsync(..., LoadSceneMode.Additive))
    ///    when entering, unload when exiting. More setup, but keeps the main
    ///    city scene lighter. Swap in later if interiors get complex.
    ///
    /// SETUP:
    /// 1. Add a trigger Collider at the building's door.
    /// 2. Create the interior geometry (or an "interior pocket" placed far from
    ///    the city, e.g. at Y = -500) with its own exit trigger.
    /// 3. Assign entryPoint (inside spawn location) and the matching exit
    ///    trigger's exitPoint (outside spawn location, back at the door).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class BuildingEntrance : MonoBehaviour
    {
        public string buildingName = "Shop";
        public Transform entryPoint;   // where the player appears once inside
        public Transform exteriorExit; // where the player appears once they leave (usually just outside this door)

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player == null || entryPoint == null) return;

            TeleportPlayer(player, entryPoint);
            // Hook: fade screen to black briefly here for a clean transition once
            // you have UI for it — instant teleport is fine for Phase 1/2.
        }

        /// <summary>Call this from an interior exit trigger to send the player back outside.</summary>
        public void ExitToExterior(PlayerController player)
        {
            if (player == null || exteriorExit == null) return;
            TeleportPlayer(player, exteriorExit);
        }

        private void TeleportPlayer(PlayerController player, Transform destination)
        {
            var controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false; // avoid CharacterController fighting the teleport

            player.transform.position = destination.position;
            player.transform.rotation = destination.rotation;

            if (controller != null) controller.enabled = true;
        }
    }
}
