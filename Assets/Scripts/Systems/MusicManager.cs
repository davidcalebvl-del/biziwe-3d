using UnityEngine;
using System.Collections;

namespace Biziwe.Systems
{
    public enum MusicState { Explore, Chase, Mission, Menu }

    /// <summary>
    /// Background music that reacts to what's happening — calm while
    /// exploring, tense during a police chase, distinct mission music —
    /// crossfading smoothly between states rather than hard-cutting. Hooks
    /// into WantedSystem automatically so chase music kicks in without any
    /// manual triggering.
    ///
    /// Honest note: this is the music PLAYBACK system — it doesn't include
    /// actual music tracks. See Docs/AUDIO.md for where to source real,
    /// usable music/sound (free options included).
    ///
    /// SETUP:
    /// 1. Add to the GameManager object (or its own "MusicManager" object).
    /// 2. Add two AudioSource components to the same object (needed for
    ///    crossfading between tracks — one fades out while the other fades in).
    /// 3. Assign exploreTrack, chaseTrack, missionTrack clips.
    /// 4. Assign the WantedSystem reference so chase music is automatic.
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        public AudioSource sourceA;
        public AudioSource sourceB;
        private AudioSource activeSource;
        private AudioSource inactiveSource;

        public AudioClip exploreTrack;
        public AudioClip chaseTrack;
        public AudioClip missionTrack;
        public AudioClip menuTrack;

        public WantedSystem wantedSystem;

        [Header("Crossfade")]
        public float fadeDuration = 1.5f;
        [Range(0f, 1f)] public float musicVolume = 0.6f;

        private MusicState currentState = MusicState.Explore;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            activeSource = sourceA;
            inactiveSource = sourceB;
        }

        private void OnEnable()
        {
            if (wantedSystem != null)
                wantedSystem.OnWantedLevelChanged += HandleWantedLevelChanged;
        }

        private void OnDisable()
        {
            if (wantedSystem != null)
                wantedSystem.OnWantedLevelChanged -= HandleWantedLevelChanged;
        }

        private void HandleWantedLevelChanged(int level)
        {
            // Mission music (if actively playing) takes priority over chase
            // music — being chased during a scripted mission shouldn't cut
            // the mission's own track.
            if (currentState == MusicState.Mission) return;

            SetState(level > 0 ? MusicState.Chase : MusicState.Explore);
        }

        public void SetState(MusicState newState)
        {
            if (newState == currentState) return;
            currentState = newState;

            AudioClip clip = newState switch
            {
                MusicState.Explore => exploreTrack,
                MusicState.Chase => chaseTrack,
                MusicState.Mission => missionTrack,
                MusicState.Menu => menuTrack,
                _ => exploreTrack,
            };

            if (clip == null) return;

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(CrossfadeTo(clip));
        }

        private IEnumerator CrossfadeTo(AudioClip newClip)
        {
            inactiveSource.clip = newClip;
            inactiveSource.volume = 0f;
            inactiveSource.loop = true;
            inactiveSource.Play();

            float t = 0f;
            float startVolume = activeSource.volume;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float progress = t / fadeDuration;
                activeSource.volume = Mathf.Lerp(startVolume, 0f, progress);
                inactiveSource.volume = Mathf.Lerp(0f, musicVolume, progress);
                yield return null;
            }

            activeSource.Stop();
            activeSource.volume = 0f;

            // Swap roles — what was fading in is now the active source.
            (activeSource, inactiveSource) = (inactiveSource, activeSource);
        }
    }
}
