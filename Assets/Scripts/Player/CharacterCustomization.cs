using UnityEngine;
using System;

namespace Biziwe.Player
{
    [Serializable]
    public class AppearanceOption
    {
        public string label;         // e.g. "Style A", shown in the customization UI
        public GameObject visualPrefab; // the character model/outfit variant this represents
    }

    /// <summary>
    /// Character creation — free-text name entry plus a choice of appearance
    /// options. Not tied to any nationality or default identity: the player
    /// builds who they are. Swap in real character models/outfits as
    /// appearanceOptions once art assets exist — the system itself doesn't
    /// change.
    ///
    /// SETUP:
    /// 1. Add to a "CharacterCreation" UI screen shown before the game starts.
    /// 2. Assign an InputField for the name, and populate appearanceOptions
    ///    with each visual variant (even simple placeholder capsules/colors
    ///    work fine until real art exists).
    /// 3. Call ApplyToPlayer() once the player confirms their choices, passing
    ///    the player GameObject that should receive the chosen appearance.
    /// </summary>
    public class CharacterCustomization : MonoBehaviour
    {
        public AppearanceOption[] appearanceOptions;

        public string ChosenName { get; private set; } = "Player";
        public int ChosenAppearanceIndex { get; private set; } = 0;

        public void SetName(string playerName)
        {
            ChosenName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName.Trim();
        }

        public void SetAppearance(int index)
        {
            if (appearanceOptions == null || appearanceOptions.Length == 0) return;
            ChosenAppearanceIndex = Mathf.Clamp(index, 0, appearanceOptions.Length - 1);
        }

        public void CycleAppearanceNext()
        {
            if (appearanceOptions == null || appearanceOptions.Length == 0) return;
            SetAppearance((ChosenAppearanceIndex + 1) % appearanceOptions.Length);
        }

        /// <summary>
        /// Spawns/attaches the chosen visual onto the player and applies the name
        /// (e.g. to a name tag or save profile). Call once at game start, after
        /// the player confirms their choices on the creation screen.
        /// </summary>
        public GameObject ApplyToPlayer(Transform playerRoot)
        {
            if (appearanceOptions == null || appearanceOptions.Length == 0 || playerRoot == null)
                return null;

            var chosen = appearanceOptions[ChosenAppearanceIndex];
            if (chosen.visualPrefab == null) return null;

            GameObject visual = Instantiate(chosen.visualPrefab, playerRoot);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            return visual;
        }
    }
}
