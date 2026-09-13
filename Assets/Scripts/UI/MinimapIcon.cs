using UnityEngine;

namespace Biziwe.UI
{
    public enum MinimapIconType { Player, Police, Civilian, MissionTarget, Ally, Enemy }

    /// <summary>
    /// Attach to anything that should show as a dot on the minimap — police
    /// NPCs, civilians, mission markers, etc. Registers itself with
    /// MinimapController automatically, so adding a dot to the map is just
    /// adding this component, no manual list management.
    ///
    /// SETUP:
    /// 1. Add to any trackable object (police car, civilian, mission trigger).
    /// 2. Set the iconType — MinimapController maps each type to a color
    ///    (matching the reference: red = danger/police, green = civilians/
    ///    points of interest, blue = other markers).
    /// </summary>
    public class MinimapIcon : MonoBehaviour
    {
        public MinimapIconType iconType = MinimapIconType.Civilian;

        private void OnEnable()
        {
            MinimapController.Instance?.Register(this);
        }

        private void OnDisable()
        {
            MinimapController.Instance?.Unregister(this);
        }
    }
}
