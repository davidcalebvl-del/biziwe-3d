using System.Collections.Generic;
using UnityEngine;
using Biziwe.AI;

namespace Biziwe.Systems
{
    /// <summary>
    /// Basic wanted-level system — Phase 1.
    /// Tracks a 0-3 star wanted level (3 stars is the max, by design — not 5 like
    /// some other games). At level 1+, nearby police NPCs start chasing the player.
    /// Wanted level decays over time if the player stays out of police sight.
    ///
    /// SETUP:
    /// 1. Add this script to an empty GameObject called "GameManager" in your scene.
    /// 2. Assign the player's transform.
    /// 3. Tag all police NPCs with the "Police" tag (Unity Inspector > Tag dropdown > Add Tag).
    /// 4. UI: bind wantedLevel to a 3-star icon display (fill stars 1 to 3 as it rises).
    /// </summary>
    public class WantedSystem : MonoBehaviour
    {
        public Transform player;
        public int wantedLevel = 0;
        public int maxWantedLevel = 3; // hard cap at 3 stars by design

        [Header("Decay")]
        public float secondsPerDecay = 8f;
        private float decayTimer;

        [Header("Detection")]
        public float policeAlertRadius = 25f;

        private List<NpcController> activePolice = new List<NpcController>();

        public void ReportCrime(int severity = 1)
        {
            wantedLevel = Mathf.Clamp(wantedLevel + severity, 0, maxWantedLevel);
            decayTimer = 0f;
            AlertNearbyPolice();
        }

        private void Update()
        {
            if (wantedLevel <= 0) return;

            decayTimer += Time.deltaTime;
            if (decayTimer >= secondsPerDecay)
            {
                decayTimer = 0f;
                wantedLevel = Mathf.Max(0, wantedLevel - 1);
                if (wantedLevel == 0)
                    CallOffChase();
            }
        }

        private void AlertNearbyPolice()
        {
            Collider[] hits = Physics.OverlapSphere(player.position, policeAlertRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Police"))
                {
                    NpcController npc = hit.GetComponent<NpcController>();
                    if (npc != null && !activePolice.Contains(npc))
                    {
                        npc.StartChase(player);
                        activePolice.Add(npc);
                    }
                }
            }
        }

        private void CallOffChase()
        {
            foreach (var npc in activePolice)
            {
                if (npc != null) npc.StopChase();
            }
            activePolice.Clear();
        }
    }
}
