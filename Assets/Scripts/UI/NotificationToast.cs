using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Biziwe.UI
{
    /// <summary>
    /// Pop-up banner for game events — "Mission Started", "Mission Complete
    /// +$200", "Wanted Level Up" — instead of those events only quietly
    /// updating a corner text label. Queues messages so a burst of events
    /// (e.g. mission complete + reward) doesn't overlap or get lost.
    ///
    /// SETUP:
    /// 1. Build a small banner: a background panel + a Text, usually docked
    ///    near the top of the screen.
    /// 2. Start the panel inactive.
    /// 3. Add this script to the panel's parent, assign panel + text.
    /// 4. Anything can call NotificationToast.Instance.Show("message") —
    ///    GameHUD is already wired to call it for mission/wanted events.
    /// </summary>
    public class NotificationToast : MonoBehaviour
    {
        public static NotificationToast Instance { get; private set; }

        public GameObject panel;
        public Text messageText;
        public CanvasGroup canvasGroup; // optional — enables fade in/out; leave unassigned for instant show/hide

        [Header("Timing")]
        public float displayDuration = 2.5f;
        public float fadeDuration = 0.25f;

        private readonly Queue<string> queue = new Queue<string>();
        private bool isShowing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (panel != null) panel.SetActive(false);
        }

        public void Show(string message)
        {
            queue.Enqueue(message);
            if (!isShowing) StartCoroutine(ProcessQueue());
        }

        private IEnumerator ProcessQueue()
        {
            isShowing = true;

            while (queue.Count > 0)
            {
                string message = queue.Dequeue();
                if (messageText != null) messageText.text = message;
                if (panel != null) panel.SetActive(true);

                yield return FadeTo(1f);
                yield return new WaitForSeconds(displayDuration);
                yield return FadeTo(0f);
            }

            if (panel != null) panel.SetActive(false);
            isShowing = false;
        }

        private IEnumerator FadeTo(float target)
        {
            if (canvasGroup == null) yield break; // no fade configured — instant show/hide via panel active state alone

            float start = canvasGroup.alpha;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = target;
        }
    }
}
