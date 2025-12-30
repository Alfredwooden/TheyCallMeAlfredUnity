using UnityEngine;

public static class BoundsCalculator
{
    public static Bounds GetBounds(Transform transform)
    {
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        bool hasBounds = false;

        Renderer renderer = transform.GetComponent<Renderer>();
        if (renderer != null)
        {
            bounds = renderer.bounds;
            hasBounds = true;
        }

        Collider collider = transform.GetComponent<Collider>();
        if (collider != null)
        {
            if (hasBounds)
            {
                bounds.Encapsulate(collider.bounds);
            }
            else
            {
                bounds = collider.bounds;
                hasBounds = true;
            }
        }

        foreach (Transform child in transform)
        {
            Bounds childBounds = GetBounds(child);
            if (childBounds.size != Vector3.zero)
            {
                if (hasBounds)
                {
                    bounds.Encapsulate(childBounds);
                }
                else
                {
                    bounds = childBounds;
                    hasBounds = true;
                }
            }
        }

        return bounds;
    }
}
