using UnityEngine;
using Biziwe.Player;

namespace Biziwe.World
{
    /// <summary>
    /// Place inside a building's interior — walking into this sends the player
    /// back outside via the matching BuildingEntrance. Pairs with
    /// BuildingEntrance.cs; see that script's header for full setup notes.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class InteriorExit : MonoBehaviour
    {
        public BuildingEntrance parentEntrance;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player == null || parentEntrance == null) return;

            parentEntrance.ExitToExterior(player);
        }
    }
}
