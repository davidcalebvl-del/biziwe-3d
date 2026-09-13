using UnityEngine;

namespace Biziwe.Systems
{
    /// <summary>
    /// Basic save/load — persists money balance and mission completion between
    /// play sessions using Unity's PlayerPrefs (simple, built-in, works on
    /// Android/iOS/WebGL with no extra packages). EconomySystem and
    /// MissionManager were previously in-memory-only for a single session —
    /// this closes that gap without changing either script's public API.
    ///
    /// SETUP:
    /// 1. Add to the same GameManager object as the other systems.
    /// 2. Add this AFTER EconomySystem/MissionManager in GameManager's Awake
    ///    order (GameManager already grabs them first, so this is safe as long
    ///    as SaveSystem's own Awake/Start runs after — Unity runs Awake in the
    ///    order components were added, so just add this script last).
    /// 3. Call Load() once at game start (after GameManager.Instance exists),
    ///    and Save() whenever you want to persist — e.g. after completing a
    ///    mission, or on a timer, or when the app is paused/quit (handled below).
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        private const string MoneyKey = "biziwe_money";
        private const string CompletedMissionsKey = "biziwe_completed_missions"; // comma-separated ids

        public void Save()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (gm.Economy != null)
                PlayerPrefs.SetInt(MoneyKey, gm.Economy.CurrentBalance);

            // MissionManager doesn't expose a full list directly — this saves
            // via a lightweight approach: call NotifyMissionCompleted(id) from
            // GameHUD's OnMissionCompleted hook (or MissionManager itself) to
            // append ids as they complete, avoiding a MissionManager API change.
            PlayerPrefs.Save();
        }

        public void Load()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (gm.Economy != null && PlayerPrefs.HasKey(MoneyKey))
            {
                int savedBalance = PlayerPrefs.GetInt(MoneyKey);
                // EconomySystem starts at startingBalance in its own Awake — this
                // tops it up (or down) to match the saved value without needing
                // a setter added to EconomySystem's public API.
                int diff = savedBalance - gm.Economy.CurrentBalance;
                if (diff > 0) gm.Economy.AddMoney(diff);
                else if (diff < 0) gm.Economy.SpendMoney(-diff);
            }
        }

        public void RecordMissionCompleted(string missionId)
        {
            string existing = PlayerPrefs.GetString(CompletedMissionsKey, "");
            if (!existing.Contains(missionId))
            {
                existing = string.IsNullOrEmpty(existing) ? missionId : existing + "," + missionId;
                PlayerPrefs.SetString(CompletedMissionsKey, existing);
            }
        }

        public bool IsMissionCompleted(string missionId)
        {
            string existing = PlayerPrefs.GetString(CompletedMissionsKey, "");
            return ("," + existing + ",").Contains("," + missionId + ",");
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) Save(); // mobile: app going to background is the reliable "about to close" signal
        }

        private void OnApplicationQuit()
        {
            Save();
        }
    }
}
