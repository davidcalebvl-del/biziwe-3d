using UnityEngine;

namespace Biziwe.Systems
{
    /// <summary>
    /// Central access point tying together the core systems (wanted level, money,
    /// missions, day/night, police dispatch) so any script can reach them without
    /// a maze of manually-dragged Inspector references. This is what keeps the
    /// project from accumulating broken/missing references as it grows.
    ///
    /// SETUP:
    /// 1. Create one empty GameObject in your scene called "GameManager".
    /// 2. Add WantedSystem, EconomySystem, MissionManager, MissionDatabase,
    ///    DayNightCycle, PoliceDispatcher, RoadblockSystem, and SaveSystem
    ///    components to it (all already written in this repo).
    /// 3. Add this script last — it grabs the sibling components automatically in
    ///    Awake(), so no manual wiring needed for these eight.
    /// 4. Other scripts can then reach everything via GameManager.Instance.*
    ///    instead of needing their own Inspector-assigned references.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public WantedSystem Wanted { get; private set; }
        public EconomySystem Economy { get; private set; }
        public MissionManager Missions { get; private set; }
        public MissionDatabase MissionDB { get; private set; }
        public DayNightCycle DayNight { get; private set; }
        public PoliceDispatcher PoliceDispatch { get; private set; }
        public RoadblockSystem Roadblocks { get; private set; }
        public SaveSystem Save { get; private set; }

        public Transform Player;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple GameManager instances found — destroying the extra one.");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Wanted = GetComponent<WantedSystem>();
            Economy = GetComponent<EconomySystem>();
            Missions = GetComponent<MissionManager>();
            MissionDB = GetComponent<MissionDatabase>();
            DayNight = GetComponent<DayNightCycle>();
            PoliceDispatch = GetComponent<PoliceDispatcher>();
            Roadblocks = GetComponent<RoadblockSystem>();
            Save = GetComponent<SaveSystem>();

            if (Wanted == null) Debug.LogWarning("GameManager: no WantedSystem found on this object.");
            if (Economy == null) Debug.LogWarning("GameManager: no EconomySystem found on this object.");
        }

        private void Start()
        {
            Save?.Load(); // apply any saved progress once every system above has initialized

            if (Missions != null && Save != null)
            {
                Missions.OnMissionCompleted += (id) => Save.RecordMissionCompleted(id);
            }
        }
    }
}
