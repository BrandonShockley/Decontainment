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
            // Assemble all program files
            string[] filePaths = Directory.GetFiles(ProgramDirectory.PATH, "*.txt");
            foreach (string filePath in filePaths) {
                string programText = File.ReadAllText(filePath);
                Program program = Assembler.Assemble(programText);
                if (program != null) {
                    program.name = Path.GetFileNameWithoutExtension(filePath);
                    Debug.Log("Assembly of program " + program.name + " succeeded");

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
            // Remove old file
            File.Delete(ProgramDirectory.ProgramPath(program.name));

            // Update back end
            program.name = name;

            // Create new file
            SaveProgram(program);
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
