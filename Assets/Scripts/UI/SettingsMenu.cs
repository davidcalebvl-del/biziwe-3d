using UnityEngine;
using UnityEngine.UI;
using Biziwe.Systems;

namespace Biziwe.UI
{
    /// <summary>
    /// Options screen — Music, Ambience, and SFX volume sliders. Reads/writes
    /// AudioPreferences, which every audio-playing script (MusicManager,
    /// CityAmbience, PoliceSirenAudio, Weapon, Explosive, VehicleController)
    /// already listens to, so dragging a slider here changes the sound
    /// everywhere immediately — no scene reload needed.
    ///
    /// SETUP:
    /// 1. Build a settings panel with 3 Sliders (0-1 range) — Music,
    ///    Ambience, SFX — labeled however you like.
    /// 2. Wire this to the panel, assign the 3 Slider references below.
    /// 3. Hook the pause menu's "Options" button to call
    ///    PauseMenu.ShowHelp(...)-style: SetActive(true) on this panel
    ///    (or call Open() from this script directly).
    /// 4. No OnClick wiring needed on the sliders themselves — this script
    ///    subscribes to their onValueChanged in OnEnable.
    /// </summary>
    public class SettingsMenu : MonoBehaviour
    {
        public GameObject panel;
        public Slider musicSlider;
        public Slider ambienceSlider;
        public Slider sfxSlider;

        private void OnEnable()
        {
            // Reflect the currently saved levels rather than resetting sliders to default each time the panel opens.
            if (musicSlider != null)
            {
                musicSlider.value = AudioPreferences.MusicVolume;
                musicSlider.onValueChanged.AddListener(AudioPreferences.SetMusicVolume);
            }
            if (ambienceSlider != null)
            {
                ambienceSlider.value = AudioPreferences.AmbienceVolume;
                ambienceSlider.onValueChanged.AddListener(AudioPreferences.SetAmbienceVolume);
            }
            if (sfxSlider != null)
            {
                sfxSlider.value = AudioPreferences.SfxVolume;
                sfxSlider.onValueChanged.AddListener(AudioPreferences.SetSfxVolume);
            }
        }

        private void OnDisable()
        {
            if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(AudioPreferences.SetMusicVolume);
            if (ambienceSlider != null) ambienceSlider.onValueChanged.RemoveListener(AudioPreferences.SetAmbienceVolume);
            if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(AudioPreferences.SetSfxVolume);
        }

        /// <summary>Call from the Pause Menu's "Options" button.</summary>
        public void Open()
        {
            if (panel != null) panel.SetActive(true);
        }

        /// <summary>Call from this panel's own "Back"/close button.</summary>
        public void Close()
        {
            if (panel != null) panel.SetActive(false);
        }
    }
}
