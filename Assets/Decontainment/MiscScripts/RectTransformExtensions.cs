using UnityEngine;

namespace Extensions
{
    public static class RectTransformExtensions
    {
        public static bool RectOverlaps(this RectTransform rectTrans1, RectTransform rectTrans2)
        {
            Rect rect1 = new Rect(rectTrans1.position.x - rectTrans1.rect.width / 2,
                rectTrans1.position.y - rectTrans1.rect.height / 2,
                rectTrans1.rect.width, rectTrans1.rect.height);
            Rect rect2 = new Rect(rectTrans2.position.x - rectTrans2.rect.width / 2,
                rectTrans2.position.y - rectTrans2.rect.height / 2,
                rectTrans2.rect.width, rectTrans2.rect.height);

            return rect1.Overlaps(rect2);
        }

        public static Vector2 GetWorldSize(this RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return corners[3] - corners[0];
        }
    }
}