using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Biziwe.UI
{
    /// <summary>
    /// Positions colored dots over the minimap for every registered
    /// MinimapIcon, based on their real-time world position relative to the
    /// player — matching the red/green/blue dots in the Payback²-style
    /// reference. Works alongside MinimapCamera (which renders the actual
    /// top-down city view these dots sit on top of).
    ///
    /// SETUP:
    /// 1. Add to the minimap's Canvas/panel GameObject.
    /// 2. Assign the player transform, the minimap's RectTransform (the
    ///    square/circle panel showing the camera's RenderTexture), and a
    ///    dotPrefab (a small colored UI Image, e.g. a circle sprite).
    /// 3. Set worldRadius to match your MinimapCamera's orthographic size —
    ///    this is "how far in world units the edge of the minimap represents."
    /// </summary>
    public class MinimapController : MonoBehaviour
    {
        public static MinimapController Instance { get; private set; }

        public Transform player;
        public RectTransform minimapPanel;
        public Image dotPrefab;
        public float worldRadius = 60f; // should roughly match MinimapCamera's orthographic size

        [Header("Icon Colors")]
        public Color playerColor = Color.white;
        public Color policeColor = Color.red;
        public Color civilianColor = Color.green;
        public Color missionTargetColor = new Color(1f, 0.71f, 0.28f); // matches the danfo-yellow accent
        public Color allyColor = Color.cyan;
        public Color enemyColor = Color.red;

        private List<MinimapIcon> registered = new List<MinimapIcon>();
        private Dictionary<MinimapIcon, Image> activeDots = new Dictionary<MinimapIcon, Image>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple MinimapController instances found — destroying the extra one.");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Register(MinimapIcon icon)
        {
            if (!registered.Contains(icon)) registered.Add(icon);
        }

        public void Unregister(MinimapIcon icon)
        {
            registered.Remove(icon);
            if (activeDots.TryGetValue(icon, out var dot))
            {
                if (dot != null) Destroy(dot.gameObject);
                activeDots.Remove(icon);
            }
        }

        private void LateUpdate()
        {
            if (player == null || minimapPanel == null || dotPrefab == null) return;

            float panelHalfSize = minimapPanel.rect.width / 2f;

            // Clean up any icons that were destroyed without disabling first.
            registered.RemoveAll(i => i == null);

            foreach (var icon in registered)
            {
                Vector3 offset = icon.transform.position - player.position;
                Vector2 flatOffset = new Vector2(offset.x, offset.z);

                if (flatOffset.magnitude > worldRadius)
                {
                    // Out of range — hide (don't destroy, might come back in range).
                    if (activeDots.TryGetValue(icon, out var offscreenDot) && offscreenDot != null)
                        offscreenDot.gameObject.SetActive(false);
                    continue;
                }

                if (!activeDots.TryGetValue(icon, out var dot) || dot == null)
                {
                    dot = Instantiate(dotPrefab, minimapPanel);
                    dot.color = GetColor(icon.iconType);
                    activeDots[icon] = dot;
                }

                dot.gameObject.SetActive(true);
                Vector2 normalized = flatOffset / worldRadius; // -1 to 1
                dot.rectTransform.anchoredPosition = normalized * panelHalfSize;
            }
        }

        private Color GetColor(MinimapIconType type)
        {
            switch (type)
            {
                case MinimapIconType.Player: return playerColor;
                case MinimapIconType.Police: return policeColor;
                case MinimapIconType.Civilian: return civilianColor;
                case MinimapIconType.MissionTarget: return missionTargetColor;
                case MinimapIconType.Ally: return allyColor;
                case MinimapIconType.Enemy: return enemyColor;
                default: return Color.white;
            }
        }
    }
}
