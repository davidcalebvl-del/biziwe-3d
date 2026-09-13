using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Biziwe.UI
{
    /// <summary>
    /// Loading screen — title, a rotating tagline, and a real progress bar
    /// tied to actual scene-load progress (not a fake timer).
    ///
    /// SETUP:
    /// 1. Create a separate small "LoadingScreen" scene with just this UI.
    /// 2. Assign titleText ("BIZIWE"), taglineText, and the progress bar Image
    ///    (set to Filled type, Horizontal).
    /// 3. Call LoadSceneWithProgress("YourGameSceneName") to begin — typically
    ///    triggered from a main menu's "Play" button.
    /// </summary>
    public class LoadingScreen : MonoBehaviour
    {
        public Text titleText;
        public Text taglineText;
        public Image progressBarFill;

        public string[] taglines = new string[]
        {
            "Obodo Bay never sleeps.",
            "Every hustle has a price.",
            "The city remembers everything.",
            "Trust is the rarest currency here.",
        };

        public void BeginLoad(string sceneName)
        {
            if (titleText != null) titleText.text = "BIZIWE";
            if (taglineText != null && taglines.Length > 0)
                taglineText.text = taglines[Random.Range(0, taglines.Length)];

            StartCoroutine(LoadSceneWithProgress(sceneName));
        }

        private IEnumerator LoadSceneWithProgress(string sceneName)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            while (!op.isDone)
            {
                // Unity reports progress 0-0.9 during load, then jumps to 1 on
                // activation — remap to a clean 0-1 range for the bar.
                float displayProgress = Mathf.Clamp01(op.progress / 0.9f);
                if (progressBarFill != null) progressBarFill.fillAmount = displayProgress;

                if (op.progress >= 0.9f)
                {
                    if (progressBarFill != null) progressBarFill.fillAmount = 1f;
                    yield return new WaitForSeconds(0.3f); // brief pause at 100% feels more intentional than an instant cut
                    op.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}
