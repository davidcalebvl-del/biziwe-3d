using UnityEngine;
using UnityEngine.UI;

namespace Biziwe.UI
{
    /// <summary>
    /// The title screen — New Game, Continue, Quit. Lives in its own small
    /// scene (separate from the main city scene), same pattern as
    /// LoadingScreen. Continue is only enabled if save data actually exists.
    ///
    /// SETUP:
    /// 1. Create a "MainMenu" scene (File > New Scene), add it to Build
    ///    Settings so it can be loaded/switched to.
    /// 2. Build the title UI: BIZIWE logo/title, New Game button, Continue
    ///    button, Quit button.
    /// 3. Add this script to the Canvas, assign the buttons and panel refs.
    /// 4. Also add this scene's LoadingScreen (used when transitioning out
    ///    to character creation or straight into the game).
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        public Button continueButton;
        public GameObject characterCreationPanel; // shown for New Game
        public LoadingScreen loadingScreen;

        public string gameSceneName = "MainCity"; // the actual playable city scene

        private void Start()
        {
            if (continueButton != null)
                continueButton.interactable = Systems.SaveSystem.HasSaveData();
        }

        /// <summary>Wire to the "New Game" button.</summary>
        public void OnNewGamePressed()
        {
            if (characterCreationPanel != null)
                characterCreationPanel.SetActive(true); // player builds their character first
            else
                StartGame(); // no character creation UI set up yet — just go straight in
        }

        /// <summary>Wire to the "Continue" button.</summary>
        public void OnContinuePressed()
        {
            if (!Systems.SaveSystem.HasSaveData()) return;
            StartGame(); // SaveSystem.Load() in the game scene restores everything automatically
        }

        /// <summary>Called by CharacterCreationScreen once the player confirms their character.</summary>
        public void StartGame()
        {
            if (loadingScreen != null)
                loadingScreen.BeginLoad(gameSceneName);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        }

        /// <summary>Wire to the "Quit" button.</summary>
        public void OnQuitPressed()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Quit doesn't work in the Editor otherwise
#endif
        }
    }
}
