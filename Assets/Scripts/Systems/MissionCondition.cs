using UnityEngine;

namespace Biziwe.Systems
{
    /// <summary>
    /// Reusable condition checks for missions — e.g. an Escape mission that
    /// only completes once your wanted level drops to 0, or a Stealth/Heist
    /// mission only available at night. Attach alongside MissionTrigger and
    /// it gates completesMission/startsMission on the condition being true,
    /// without changing MissionTrigger's own logic.
    ///
    /// SETUP:
    /// 1. Add to the same GameObject as a MissionTrigger.
    /// 2. Pick a condition type and fill in its threshold.
    /// 3. MissionTrigger checks this automatically if present (see the
    ///    integration note in MissionTrigger.cs).
    /// </summary>
    public class MissionCondition : MonoBehaviour
    {
        public enum ConditionType { WantedLevelAtOrBelow, TimeOfDayBetween, MinimumMoney }

        public ConditionType type = ConditionType.WantedLevelAtOrBelow;

        [Header("WantedLevelAtOrBelow")]
        public int maxWantedLevel = 0; // e.g. 0 = "must have fully lost the police"

        [Header("TimeOfDayBetween (24h)")]
        public float startHour = 20f;
        public float endHour = 5f; // wraps past midnight if start > end

        [Header("MinimumMoney")]
        public int requiredMoney = 0;

        public bool IsMet()
        {
            var gm = GameManager.Instance;
            if (gm == null) return true; // fail open — don't block missions if systems aren't wired yet

            switch (type)
            {
                case ConditionType.WantedLevelAtOrBelow:
                    return gm.Wanted == null || gm.Wanted.wantedLevel <= maxWantedLevel;

                case ConditionType.TimeOfDayBetween:
                    if (gm.DayNight == null) return true;
                    float hour = gm.DayNight.CurrentHour;
                    return startHour <= endHour
                        ? (hour >= startHour && hour <= endHour)
                        : (hour >= startHour || hour <= endHour); // wraps past midnight

                case ConditionType.MinimumMoney:
                    return gm.Economy == null || gm.Economy.CurrentBalance >= requiredMoney;
            }
            return true;
        }
    }
}
