using Asm;
using System.IO;

public static class ProgramDirectory
{
    #if UNITY_EDITOR
    public static readonly string value = Directory.GetCurrentDirectory() + "/Assets/Decontainment/BotData/Programs";
    #else
    public static readonly string value = Directory.GetCurrentDirectory() + "/Programs";
    #endif

    public static string ProgramPath(string programName)
    {
        #if !UNITY_EDITOR
        if (!Directory.Exists(value)) {
            Directory.CreateDirectory(value);
        }
        #endif
        return value + "/" + programName + ".txt";
    }
}