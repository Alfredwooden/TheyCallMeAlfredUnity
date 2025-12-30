using UnityEngine;

public static class HierarchyInfoProvider
{
    public static int GetTotalChildCount(Transform transform)
    {
        int count = transform.childCount;
        foreach (Transform child in transform)
        {
            count += GetTotalChildCount(child);
        }
        return count;
    }

    public static int GetHierarchyDepth(Transform transform)
    {
        int depth = 0;
        Transform parent = transform.parent;
        while (parent != null)
        {
            depth++;
            parent = parent.parent;
        }
        return depth;
    }
}
