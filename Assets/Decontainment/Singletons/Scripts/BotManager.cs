using Bot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class BotManager : SceneSingleton<BotManager>
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

    [SerializeField]
    private float propagationDelay = 3;

    private List<Controller> bots = new List<Controller>();
    private List<Controller>[] teams = new List<Controller>[2];
    private List<Transform> projectiles = new List<Transform>();

    public event Action<int> OnTeamDisable;

    public List<Transform> Projectiles { get { return projectiles; } }

    void Awake()
    {
        for (int tid = 0; tid < teams.Length; ++tid) {
            teams[tid] = new List<Controller>();
        }

        foreach (Controller bot in FindObjectsOfType<Controller>()) {
            AddBot(bot);
        }
    }

    public void AddBot(Controller bot)
    {
        bot.Health.OnDisable += () => HandleDisable(bot.TeamID);

        bots.Add(bot);
        teams[bot.TeamID].Add(bot);
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

            if (bots[i] != targeter && targetType == type && !bots[i].Health.Disabled) {
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

    public void PropagateRegister(int registerNumber, int registerValue, int botTeamID) {
        StartCoroutine(PropagationCoroutine(registerNumber, registerValue, botTeamID));
    }

    private IEnumerator PropagationCoroutine(int registerNumber, int registerValue, int botTeamID) {
        yield return new WaitForSeconds(propagationDelay);
        foreach (Controller bot in teams[botTeamID]) {
            bot.VM.UpdateSharedReg(registerNumber, registerValue);
        }
        yield break;
    }

    private void HandleDisable(int teamID)
    {
        bool allDisabled = true;
        foreach (Controller bot in teams[teamID]) {
            if (!bot.Health.Disabled) {
                allDisabled = false;
                break;
            }
        }

        if (allDisabled) {
            OnTeamDisable?.Invoke(teamID);
        }
    }
}