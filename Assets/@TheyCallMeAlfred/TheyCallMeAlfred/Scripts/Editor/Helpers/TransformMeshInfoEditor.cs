using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(Transform))]
[CanEditMultipleObjects]
public class TransformMeshInfoEditor : Editor
{
    private Editor defaultEditor;
    private Transform transformComponent;

    void OnEnable()
    {
        transformComponent = (Transform)target;
        System.Type defaultEditorType = System.Type.GetType("UnityEditor.TransformInspector, UnityEditor");
        if (defaultEditorType != null)
        {
            defaultEditor = Editor.CreateEditor(targets, defaultEditorType);
        }
    }

    void OnDisable()
    {
        if (defaultEditor != null)
        {
            DestroyImmediate(defaultEditor);
        }
    }

    public override void OnInspectorGUI()
    {
        if (defaultEditor != null)
        {
            defaultEditor.OnInspectorGUI();
        }

        var style = new GUIStyle(EditorStyles.label);
        style.fontSize = 10;

        MeshInfoCalculator.MeshInfo meshInfo = MeshInfoCalculator.GetMeshInfo(transformComponent);
        if (meshInfo.totalVertices > 0)
        {
            string meshText = $"Verts: {meshInfo.totalVertices:N0} | Tris: {meshInfo.totalTriangles:N0}";
            if (meshInfo.totalSubmeshes > 1) meshText += $" | Submeshes: {meshInfo.totalSubmeshes}";
            if (meshInfo.totalMaterials > 1) meshText += $" | Materials: {meshInfo.totalMaterials}";
            EditorGUILayout.LabelField(meshText, style);
        }

        Bounds bounds = BoundsCalculator.GetBounds(transformComponent);
        if (bounds.size != Vector3.zero)
        {
            EditorGUILayout.LabelField($"Bounds: {bounds.size.x:F2} x {bounds.size.y:F2} x {bounds.size.z:F2}", style);
        }

        Vector3 scale = transformComponent.localScale;
        if (Mathf.Abs(scale.x - scale.y) > 0.001f || Mathf.Abs(scale.y - scale.z) > 0.001f ||
            Mathf.Abs(scale.x - scale.z) > 0.001f)
        {
            var warningStyle = new GUIStyle(EditorStyles.label);
            warningStyle.fontSize = 10;
            warningStyle.normal.textColor = Color.yellow;
            EditorGUILayout.LabelField("⚠ Non-uniform scale!", warningStyle);
        }

        EditorGUILayout.LabelField(
            $"World Pos: ({transformComponent.position.x:F2}, {transformComponent.position.y:F2}, {transformComponent.position.z:F2})",
            style);

        int childCount = HierarchyInfoProvider.GetTotalChildCount(transformComponent);
        if (childCount > 0)
        {
            EditorGUILayout.LabelField($"Children: {childCount} (including nested)", style);
        }

        int depth = HierarchyInfoProvider.GetHierarchyDepth(transformComponent);
        if (depth > 0)
        {
            EditorGUILayout.LabelField($"Hierarchy Depth: {depth}", style);
        }

        string prefabInfo = ComponentInfoProvider.GetPrefabInfo(transformComponent);
        if (prefabInfo != null)
        {
            EditorGUILayout.LabelField(prefabInfo, style);
        }

        string colliderInfo = ComponentInfoProvider.GetColliderInfo(transformComponent);
        if (colliderInfo != null)
        {
            EditorGUILayout.LabelField(colliderInfo, style);
        }

        string rbInfo = ComponentInfoProvider.GetRigidbodyInfo(transformComponent);
        if (rbInfo != null)
        {
            EditorGUILayout.LabelField(rbInfo, style);
        }

        string lightInfo = ComponentInfoProvider.GetLightInfo(transformComponent);
        if (lightInfo != null)
        {
            EditorGUILayout.LabelField(lightInfo, style);
        }

        int drawCalls = PerformanceMetricsCalculator.EstimateDrawCalls(transformComponent);
        if (drawCalls > 0)
        {
            EditorGUILayout.LabelField($"Est. Draw Calls: {drawCalls}", style);
        }

        long memoryEstimate = PerformanceMetricsCalculator.EstimateMemoryUsage(transformComponent, meshInfo);
        if (memoryEstimate > 0)
        {
            string memoryText = memoryEstimate < 1024 ? $"{memoryEstimate} B" :
                memoryEstimate < 1048576 ? $"{memoryEstimate / 1024f:F2} KB" :
                $"{memoryEstimate / 1048576f:F2} MB";
            EditorGUILayout.LabelField($"Est. Memory: {memoryText}", style);
        }
    }
}