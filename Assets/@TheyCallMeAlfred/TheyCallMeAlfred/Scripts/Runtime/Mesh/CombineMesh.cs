using UnityEngine;

namespace TheyCallMeAlfredUnity.Meshes {
    [AddComponentMenu("@TheyCallMeAlfred/Mesh/Combine Mesh")]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class CombineMesh : MonoBehaviour {
        private void Start() {
            CombineInstance[] combine = new CombineInstance[transform.childCount];
            int index = 0;
            foreach (Transform child in gameObject.transform) {
                var filter = child.GetComponent<MeshFilter>();
                combine[index].mesh = filter.sharedMesh;
                combine[index].transform = child.localToWorldMatrix;
                child.gameObject.SetActive(false);
                index++;
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine);
            transform.GetComponent<MeshFilter>().sharedMesh = mesh;
            transform.gameObject.SetActive(true);
        }
    }
}
