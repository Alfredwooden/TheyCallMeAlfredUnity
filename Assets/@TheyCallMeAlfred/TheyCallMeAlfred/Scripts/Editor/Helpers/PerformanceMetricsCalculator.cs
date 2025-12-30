using UnityEngine;

public static class PerformanceMetricsCalculator
{
    public static int EstimateDrawCalls(Transform transform)
    {
        int drawCalls = 0;

        Renderer renderer = transform.GetComponent<Renderer>();
        if (renderer != null && renderer.enabled)
        {
            MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
            SkinnedMeshRenderer skinnedMesh = transform.GetComponent<SkinnedMeshRenderer>();

            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                drawCalls += meshFilter.sharedMesh.subMeshCount;
            }
            else if (skinnedMesh != null && skinnedMesh.sharedMesh != null)
            {
                drawCalls += skinnedMesh.sharedMesh.subMeshCount;
            }
        }

        foreach (Transform child in transform)
        {
            drawCalls += EstimateDrawCalls(child);
        }

        return drawCalls;
    }

    public static long EstimateMemoryUsage(Transform transform, MeshInfoCalculator.MeshInfo meshInfo)
    {
        long memory = 0;

        MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            Mesh mesh = meshFilter.sharedMesh;
            memory += mesh.vertexCount * 12;
            memory += mesh.triangles.Length * 4;
        }

        SkinnedMeshRenderer skinnedMesh = transform.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMesh != null && skinnedMesh.sharedMesh != null)
        {
            Mesh mesh = skinnedMesh.sharedMesh;
            memory += mesh.vertexCount * 12;
            memory += mesh.triangles.Length * 4;
        }

        Renderer renderer = transform.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterials != null)
        {
            foreach (Material mat in renderer.sharedMaterials)
            {
                if (mat != null && mat.shader != null)
                {
                    try
                    {
                        if (mat.HasProperty("_MainTex"))
                        {
                            Texture mainTex = mat.GetTexture("_MainTex");
                            if (mainTex != null)
                            {
                                Texture2D tex = mainTex as Texture2D;
                                if (tex != null)
                                {
                                    memory += tex.width * tex.height * 4;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        return memory;
    }
}
