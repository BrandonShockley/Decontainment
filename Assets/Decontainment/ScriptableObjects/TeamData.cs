using Bot;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamData", menuName = "ScriptableObjects/TeamData", order = 1)]
public class TeamData : ScriptableObject
{
    public const int TEAM_SIZE = 3;

    /// Use this when creating teams in the Unity editor
    [SerializeField]
    private BotData[] builtInBotDatas;

    /// Use this when creating teams at build runtime
    private string[] customBotNames;

    public event Action<int> OnBotChanged;

    public BotData[] BotDatas
    {
        get {
            #if UNITY_EDITOR
            return builtInBotDatas;
            #else
            if (customBotNames != null) {
                BotData[] botDatas = new BotData[customBotNames.Length];
                for (int i = 0; i < botDatas.Length; ++i) {
                    botDatas[i] = BotData.Load(BotDirectory.BotPath(customBotNames[i]));
                }
                return botDatas;
            } else {
                return null;
            }
            #endif
        }
    }

    public int BotCount
    {
        get {
            #if UNITY_EDITOR
            return builtInBotDatas.Length;
            #else
            return customBotNames.Length;
            #endif
        }
    }

    public static TeamData CreateNew(string teamName, string[] botNames)
    {
        TeamData teamData = ScriptableObject.CreateInstance<TeamData>();
        teamData.name = teamName;
        #if UNITY_EDITOR
        teamData.builtInBotDatas = new BotData[TEAM_SIZE];
        #else
        teamData.customBotNames = new string[TEAM_SIZE];
        #endif
        for (int i = 0; i < botNames.Length; ++i) {
            teamData.SetBotName(i, botNames[i]);
        }
        return teamData;
    }

    public static TeamData Load(string path)
    {
        #if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<TeamData>(path);
        #else
        StreamReader file = File.OpenText(path);
        string teamName = Path.GetFileNameWithoutExtension(path);
        int numBots = int.Parse(file.ReadLine());
        string[] botNames = new string[numBots];
        for (int i = 0; i < numBots; ++i) {
            botNames[i] = file.ReadLine();
        }
        return CreateNew(teamName, botNames);
        #endif
    }

    public string GetBotName(int index)
    {
        #if UNITY_EDITOR
        if (builtInBotDatas == null) {
            return null;
        } else {
            return builtInBotDatas[index].name;
        }
        #else
        if (customBotNames == null) {
            return null;
        } else {
            return customBotNames[index];
        }
        #endif
    }

    public void SetBotName(int index, string name)
    {
        #if UNITY_EDITOR
        if (name == null) {
            builtInBotDatas[index] = null;
        } else {
            string path = BotDirectory.BotPath(name);
            builtInBotDatas[index] = AssetDatabase.LoadAssetAtPath<BotData>(path);
        }
        #else
        customBotNames[index] = name;
        #endif
        Save();
        OnBotChanged?.Invoke(index);
    }

    public void Save()
    {
        string path = TeamDirectory.TeamPath(name);

        #if UNITY_EDITOR
        TeamData existingAsset = AssetDatabase.LoadAssetAtPath<TeamData>(path);
        if (existingAsset == null) {
            AssetDatabase.CreateAsset(this, path);
        } else {
            existingAsset.builtInBotDatas = builtInBotDatas;
            existingAsset.customBotNames = customBotNames;
        }
        #else
        StreamWriter file = File.CreateText(path);
        file.WriteLine(customBotNames.Length.ToString());
        foreach (string botName in customBotNames) {
            file.WriteLine(botName);
        }
        file.Close();
        #endif
    }

    public void DeleteOnDisk()
    {
        string path = TeamDirectory.TeamPath(name);

        #if UNITY_EDITOR
        AssetDatabase.DeleteAsset(path);
        #else
        File.Delete(path);
        #endif
    }

    public void Rename(string newName)
    {
        string fromPath = TeamDirectory.TeamPath(name);
        string toPath = TeamDirectory.TeamPath(newName);

        #if UNITY_EDITOR
        AssetDatabase.RenameAsset(fromPath, newName);
        #else
        File.Move(fromPath, toPath);
        #endif
        name = newName;
    }

    public override string ToString() { return name; }
}
