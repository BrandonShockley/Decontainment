using Asm;
using System.IO;


public static class ProgramDirectory
{
    #if UNITY_EDITOR && !BUILD_MODE
    public static readonly string PATH = "Assets/Decontainment/BotData/Programs";
    #else
    public static readonly string PATH = Directory.GetCurrentDirectory() + "/Programs";
    #endif

    public const string EXTENSION = ".txt";

    public static string ProgramPath(string programName)
    {
        #if !UNITY_EDITOR || BUILD_MODE
        if (!Directory.Exists(PATH)) {
            Directory.CreateDirectory(PATH);
        }
        #endif
        return PATH + "/" + programName + EXTENSION;
    }
}

public static class BotDirectory
{
    #if UNITY_EDITOR && !BUILD_MODE
    public static readonly string PATH = "Assets/Decontainment/BotData/Bots";
    public const string EXTENSION = ".asset";
    #else
    public static readonly string PATH = Directory.GetCurrentDirectory() + "/Bots";
    public const string EXTENSION = ".bot";
    #endif

    public static string BotPath(string botName)
    {
        #if !UNITY_EDITOR || BUILD_MODE
        if (!Directory.Exists(PATH)) {
            Directory.CreateDirectory(PATH);
        }
        #endif
        return PATH + "/" + botName + EXTENSION;
    }
}

public static class TeamDirectory
{
    #if UNITY_EDITOR && !BUILD_MODE
    public static readonly string PATH = "Assets/Decontainment/BotData/Resources/Teams";
    public const string EXTENSION = ".asset";
    #else
    public static readonly string PATH = Directory.GetCurrentDirectory() + "/Teams";
    public const string EXTENSION = ".team";
    #endif

    public static string TeamPath(string teamName)
    {
        #if !UNITY_EDITOR || BUILD_MODE
        if (!Directory.Exists(PATH)) {
            Directory.CreateDirectory(PATH);
        }
        #endif
        return PATH + "/" + teamName + EXTENSION;
    }
}