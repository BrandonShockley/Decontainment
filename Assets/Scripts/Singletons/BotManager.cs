using Bot;
using System.Collections.Generic;
using UnityEngine;

class BotManager : Singleton<BotManager>
{
    public enum DistanceType
    {
        NEAREST,
        FARTHEST,
    }

    private List<Controller> bots;
    private List<Transform> projectiles = new List<Transform>();

    public List<Transform> Projectiles { get { return projectiles; } }

    void Awake()
    {
        bots = new List<Controller>(FindObjectsOfType<Controller>());
    }

    /// Returns -1 if no valid targets
    public int FindTarget(Controller targeter, DistanceType distanceType)
    {
        int targetIndex = -1;
        float targetDistance = distanceType == DistanceType.NEAREST ? 0 : float.PositiveInfinity;
        for (int i = 0; i < bots.Count; ++i) {
            if (bots[i] != targeter) {
                if (targetIndex == -1) {
                    targetIndex = i;
                    targetDistance = Distance(targeter.transform.position, bots[i].transform.position);
                } else {
                    float distance = Distance(targeter.transform.position, bots[i].transform.position);
                    if (distanceType == DistanceType.NEAREST
                            ? distance < targetDistance : distance > targetDistance) {
                        targetIndex = i;
                        targetDistance = distance;
                    }
                }
            }
        }
        return targetIndex;
    }

    public int GetTargetHeading(Controller targeter, int targetIndex)
    {
        Controller target = bots[targetIndex];
        Vector2 look = target.transform.position - targeter.transform.position;
        return (int)Vector2.SignedAngle(targeter.transform.right, look);
    }

    // TODO: Move this into controller and have it use the cone trigger
    /// Returns number of projectiles within arc in front of scanner
    /// defined by width and range
    /// Ignores projectiles traveling away from bot
    public int ScanProjectiles(Controller scanner, float direction, float range, float width)
    {
        int pCount = 0;
        Vector2 scanPos = scanner.transform.position;
        Vector2 scanLookVec = scanner.transform.right;
        foreach (Transform p in projectiles) {
            Vector2 pLookVec = p.transform.right;
            bool incoming = Vector2.Dot(scanLookVec, pLookVec) < 0;

            Vector2 pPos = p.position;
            Vector2 diff = pPos - scanPos;
            bool inRange = diff.magnitude < range;
            bool inWidth = Mathf.Abs(Vector2.SignedAngle(scanLookVec, diff) - direction) <= width / 2;

            if (incoming && inRange && inWidth) {
                ++pCount;
            }
        }
        return pCount;
    }

    private float Distance(Vector2 v1, Vector2 v2)
    {
        return (v1 - v2).magnitude;
    }
}