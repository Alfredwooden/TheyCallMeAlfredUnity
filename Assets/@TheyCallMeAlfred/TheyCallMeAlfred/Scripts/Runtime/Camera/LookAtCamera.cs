using UnityEngine;

namespace TheyCallMeAlfredUnity.Camera {
    [AddComponentMenu("@TheyCallMeAlfred/Camera/Look At Camera")]
    public class LookAtCamera : MonoBehaviour {
        private UnityEngine.Camera targetCamera;

        private void Start() {
            FindCamera();
        }

        private void FindCamera() {
            targetCamera = UnityEngine.Camera.main;

            if (targetCamera == null) {
                targetCamera = GameObject.FindWithTag("MainCamera")?.GetComponent<UnityEngine.Camera>();
            }

            if (targetCamera == null) {
                targetCamera = FindFirstObjectByType<UnityEngine.Camera>();
            }

            if (targetCamera == null) {
                Debug.LogWarning("LookAtCamera: No camera found!");
            }
        }

        private void Update() {
            if (targetCamera != null) {
                transform.rotation = Quaternion.LookRotation(targetCamera.transform.forward);
            }
        }
    }
}
