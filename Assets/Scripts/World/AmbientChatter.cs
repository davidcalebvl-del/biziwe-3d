using UnityEngine;
using System.Collections.Generic;

namespace Biziwe.World
{
    /// <summary>
    /// Makes the streets feel alive — periodically finds pairs of civilians
    /// standing near each other, close to the player, and has them briefly
    /// "talk" (pause, face each other, show a dialogue bubble). Only runs
    /// this check near the player (same philosophy as NpcSpawner's population
    /// streaming) so it stays cheap even with many civilians in the world.
    ///
    /// This is the real, buildable version of "make the streets feel alive" —
    /// not simulating conversations for every NPC in the city at once, just
    /// the ones close enough for the player to actually notice.
    ///
    /// SETUP:
    /// 1. Add to an empty "AmbientChatterManager" GameObject in the scene.
    /// 2. Assign the player transform and an AmbientLines asset.
    /// 3. Civilians need a DialogueBubble component (with its world-space
    ///    canvas set up) for the line to actually be visible.
    /// </summary>
    public class AmbientChatter : MonoBehaviour
    {
        public Transform player;
        public AmbientLines lineBank;

        [Header("Ranges")]
        public float playerActivationRadius = 25f; // only look for chatter pairs this close to the player
        public float pairDistance = 2.5f;           // civilians this close to each other count as "near enough to talk"

        [Header("Timing")]
        public float checkInterval = 3f;
        private float checkTimer;

        private void Update()
        {
            if (player == null || lineBank == null || lineBank.lines.Length == 0) return;

            checkTimer += Time.deltaTime;
            if (checkTimer < checkInterval) return;
            checkTimer = 0f;

            TryStartAConversation();
        }

        private void TryStartAConversation()
        {
            Collider[] nearby = Physics.OverlapSphere(player.position, playerActivationRadius);
            List<AI.CivilianNpc> candidates = new List<AI.CivilianNpc>();

            foreach (var col in nearby)
            {
                var civilian = col.GetComponent<AI.CivilianNpc>();
                if (civilian != null && civilian.IsAvailableToTalk)
                    candidates.Add(civilian);
            }

            // Find any pair of candidates standing close enough to each other.
            for (int i = 0; i < candidates.Count; i++)
            {
                for (int j = i + 1; j < candidates.Count; j++)
                {
                    float dist = Vector3.Distance(candidates[i].transform.position, candidates[j].transform.position);
                    if (dist <= pairDistance)
                    {
                        string line = lineBank.lines[Random.Range(0, lineBank.lines.Length)];
                        candidates[i].StartTalking(line, candidates[j].transform);
                        candidates[j].StartTalking(PickReply(line), candidates[i].transform);
                        return; // one conversation per check is plenty — keeps it natural, not chaotic
                    }
                }
            }
        }

        private string PickReply(string openingLine)
        {
            // Simple version: a second random line as the "reply" — not a real
            // response to what was said, just believable ambient back-and-forth.
            // A smarter reply-matching system is a good later upgrade once this
            // basic version is confirmed working and feels right in-game.
            return lineBank.lines[Random.Range(0, lineBank.lines.Length)];
        }
    }
}
