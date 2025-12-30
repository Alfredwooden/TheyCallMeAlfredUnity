using UnityEngine;

public static class MeshInfoCalculator
{
    public struct MeshInfo
    {
        public int totalVertices;
        public int totalTriangles;
        public int totalSubmeshes;
        public int totalMaterials;
    }

    public static MeshInfo GetMeshInfo(Transform transform)
    {
        MeshInfo info = new MeshInfo();

        MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            Mesh mesh = meshFilter.sharedMesh;
            info.totalVertices += mesh.vertexCount;
            info.totalTriangles += mesh.triangles.Length / 3;
            info.totalSubmeshes += mesh.subMeshCount;

            Renderer renderer = transform.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterials != null)
            {
                info.totalMaterials += renderer.sharedMaterials.Length;
            }
        }

        SkinnedMeshRenderer skinnedMesh = transform.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMesh != null && skinnedMesh.sharedMesh != null)
        {
            Mesh mesh = skinnedMesh.sharedMesh;
            info.totalVertices += mesh.vertexCount;
            info.totalTriangles += mesh.triangles.Length / 3;
            info.totalSubmeshes += mesh.subMeshCount;

            if (skinnedMesh.sharedMaterials != null)
            {
                info.totalMaterials += skinnedMesh.sharedMaterials.Length;
            }
        }

        foreach (Transform child in transform)
        {
            MeshInfo childInfo = GetMeshInfo(child);
            info.totalVertices += childInfo.totalVertices;
            info.totalTriangles += childInfo.totalTriangles;
            info.totalSubmeshes += childInfo.totalSubmeshes;
            info.totalMaterials += childInfo.totalMaterials;
        }

        return info;
    }
}
