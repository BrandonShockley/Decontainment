﻿using Asm;
using Extensions;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Editor.Code
{
    public class ProgramList : EditorList<Program>
    {
        [SerializeField]
        private CodeList codeList = null;

        protected override string DefaultName { get { return "Program"; } }

        public Program FindProgram(string name)
        {
            return items.Find((Program p) => p.name == name);
        }

        protected override void InitList()
        {
            // Assemble all program files
            string[] filePaths = Directory.GetFiles(ProgramDirectory.PATH, "*.txt");
            foreach (string filePath in filePaths) {
                string programText = File.ReadAllText(filePath);
                Program program = Assembler.Assemble(Path.GetFileNameWithoutExtension(filePath), programText);
                if (program != null) {
                    // Setup autosave
                    program.OnChange += () => SaveProgram(program);

                    items.Add(program);
                } else {
                    Debug.LogWarning("Assembly of file " + filePath + " failed");
                }
            }
        }

        protected override Program CreateNewItem(string name)
        {
            Program newProgram = new Program();
            newProgram.name = name;
            newProgram.OnChange += () => SaveProgram(newProgram);
            SaveProgram(newProgram);

            return newProgram;
        }

        protected override void DeleteItem(Program program)
        {
            File.Delete(ProgramDirectory.ProgramPath(program.name));
            codeList.Program = null;
        }

        protected override void RenameItem(Program program, string name)
        {
            string fromPath = ProgramDirectory.ProgramPath(program.name);
            string toPath = ProgramDirectory.ProgramPath(name);
            #if UNITY_EDITOR
            Debug.Log(AssetDatabase.RenameAsset(fromPath, name));
            #else
            File.Move(fromPath, toPath);
            #endif
            program.name = name;
        }

        protected override void SubHandleSelect()
        {
            codeList.Program = items[SelectedIndex];
        }

        private void SaveProgram(Program program)
        {
            string programText = Disassembler.Disassemble(program);
            StreamWriter programFile = File.CreateText(ProgramDirectory.ProgramPath(program.name));
            programFile.Write(programText);
            programFile.Close();
        }
    }
}
