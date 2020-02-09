using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class BotManager : PersistentSingleton<BotManager>
{
    public enum TargetType
    {
        ALLY,
        ENEMY,
    }
    public enum DistanceType
    {
        NEAREST,
        FARTHEST,
    }

    public float propagationDelay;

    private List<Controller> bots;
    private List<Controller>[] teams = new List<Controller>[2];
    private List<Transform> projectiles = new List<Transform>();

    public List<Transform> Projectiles { get { return projectiles; } }

    void Awake()
    {
        propagationDelay = 3.0f;
        for (int tid = 0; tid < teams.Length; ++tid) {
            teams[tid] = new List<Controller>();
        }

        bots = new List<Controller>(FindObjectsOfType<Controller>());
        foreach (Controller bot in bots) {
            teams[bot.TeamID].Add(bot);
        }
    }

    /// Returns -1 if no valid targets
    public int FindTarget(Controller targeter, DistanceType distanceType, TargetType targetType)
    {
        int targetIndex = -1;
        float targetDistance = distanceType == DistanceType.NEAREST ? 0 : float.PositiveInfinity;
        for (int i = 0; i < bots.Count; ++i) {
            TargetType type = bots[i].TeamID == targeter.TeamID
                ? TargetType.ALLY
                : TargetType.ENEMY;

            if (bots[i] != targeter && targetType == type) {
                if (targetIndex == -1) {
                    targetIndex = i;
                    targetDistance = Util.Distance(targeter.transform.position, bots[i].transform.position);
                } else {
                    float distance = Util.Distance(targeter.transform.position, bots[i].transform.position);
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
        if (targetIndex < 0 || targetIndex >= bots.Count) {
            Debug.LogWarning("Bot " + targeter + " attempted to get target heading for invalid target index");
            return 0;
        }

        Controller target = bots[targetIndex];
        Vector2 look = target.transform.position - targeter.transform.position;
        return (int)Vector2.SignedAngle(targeter.transform.right, look);
    }

    public IEnumerator PropagationCoroutine (int registerNumber, int registerValue, int botTeamID) {
        //Debug.Log("Propagating with delay: " + propagationDelay);
        yield return new WaitForSecondsRealtime(propagationDelay);
        //Debug.Log("Setting Register Number: " + registerNumber + " to Register Value: " + registerValue + " on Team: " + botTeamID);
        foreach (Controller bot in teams[botTeamID]) {
            bot.VM.updateSharedReg(registerNumber, registerValue);
        }
        yield break;
    }
}