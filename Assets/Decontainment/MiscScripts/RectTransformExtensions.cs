using UnityEngine;

namespace Extensions
{
    public static class RectTransformExtensions
    {
        public static Vector2 GetWorldSize(this RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return corners[2] - corners[0];
        }

        public static Rect GetWorldRect(this RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector2 size = corners[2] - corners[0];
            return new Rect(corners[0], size);
        }

        public static bool RectOverlaps(this RectTransform rt1, RectTransform rt2)
        {
            return rt1.GetWorldRect().Overlaps(rt2.GetWorldRect());
        }
    }
}