using UnityEngine;

namespace TheyCallMeAlfredUnity
{
    [AddComponentMenu("TheyCallMeAlfred/Utils/Simple Rotation")]
    public class SimpleRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
}
