using UnityEngine;
using UnityEngine.UI;

namespace Biziwe.UI
{
    /// <summary>
    /// Character creation screen — free-text name entry and an appearance
    /// carousel (Previous/Next through appearanceOptions, showing a live 3D
    /// preview if you set one up). Confirms into MainMenu.StartGame().
    ///
    /// SETUP:
    /// 1. Build the panel: an InputField for the name, Previous/Next buttons,
    ///    a text label showing the current appearance option's name, and a
    ///    Confirm button.
    /// 2. Optionally add a small preview camera + turntable showing the
    ///    selected appearanceOptions[].visualPrefab — nice touch, not
    ///    required for this to function.
    /// 3. Add this script to the panel, assign references including the
    ///    same appearanceOptions list you'll use in-game (or a matching one).
    /// </summary>
    public class CharacterCreationScreen : MonoBehaviour
    {
        public InputField nameInputField;
        public Text appearanceLabel;
        public Player.AppearanceOption[] appearanceOptions;
        public MainMenu mainMenu;

        private int selectedAppearanceIndex = 0;

        private void OnEnable()
        {
            UpdateAppearanceLabel();
        }

        /// <summary>Wire to a "Next" arrow button.</summary>
        public void NextAppearance()
        {
            if (appearanceOptions == null || appearanceOptions.Length == 0) return;
            selectedAppearanceIndex = (selectedAppearanceIndex + 1) % appearanceOptions.Length;
            UpdateAppearanceLabel();
        }

        /// <summary>Wire to a "Previous" arrow button.</summary>
        public void PreviousAppearance()
        {
            if (appearanceOptions == null || appearanceOptions.Length == 0) return;
            selectedAppearanceIndex = (selectedAppearanceIndex - 1 + appearanceOptions.Length) % appearanceOptions.Length;
            UpdateAppearanceLabel();
        }

        private void UpdateAppearanceLabel()
        {
            if (appearanceLabel == null || appearanceOptions == null || appearanceOptions.Length == 0) return;
            appearanceLabel.text = appearanceOptions[selectedAppearanceIndex].label;
            // Hook: swap the preview model here if you've set up a preview turntable.
        }

        /// <summary>Wire to the "Confirm" / "Start" button.</summary>
        public void ConfirmAndStart()
        {
            string chosenName = nameInputField != null ? nameInputField.text : "";
            if (string.IsNullOrWhiteSpace(chosenName)) chosenName = "Player";

            Systems.SaveSystem.SaveInitialCharacter(chosenName.Trim(), selectedAppearanceIndex);

            if (mainMenu != null) mainMenu.StartGame();
        }
    }
}
