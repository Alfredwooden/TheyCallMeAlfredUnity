using UnityEngine;

namespace TheyCallMeAlfredUnity.Animation {
    [AddComponentMenu("TheyCallMeAlfred/Animation/Simple Rotation")]
    public class SimpleRotation : MonoBehaviour {
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;

        private void Update() => transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
