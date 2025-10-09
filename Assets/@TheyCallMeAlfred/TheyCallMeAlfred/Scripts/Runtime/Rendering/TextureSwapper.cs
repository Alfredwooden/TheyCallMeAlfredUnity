using System.Collections;
using UnityEngine;

namespace TheyCallMeAlfredUnity.Rendering {
    [AddComponentMenu("@TheyCallMeAlfred/Rendering/Texture Swapper")]
    public class TextureSwapper : MonoBehaviour {
        [Header("Face Settings")] public Renderer targetRenderer;

        public float textureDuration = 2f;
        public bool autoStart = true;

        [Header("Face Configuration")] public int faceCount = 3;

        public float faceScrollStep = -0.256f;

        [Header("Material Settings")] public int materialIndex;

        public string scrollUVPropertyName = "_ScrollUV";

        private int currentFaceIndex;
        private Material materialInstance;
        private Coroutine textureCoroutine;

        private void Start() {
            if (targetRenderer == null) {
                targetRenderer = GetComponent<Renderer>();
            }

            if (targetRenderer == null) {
                Debug.LogError("No Renderer component found on " + gameObject.name);
                return;
            }

            CreateMaterialInstance();

            if (autoStart && faceCount > 0) {
                StartTextureLoop();
            }
        }

        private void OnDestroy() {
            if (materialInstance != null) {
                DestroyImmediate(materialInstance);
            }
        }

        private void CreateMaterialInstance() {
            if (targetRenderer.materials.Length > materialIndex) {
                var materials = targetRenderer.materials;
                materialInstance = new Material(materials[materialIndex]);
                materials[materialIndex] = materialInstance;
                targetRenderer.materials = materials;

                ApplyCurrentFace();
            }
            else {
                Debug.LogError(
                    $"Material index {materialIndex} is out of range. Renderer has {targetRenderer.materials.Length} materials.");
            }
        }

        public void StartTextureLoop() {
            if (textureCoroutine != null) {
                StopCoroutine(textureCoroutine);
            }

            textureCoroutine = StartCoroutine(TextureLoop());
        }

        public void StopTextureLoop() {
            if (textureCoroutine != null) {
                StopCoroutine(textureCoroutine);
                textureCoroutine = null;
            }
        }

        private IEnumerator TextureLoop() {
            while (true) {
                if (faceCount == 0) {
                    yield break;
                }

                ApplyCurrentFace();

                yield return new WaitForSeconds(textureDuration);

                currentFaceIndex = (currentFaceIndex + 1) % faceCount;
            }
        }

        public void SwitchToNextFace() {
            if (faceCount == 0) {
                return;
            }

            currentFaceIndex = (currentFaceIndex + 1) % faceCount;
            ApplyCurrentFace();
        }

        public void SwitchToSpecificFace(int index) {
            if (index >= 0 && index < faceCount) {
                currentFaceIndex = index;
                ApplyCurrentFace();
            }
        }

        private void ApplyCurrentFace() {
            if (materialInstance == null) {
                return;
            }

            Vector2 scrollUV = new(0f, currentFaceIndex * faceScrollStep);
            materialInstance.SetVector(scrollUVPropertyName, scrollUV);
        }
    }
}
