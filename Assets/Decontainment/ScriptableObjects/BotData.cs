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

        public event Action OnProgramChange;
        public event Action OnWeaponChange;

        public WeaponData WeaponData
        {
            get { return weaponData; }
            set {
                weaponData = value;
                Save();
                OnWeaponChange?.Invoke();
            }
        }

        public string ProgramName
        {
            get {
                #if UNITY_EDITOR
                if (builtInProgram == null) {
                    return null;
                } else {
                    return builtInProgram.name;
                }
                #else
                return customProgramName;
                #endif
            }
            set {
                if (value == null) {
                    builtInProgram = null;
                    customProgramName = null;
                } else {
                    string path = ProgramDirectory.ProgramPath(value);
                    #if UNITY_EDITOR
                    builtInProgram = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                    #else
                    customProgramName = value;
                    #endif
                }
                Save();
                OnProgramChange?.Invoke();
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

        public Program AssembleProgram()
        {
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
                Program program = Assembler.Assemble(customProgramName, programText);
                if (program == null) {
                    Debug.LogWarning("Assembly failed. Using fallback program.");
                    return FALLBACK_PROGRAM;
                } else {
                    return program;
                }
            }
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
            string fromPath = BotDirectory.BotPath(name);
            string toPath = BotDirectory.BotPath(newName);

            #if UNITY_EDITOR
            AssetDatabase.RenameAsset(fromPath, newName);
            #else
            File.Move(fromPath, toPath);
            #endif
            name = newName;
        }

        public override string ToString() { return name; }
    }
}