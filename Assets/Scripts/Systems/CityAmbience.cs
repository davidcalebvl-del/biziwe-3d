using UnityEngine;
using System.Collections;

namespace Biziwe.Systems
{
    /// <summary>
    /// Looping background city sound — traffic hum, distant crowd noise —
    /// that shifts between a daytime and nighttime ambience automatically.
    /// Separate from MusicManager (this is atmosphere, not the score) so
    /// they layer together rather than competing.
    ///
    /// Honest note: needs real ambient loop audio files — see Docs/AUDIO.md
    /// for free sources. This is the playback/crossfade system.
    ///
    /// SETUP:
    /// 1. Add to the GameManager object.
    /// 2. Add two AudioSource components (for crossfading day/night).
    /// 3. Assign dayAmbience and nightAmbience loop clips.
    /// 4. Assign the DayNightCycle reference.
    /// </summary>
    public class CityAmbience : MonoBehaviour
    {
        public AudioSource sourceA;
        public AudioSource sourceB;
        private AudioSource activeSource;
        private AudioSource inactiveSource;

        public AudioClip dayAmbience;
        public AudioClip nightAmbience;
        public DayNightCycle dayNightCycle;

        [Range(0f, 1f)] public float ambienceVolume = 0.35f; // base level before the player's Settings slider is applied
        public float fadeDuration = 4f; // slower fade than music — ambience shifts should feel gradual

        private float EffectiveVolume => ambienceVolume * AudioPreferences.AmbienceVolume;

        private bool isNight;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            activeSource = sourceA;
            inactiveSource = sourceB;
        }

        private void OnEnable()
        {
            AudioPreferences.OnChanged += ApplyVolumeImmediate;
        }

        private void OnDisable()
        {
            AudioPreferences.OnChanged -= ApplyVolumeImmediate;
        }

        private void ApplyVolumeImmediate()
        {
            if (activeSource != null && activeSource.isPlaying)
                activeSource.volume = EffectiveVolume;
        }

        private void Start()
        {
            if (dayNightCycle == null || dayAmbience == null) return;

            isNight = dayNightCycle.IsNight;
            activeSource.clip = isNight ? nightAmbience : dayAmbience;
            activeSource.loop = true;
            activeSource.volume = EffectiveVolume;
            activeSource.Play();
        }

        private void Update()
        {
            if (dayNightCycle == null) return;

            bool nowNight = dayNightCycle.IsNight;
            if (nowNight != isNight)
            {
                isNight = nowNight;
                AudioClip target = isNight ? nightAmbience : dayAmbience;
                if (target == null) return;

                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(CrossfadeTo(target));
            }
        }

        private IEnumerator CrossfadeTo(AudioClip newClip)
        {
            inactiveSource.clip = newClip;
            inactiveSource.volume = 0f;
            inactiveSource.loop = true;
            inactiveSource.Play();

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float progress = t / fadeDuration;
                activeSource.volume = Mathf.Lerp(EffectiveVolume, 0f, progress);
                inactiveSource.volume = Mathf.Lerp(0f, EffectiveVolume, progress);
                yield return null;
            }

            activeSource.Stop();
            (activeSource, inactiveSource) = (inactiveSource, activeSource);
        }
    }
}
