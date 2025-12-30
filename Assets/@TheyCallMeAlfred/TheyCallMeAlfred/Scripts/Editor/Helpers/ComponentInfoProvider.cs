using UnityEngine;
using UnityEditor;

public static class ComponentInfoProvider
{
    public static string GetColliderInfo(Transform transform)
    {
        Collider collider = transform.GetComponent<Collider>();
        if (collider == null) return null;

        string info = $"Collider: {collider.GetType().Name}";

        if (collider is BoxCollider box)
        {
            info += $" | Size: {box.size.x:F2} x {box.size.y:F2} x {box.size.z:F2}";
        }
        else if (collider is SphereCollider sphere)
        {
            info += $" | Radius: {sphere.radius:F2}";
        }
        else if (collider is CapsuleCollider capsule)
        {
            info += $" | Radius: {capsule.radius:F2} | Height: {capsule.height:F2}";
        }

        return info;
    }

    public static string GetRigidbodyInfo(Transform transform)
    {
        Rigidbody rb = transform.GetComponent<Rigidbody>();
        if (rb == null) return null;

        string info = $"Rigidbody: Mass={rb.mass:F2}";
        if (rb.isKinematic) info += " | Kinematic";
        if (rb.useGravity) info += " | Gravity";

        return info;
    }

    public static string GetLightInfo(Transform transform)
    {
        Light light = transform.GetComponent<Light>();
        if (light == null) return null;

        string info = $"Light: {light.type} | Intensity={light.intensity:F2}";
        if (light.type == LightType.Point || light.type == LightType.Spot)
        {
            info += $" | Range={light.range:F2}";
        }

        return info;
    }

    public static string GetPrefabInfo(Transform transform)
    {
        if (!PrefabUtility.IsPartOfAnyPrefab(transform)) return null;

        PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(transform);
        PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(transform);
        string prefabStatus = status == PrefabInstanceStatus.Connected ? "Connected" : "Disconnected";

        return $"Prefab: {prefabType} ({prefabStatus})";
    }
}
