using UnityEngine;
using UnityEngine.UI;
using Biziwe.Systems;
using Biziwe.Weapons;

namespace Biziwe.UI
{
    /// <summary>
    /// Ties WantedSystem, EconomySystem, MissionManager, and the equipped weapon
    /// together into one HUD — subscribes to each system's events rather than
    /// polling, so it stays in sync automatically as those systems change.
    ///
    /// SETUP:
    /// 1. Add to a HUD Canvas object.
    /// 2. Assign references to WantedSystem, EconomySystem, MissionManager,
    ///    WeaponHolder, and the UI Text/Image elements below.
    /// 3. For the star display, use 3 Image components (starIcons) — this script
    ///    toggles them filled/empty based on wanted level, matching the 3-star cap.
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("System References")]
        public WantedSystem wantedSystem;
        public EconomySystem economySystem;
        public MissionManager missionManager;
        public WeaponHolder weaponHolder;

        [Header("UI Elements")]
        public Image[] starIcons = new Image[3];
        public Color starFilledColor = Color.white;
        public Color starEmptyColor = new Color(1f, 1f, 1f, 0.25f);
        public Text moneyText;
        public Text missionText;
        public Text ammoText;

        private int previousWantedLevel;

        private void OnEnable()
        {
            if (wantedSystem != null) wantedSystem.OnWantedLevelChanged += UpdateStars;
            if (economySystem != null) economySystem.OnBalanceChanged += UpdateMoney;
            if (missionManager != null)
            {
                missionManager.OnMissionStarted += HandleMissionStarted;
                missionManager.OnMissionCompleted += HandleMissionCompleted;
                missionManager.OnMissionFailed += HandleMissionFailed;
            }
        }

        private void OnDisable()
        {
            if (wantedSystem != null) wantedSystem.OnWantedLevelChanged -= UpdateStars;
            if (economySystem != null) economySystem.OnBalanceChanged -= UpdateMoney;
            if (missionManager != null)
            {
                missionManager.OnMissionStarted -= HandleMissionStarted;
                missionManager.OnMissionCompleted -= HandleMissionCompleted;
                missionManager.OnMissionFailed -= HandleMissionFailed;
            }
        }

        private void Start()
        {
            // Initialize with current values rather than waiting for the first change event.
            if (wantedSystem != null)
            {
                previousWantedLevel = wantedSystem.wantedLevel; // set before UpdateStars so load doesn't fire a false "Wanted Level Up" toast
                UpdateStars(wantedSystem.wantedLevel);
            }
            if (economySystem != null) UpdateMoney(economySystem.CurrentBalance);
        }

        private void Update()
        {
            // Ammo updates every frame while a ranged weapon is equipped — simplest
            // reliable approach since Weapon doesn't fire an ammo-changed event yet.
            if (ammoText == null || weaponHolder == null) return;

            var equipped = weaponHolder.EquippedWeaponObject;
            var ranged = equipped != null ? equipped.GetComponent<Weapon>() : null;
            ammoText.text = ranged != null ? ranged.CurrentAmmo.ToString() : "";
        }

        private void UpdateStars(int level)
        {
            for (int i = 0; i < starIcons.Length; i++)
            {
                if (starIcons[i] == null) continue;
                starIcons[i].color = i < level ? starFilledColor : starEmptyColor;
            }

            if (level > previousWantedLevel)
                NotificationToast.Instance?.Show(level >= 3 ? "MAX WANTED LEVEL" : $"Wanted Level {level}");
            previousWantedLevel = level;
        }

        private void UpdateMoney(int balance)
        {
            if (moneyText != null) moneyText.text = $"${balance:N0}";
        }

        private void HandleMissionStarted(string id)
        {
            if (missionText != null) missionText.text = $"Objective: {id}";
            NotificationToast.Instance?.Show("Mission Started");
        }

        private void HandleMissionCompleted(string id)
        {
            if (missionText != null) missionText.text = $"{id} — Complete";
            NotificationToast.Instance?.Show("Mission Complete");
        }

        private void HandleMissionFailed(string id)
        {
            if (missionText != null) missionText.text = $"{id} — Failed";
            NotificationToast.Instance?.Show("Mission Failed");
        }
    }
}
