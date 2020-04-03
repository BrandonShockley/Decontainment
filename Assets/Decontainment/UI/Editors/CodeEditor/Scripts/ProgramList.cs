using Asm;
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

        protected override void InitList()
        {
            if (!Directory.Exists(ProgramDirectory.PATH)) {
                return;
            }

            // Assemble all program files
            string[] filePaths = Directory.GetFiles(ProgramDirectory.PATH, "*.txt");
            foreach (string filePath in filePaths) {
                string programText = File.ReadAllText(filePath);
                Program program = Assembler.Assemble(Path.GetFileNameWithoutExtension(filePath), programText);
                if (program != null) {
                    // Setup autosave
                    program.OnChange += (c) => SaveProgram(program);

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
            newProgram.OnChange += (c) => SaveProgram(newProgram);
            SaveProgram(newProgram);

            #if UNITY_EDITOR && !BUILD_MODE
            AssetDatabase.Refresh();
            #endif

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
            #if UNITY_EDITOR && !BUILD_MODE
            AssetDatabase.RenameAsset(fromPath, name);
            #else
            File.Move(fromPath, toPath);
            #endif
            program.name = name;
        }

        protected override void SubHandleSelect(int oldIndex)
        {
            codeList.Program = this[SelectedIndex];
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
