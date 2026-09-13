using UnityEngine;
using UnityEngine.UI;

namespace Biziwe.World
{
    /// <summary>
    /// Shows a short line of text floating above an NPC's head briefly — the
    /// visual for ambient street chatter. Simple world-space UI, not full
    /// dialogue UI (no portraits/choices — this is background flavor, not a
    /// conversation system).
    ///
    /// SETUP:
    /// 1. Create a World Space Canvas as a child of the NPC, positioned above
    ///    their head, small scale (e.g. 0.01, 0.01, 0.01) so it reads as a
    ///    speech-bubble size in the 3D world.
    /// 2. Add a Text (or TextMeshPro) component to it, assign to bubbleText.
    /// 3. Start the Canvas GameObject inactive — this script activates it only
    ///    while a line is showing.
    /// 4. Attach this script to the NPC (or the bubble object itself).
    /// </summary>
    public class DialogueBubble : MonoBehaviour
    {
        public GameObject bubbleRoot; // the World Space Canvas GameObject
        public Text bubbleText;
        public float displayDuration = 2.5f;

        private float hideTimer;
        private bool isShowing;

        public void Show(string line)
        {
            if (bubbleRoot == null) return;

            if (bubbleText != null) bubbleText.text = line;
            bubbleRoot.SetActive(true);
            isShowing = true;
            hideTimer = 0f;
        }

        private void Update()
        {
            if (!isShowing) return;

            hideTimer += Time.deltaTime;
            if (hideTimer >= displayDuration)
            {
                isShowing = false;
                if (bubbleRoot != null) bubbleRoot.SetActive(false);
            }

            // Simple billboard — keep the bubble facing the camera so it's
            // always readable regardless of NPC facing direction.
            if (bubbleRoot != null && Camera.main != null)
            {
                bubbleRoot.transform.LookAt(bubbleRoot.transform.position +
                    Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);
            }
        }
    }
}
