using UnityEngine;

namespace Biziwe.World
{
    /// <summary>
    /// A bank of short ambient street lines — generic, natural chatter civilians
    /// say to each other as the player passes by. Data-only, same pattern as
    /// MissionData: writing new lines means adding to an asset in the Editor,
    /// not touching code. Create multiple AmbientLines assets per
    /// neighborhood/district later (market chatter vs. dockside chatter vs.
    /// upscale-district chatter) for variety — the system just picks whichever
    /// bank is assigned to a given area's civilians.
    ///
    /// SETUP:
    /// 1. Right-click in Project window > Create > Biziwe > Ambient Lines.
    /// 2. Fill in a handful of short, natural lines.
    /// 3. Assign this asset to AmbientChatter's lineBank field.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAmbientLines", menuName = "Biziwe/Ambient Lines")]
    public class AmbientLines : ScriptableObject
    {
        [TextArea(1, 2)]
        public string[] lines = new string[]
        {
            "How market be today?",
            "Traffic don hold again o.",
            "You see wetin happen for that junction?",
            "This heat no dey do well at all.",
            "I dey come, make I just finish this thing.",
            "Abeg carry me reach the junction.",
        };
    }
}
