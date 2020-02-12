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
                        // TODO: When the error prompt ticket is complete, do a prompt here
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

        public WeaponData WeaponData { get { return weaponData; } }

        public static BotData CreateNew(string programName, WeaponData weaponData)
        {
            BotData botData = ScriptableObject.CreateInstance<BotData>();
            botData.customProgramName = programName;
            botData.weaponData = weaponData;
            return botData;
        }
    }
}