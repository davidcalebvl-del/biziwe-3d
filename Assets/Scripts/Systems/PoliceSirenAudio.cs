using UnityEngine;

namespace Biziwe.Systems
{
    /// <summary>
    /// Siren sound that plays while this police unit is actively chasing —
    /// works for both on-foot police (NpcController) and police vehicles
    /// (PoliceVehicleAI), checking whichever is present.
    ///
    /// SETUP:
    /// 1. Add to a police NPC or police car prefab.
    /// 2. Add an AudioSource (loop enabled), assign sirenClip.
    /// 3. This script starts/stops it automatically based on chase state —
    ///    no manual triggering needed.
    /// </summary>
    public class PoliceSirenAudio : MonoBehaviour
    {
        public AudioSource sirenSource;
        public AudioClip sirenClip;

        private AI.NpcController npcController;
        private AI.PoliceVehicleAI vehicleAI;
        private bool wasChasing;

        private void Awake()
        {
            npcController = GetComponent<AI.NpcController>();
            vehicleAI = GetComponent<AI.PoliceVehicleAI>();

            if (sirenSource != null)
            {
                sirenSource.clip = sirenClip;
                sirenSource.loop = true;
            }
        }

        private void Update()
        {
            bool isChasing = IsCurrentlyChasing();

            if (isChasing != wasChasing)
            {
                wasChasing = isChasing;
                if (sirenSource == null) return;

                if (isChasing) sirenSource.Play();
                else sirenSource.Stop();
            }
        }

        private bool IsCurrentlyChasing()
        {
            // Checks whichever AI component is present — reflection-free,
            // just checks both possibilities since a unit only has one.
            if (vehicleAI != null) return IsPoliceVehicleActive();
            if (npcController != null) return npcController.IsChasing;
            return false;
        }

        private bool IsPoliceVehicleActive()
        {
            // PoliceVehicleAI doesn't currently expose a public "is chasing"
            // flag — using isActiveAndEnabled as a reasonable proxy, since
            // PoliceDispatcher activates/deactivates the GameObject itself
            // when starting/stopping a chase (see PoliceDispatcher.cs).
            return vehicleAI.isActiveAndEnabled;
        }
    }
}
