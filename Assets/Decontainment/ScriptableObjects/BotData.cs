using Asm;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Bot
{
    [CreateAssetMenu(fileName = "BotData", menuName = "ScriptableObjects/BotData", order = 1)]
    public class BotData : ScriptableObject
    {
        private readonly Program FALLBACK_PROGRAM = new Program() { name = "Fallback", instructions = new List<Instruction> { new Instruction(OpCode.NOP) }};

        [SerializeField]
        private TextAsset builtInProgram = null;
        [SerializeField]
        private WeaponData weaponData = null;

        private string customProgramName;

        public Program Program
        {
            get {
                string programText = null;
                if (builtInProgram != null) {
                    programText = builtInProgram.text;
                } else if (customProgramName != null) {
                    try {
                        programText = File.ReadAllText(ProgramDirectory.ProgramPath(customProgramName));
                    } catch {
                        Debug.LogWarning("Error opening program " + customProgramName + ". Using fallback program.");
                        // TODO: When the error prompt ticket is complete (Trello #18), do a prompt here
                    }
                }

                if (programText == null) {
                    Debug.LogWarning("No program provided. Using fallback program.");
                    return FALLBACK_PROGRAM;
                } else {
                    Program program = Assembler.Assemble(programText);
                    if (program == null) {
                        Debug.LogWarning("Assembly failed. Using fallback program.");
                        return FALLBACK_PROGRAM;
                    } else {
                        return program;
                    }
                }
            }
        }

        public WeaponData WeaponData
        {
            get { return weaponData; }
            set {
                weaponData = value;
                Save();
            }
        }

        public string CustomProgramName
        {
            get { return customProgramName; }
            set {
                customProgramName = value;
                Save();
            }
        }

        public static BotData CreateNew(string botName, string programName, WeaponData weaponData)
        {
            BotData botData = ScriptableObject.CreateInstance<BotData>();
            botData.name = botName;
            botData.customProgramName = programName;
            botData.weaponData = weaponData;
            botData.Save();
            return botData;
        }

        public static BotData Load(string path)
        {
            #if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<BotData>(path);
            #else
            StreamReader file = File.OpenText(path);
            string botName = Path.GetFileNameWithoutExtension(path);
            string programName = file.ReadLine();
            string weaponName = file.ReadLine();
            WeaponData weaponData = Resources.Load<WeaponData>(WeaponData.RESOURCES_DIR + "/" + weaponName);
            return CreateNew(botName, programName, weaponData);
            #endif
        }

        public void Save()
        {
            string path = BotDirectory.BotPath(name);

            #if UNITY_EDITOR
            BotData existingAsset = AssetDatabase.LoadAssetAtPath<BotData>(path);
            if (existingAsset == null) {
                AssetDatabase.CreateAsset(this, path);
            } else {
                existingAsset.builtInProgram = builtInProgram;
                existingAsset.customProgramName = customProgramName;
                existingAsset.weaponData = weaponData;
            }
            #else
            StreamWriter file = File.CreateText(path);
            file.WriteLine(customProgramName);
            file.WriteLine(weaponData.name);
            file.Close();
            #endif
        }

        public void DeleteOnDisk()
        {
            string path = BotDirectory.BotPath(name);

            #if UNITY_EDITOR
            AssetDatabase.DeleteAsset(path);
            #else
            File.Delete(path);
            #endif
        }

        public void Rename(string newName)
        {
            DeleteOnDisk();
            name = newName;
            Save();
        }

        public override string ToString() { return name; }
    }
}