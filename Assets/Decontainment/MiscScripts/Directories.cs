using Asm;
using System.IO;

public static class ProgramDirectory
{
    #if UNITY_EDITOR
    public static readonly string PATH = "Assets/Decontainment/BotData/Programs";
    #else
    public static readonly string PATH = Directory.GetCurrentDirectory() + "/Programs";
    #endif

    public static string ProgramPath(string programName)
    {
        #if !UNITY_EDITOR
        if (!Directory.Exists(PATH)) {
            Directory.CreateDirectory(PATH);
        }
        #endif
        return PATH + "/" + programName + ".txt";
    }
}

public static class BotDirectory
{
    #if UNITY_EDITOR
    public static readonly string PATH = "Assets/Decontainment/BotData/Bots";
    public const string EXTENSION = ".asset";
    #else
    public static readonly string PATH = Directory.GetCurrentDirectory() + "/Bots";
    public const string EXTENSION = ".bot";
    #endif

    public static string BotPath(string botName)
    {
        #if !UNITY_EDITOR
        if (!Directory.Exists(PATH)) {
            Directory.CreateDirectory(PATH);
        }
        #endif
        return PATH + "/" + botName + EXTENSION;
    }
}