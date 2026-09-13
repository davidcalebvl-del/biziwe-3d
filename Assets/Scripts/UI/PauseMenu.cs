using UnityEngine;
using UnityEngine.SceneManagement;

namespace Biziwe.UI
{
    /// <summary>
    /// Pause menu — Resume, Options, Help, Quit. Freezes gameplay via
    /// Time.timeScale (so physics/AI/animations all pause together, not just
    /// input) while keeping the UI itself responsive.
    ///
    /// SETUP:
    /// 1. Create a Canvas with a pause panel (Resume/Options/Help/Quit buttons,
    ///    plus your money/time-elapsed display if you want it visible while paused).
    /// 2. Start the panel inactive.
    /// 3. Add this script to the Canvas, assign the panel and buttons' OnClick
    ///    to the matching methods below.
    /// 4. Wire a pause button (visible during gameplay) to TogglePause().
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pausePanel;
        public bool IsPaused { get; private set; }

        private float sessionStartRealtime;

        private void Start()
        {
            sessionStartRealtime = Time.realtimeSinceStartup;
        }

        public void TogglePause()
        {
            if (IsPaused) Resume();
            else Pause();
        }

        public void Pause()
        {
            IsPaused = true;
            Time.timeScale = 0f;
            if (pausePanel != null) pausePanel.SetActive(true);
        }

        public void Resume()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        /// <summary>Call from the pause menu's "Help" button.</summary>
        public void ShowHelp(GameObject helpPanel)
        {
            if (helpPanel != null) helpPanel.SetActive(true);
        }

        /// <summary>Call from the pause menu's "End"/Quit button.</summary>
        public void QuitToMenu(string mainMenuSceneName)
        {
            Time.timeScale = 1f; // always restore before leaving the scene
            SceneManager.LoadScene(mainMenuSceneName);
        }

        /// <summary>Elapsed real play time this session, for the "Time elapsed" display.</summary>
        public string GetElapsedTimeText()
        {
            float elapsed = Time.realtimeSinceStartup - sessionStartRealtime;
            int minutes = Mathf.FloorToInt(elapsed / 60f);
            int seconds = Mathf.FloorToInt(elapsed % 60f);
            return $"{minutes}:{seconds:00}";
        }
    }
}
