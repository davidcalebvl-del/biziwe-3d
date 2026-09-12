using UnityEngine;

namespace Biziwe.Systems
{
    public enum MissionType { Driving, Delivery, Escape, Chase, Rescue, Investigation, Combat, Stealth, Exploration, Racing }

    /// <summary>
    /// Data-only definition of a mission — a ScriptableObject asset, not code,
    /// so writing a new mission means creating a new asset in the Unity Editor
    /// rather than writing a new script. This is what lets the game scale to
    /// "hundreds of missions later" (per the vision doc) without the codebase
    /// growing linearly with mission count.
    ///
    /// SETUP:
    /// 1. In Unity: right-click in the Project window > Create > Biziwe > Mission.
    /// 2. Fill in the fields for each mission you design.
    /// 3. Reference the mission's `id` field from MissionTrigger components in
    ///    the scene — MissionManager (the runtime state tracker) stays exactly
    ///    as-is; this just gives each mission a proper data home instead of a
    ///    bare string.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMission", menuName = "Biziwe/Mission")]
    public class MissionData : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea(2, 5)] public string description;
        public MissionType type;
        public int rewardMoney = 100;
        public int wantedStarsOnFail = 0; // some missions (Combat/Stealth) can raise wanted level on failure

        [Header("Unlock")]
        [Tooltip("Mission id that must be completed before this one becomes available. Leave blank if always available.")]
        public string requiresMissionId;
    }
}
