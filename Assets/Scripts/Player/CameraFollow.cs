using UnityEngine;

namespace Biziwe.Player
{
    /// <summary>
    /// Simple third-person orbit camera — Phase 1.
    /// Follows a target (the player) at a fixed distance, with touch-drag look support.
    ///
    /// SETUP:
    /// 1. Attach to your Main Camera.
    /// 2. Drag the player's "CameraTarget" child object into the Target field.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float distance = 5f;
        public float height = 2f;
        public float rotationSpeed = 3f;
        public float smoothTime = 0.12f;

        private float yaw;
        private float pitch = 10f;
        private Vector3 velocity;

        private void LateUpdate()
        {
            if (target == null) return;

            // Touch drag to look around — one finger, right half of screen recommended.
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Moved)
                {
                    yaw += t.deltaPosition.x * rotationSpeed * Time.deltaTime;
                    pitch -= t.deltaPosition.y * rotationSpeed * Time.deltaTime;
                    pitch = Mathf.Clamp(pitch, -20f, 60f);
                }
            }
#if UNITY_EDITOR
            // Mouse look for testing in the Editor before touch input is wired up.
            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * rotationSpeed * 40f * Time.deltaTime;
                pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * 40f * Time.deltaTime;
                pitch = Mathf.Clamp(pitch, -20f, 60f);
            }
#endif

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPosition = target.position + Vector3.up * height - rotation * Vector3.forward * distance;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
            transform.LookAt(target.position + Vector3.up * height * 0.5f);
        }
    }
}
