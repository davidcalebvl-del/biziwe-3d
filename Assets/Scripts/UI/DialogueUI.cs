using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Biziwe.UI
{
    /// <summary>
    /// On-screen dialogue box for mission conversations — speaker name, line
    /// text, tap/click to advance through a sequence. This is what the Act 1
    /// mission scripts (Mission_TheErrand.cs etc.) hook into instead of
    /// logging lines to the Console.
    ///
    /// SETUP:
    /// 1. Build a dialogue panel: speaker name Text, line Text, a "tap to
    ///    continue" indicator, optionally a portrait Image.
    /// 2. Start the panel inactive.
    /// 3. Add this script to the panel, assign the Text/Image references.
    /// 4. Wire a full-screen invisible Button (or just call AdvanceLine()
    ///    from any tap-to-continue input) over the dialogue panel.
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        public static DialogueUI Instance { get; private set; }

        public GameObject panelRoot;
        public Text speakerText;
        public Text lineText;
        public Image portraitImage; // optional — leave unassigned if not using portraits

        public event System.Action OnSequenceComplete;

        private Queue<(string speaker, string line)> queue = new Queue<(string, string)>();
        private bool isShowing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple DialogueUI instances found — destroying the extra one.");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (panelRoot != null) panelRoot.SetActive(false);
        }

        /// <summary>Show a single line with a speaker name — e.g. "Femi: Take this to the drop point."</summary>
        public void ShowLine(string speaker, string line)
        {
            queue.Clear();
            queue.Enqueue((speaker, line));
            BeginSequence();
        }

        /// <summary>
        /// Show a full conversation. Accepts lines already formatted as
        /// "Speaker: text" (matching how the mission scripts currently write
        /// them) and splits on the first colon automatically.
        /// </summary>
        public void ShowSequence(IEnumerable<string> lines)
        {
            queue.Clear();
            foreach (var raw in lines)
            {
                int colonIndex = raw.IndexOf(':');
                if (colonIndex > 0 && colonIndex < raw.Length - 1)
                {
                    string speaker = raw.Substring(0, colonIndex).Trim();
                    string text = raw.Substring(colonIndex + 1).Trim();
                    queue.Enqueue((speaker, text));
                }
                else
                {
                    queue.Enqueue(("", raw)); // no "Speaker:" prefix found — show as-is
                }
            }
            BeginSequence();
        }

        private void BeginSequence()
        {
            if (queue.Count == 0) return;
            isShowing = true;
            if (panelRoot != null) panelRoot.SetActive(true);
            DisplayNext();
        }

        /// <summary>Wire to a tap/click-anywhere-to-continue input.</summary>
        public void AdvanceLine()
        {
            if (!isShowing) return;

            if (queue.Count > 0)
            {
                DisplayNext();
            }
            else
            {
                EndSequence();
            }
        }

        private void DisplayNext()
        {
            var (speaker, line) = queue.Dequeue();
            if (speakerText != null) speakerText.text = speaker;
            if (lineText != null) lineText.text = line;
        }

        private void EndSequence()
        {
            isShowing = false;
            if (panelRoot != null) panelRoot.SetActive(false);
            OnSequenceComplete?.Invoke();
        }
    }
}
