using UnityEngine;

namespace Biziwe.Systems
{
    /// <summary>
    /// Day/night cycle — rotates the sun light over time and shifts ambient
    /// lighting/skybox tint accordingly. Phase 3.
    ///
    /// SETUP:
    /// 1. Assign your scene's main Directional Light (the "sun") to sunLight.
    /// 2. Set dayLengthMinutes to however long you want a full day/night cycle
    ///    to take in real minutes (e.g. 20 minutes = one full day).
    /// 3. Optionally assign day/night gradient colours for ambient light and fog,
    ///    referenced by NPC routines later (people out by day, fewer at night).
    /// </summary>
    public class DayNightCycle : MonoBehaviour
    {
        public Light sunLight;
        public float dayLengthMinutes = 20f;

        [Range(0f, 24f)]
        public float startHour = 8f; // start at 8 AM

        [Header("Lighting")]
        public Gradient ambientColorOverDay;
        public AnimationCurve sunIntensityOverDay = AnimationCurve.EaseInOut(0, 0.1f, 1, 1f);

        private float currentHour;

        public float CurrentHour => currentHour;
        public bool IsNight => currentHour < 5.5f || currentHour > 19f;

        private void Start()
        {
            currentHour = startHour;
        }

        private void Update()
        {
            float hoursPerSecond = 24f / (dayLengthMinutes * 60f);
            currentHour += hoursPerSecond * Time.deltaTime;
            if (currentHour >= 24f) currentHour -= 24f;

            UpdateSun();
        }

        private void UpdateSun()
        {
            if (sunLight == null) return;

            // 0h = midnight (sun straight down), 12h = noon (sun straight up)
            float sunAngle = (currentHour / 24f) * 360f - 90f;
            sunLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

            float dayProgress = currentHour / 24f;
            sunLight.intensity = sunIntensityOverDay.Evaluate(dayProgress);

            if (ambientColorOverDay != null)
                RenderSettings.ambientLight = ambientColorOverDay.Evaluate(dayProgress);
        }
    }
}
