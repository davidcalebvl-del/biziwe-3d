using UnityEngine;

namespace Biziwe.Systems
{
    /// <summary>
    /// Shared, persisted volume levels — Music, Ambience, SFX — that any
    /// audio-playing script reads from so one Settings screen actually
    /// controls the whole game's sound. Not a MonoBehaviour — no GameObject
    /// needed, just call these statics directly.
    ///
    /// SETUP: nothing to place in the scene. SettingsMenu.cs writes to this;
    /// MusicManager, CityAmbience, PoliceSirenAudio, Weapon, Explosive and
    /// VehicleController all read from it.
    /// </summary>
    public static class AudioPreferences
    {
        private const string MusicKey = "audio_music_volume";
        private const string AmbienceKey = "audio_ambience_volume";
        private const string SfxKey = "audio_sfx_volume";

        private static float? music;
        private static float? ambience;
        private static float? sfx;

        public static event System.Action OnChanged;

        public static float MusicVolume
        {
            get { music ??= PlayerPrefs.GetFloat(MusicKey, 1f); return music.Value; }
        }

        public static float AmbienceVolume
        {
            get { ambience ??= PlayerPrefs.GetFloat(AmbienceKey, 1f); return ambience.Value; }
        }

        public static float SfxVolume
        {
            get { sfx ??= PlayerPrefs.GetFloat(SfxKey, 1f); return sfx.Value; }
        }

        public static void SetMusicVolume(float value)
        {
            music = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MusicKey, music.Value);
            OnChanged?.Invoke();
        }

        public static void SetAmbienceVolume(float value)
        {
            ambience = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(AmbienceKey, ambience.Value);
            OnChanged?.Invoke();
        }

        public static void SetSfxVolume(float value)
        {
            sfx = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(SfxKey, sfx.Value);
            OnChanged?.Invoke();
        }
    }
}
