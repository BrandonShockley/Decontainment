using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bot
{
    public class Scanner : MonoBehaviour
    {
        public enum Target
        {
            PROJECTILES,
            OBSTACLES,
            ALLIES,
            ENEMIES,
        }

        private static Dictionary<Target, string> targetLayerMap = new Dictionary<Target, string>()
        {
            {Target.PROJECTILES, "Projectile"},
            {Target.OBSTACLES, "Obstacle"},
            {Target.ALLIES, "Bot"},
            {Target.ENEMIES, "Bot"},
        };

        [SerializeField]
        private int idealDegreesPerPoint = 10;

        private PolygonCollider2D pc;

        void Awake()
        {
            pc = GetComponent<PolygonCollider2D>();
        }

        /// Returns the number of objects the scan finds
        public int Scan(Target target, float direction, float range, float width)
        {
            // Construct arc collider
            int numPoints = Mathf.CeilToInt(width / idealDegreesPerPoint) + 1;
            float realDegreesPerPoint = width / (numPoints - 1);
            Vector2[] points = new Vector2[numPoints + 1];
            points[0] = Vector2.zero;
            for (int i = 0; i < numPoints; ++i) {
                float angle = i * realDegreesPerPoint - width / 2;
                points[i + 1] = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * range;
            }
            pc.points = points;
            pc.transform.localRotation = Quaternion.Euler(0, 0, direction);

            // Check for collisions
            List<Collider2D> results = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.useLayerMask = true;
            filter.useTriggers = true;

            filter.layerMask = LayerMask.GetMask(targetLayerMap[target]);
            int numResults = pc.OverlapCollider(filter, results);

            if (target == Target.ALLIES || target == Target.ENEMIES) {
                // TODO: Extra filtering for bot allegiance
                foreach (Collider2D result in results) {
                    if (result.transform == transform.parent) {
                        --numResults;
                    }
                }
            }

            return numResults;
        }
    }
}
