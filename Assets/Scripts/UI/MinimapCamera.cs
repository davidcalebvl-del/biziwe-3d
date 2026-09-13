using UnityEngine;

namespace Biziwe.UI
{
    /// <summary>
    /// Top-down camera for the minimap — follows the player from directly
    /// above, renders to a RenderTexture that a UI RawImage displays. This is
    /// the actual rendered city view behind the minimap (not just abstract
    /// dots on nothing).
    ///
    /// SETUP:
    /// 1. Create a new Camera, set Projection to Orthographic.
    /// 2. Create a RenderTexture asset (e.g. 512x512), assign it to this
    ///    camera's Target Texture.
    /// 3. On your minimap UI, add a RawImage and assign the same
    ///    RenderTexture as its texture — that's your live minimap background.
    /// 4. Add this script to the camera, assign the player transform.
    /// 5. Set the camera's Culling Mask to exclude UI and anything you don't
    ///    want visible from above (e.g. a "Minimap Hidden" layer for interiors).
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class MinimapCamera : MonoBehaviour
    {
        public Transform player;
        public float heightAbovePlayer = 60f;
        public bool rotateWithPlayer = false; // false = north-up (like the reference), true = player-forward-up

        private void LateUpdate()
        {
            if (player == null) return;

            transform.position = new Vector3(player.position.x, player.position.y + heightAbovePlayer, player.position.z);
            transform.rotation = rotateWithPlayer
                ? Quaternion.Euler(90f, player.eulerAngles.y, 0f)
                : Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
