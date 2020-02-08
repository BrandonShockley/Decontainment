using Asm;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Editor
{
    public class ProgramList : MonoBehaviour
    {
        [SerializeField]
        private CodeList codeList = null;

        /// Used to locate in-project program directory
        [SerializeField]
        private TextAsset assetLocationProgram = null;
        [SerializeField]
        private GameObject listEntryPrefab = null;

        private string programDirectory;
        private List<Program> programs = new List<Program>();

        void Awake()
        {
            #if UNITY_EDITOR
            programDirectory = Directory.GetCurrentDirectory() + "/" + Path.GetDirectoryName(AssetDatabase.GetAssetPath(assetLocationProgram));
            #else
            programDirectory = Directory.GetCurrentDirectory();
            #endif

            // Assemble all program files
            string[] filePaths = Directory.GetFiles(programDirectory, "*.txt");
            foreach (string filePath in filePaths) {
                string programText = File.ReadAllText(filePath);
                Program program = Assembler.Assemble(programText);
                if (program != null) {
                    program.name = Path.GetFileNameWithoutExtension(filePath);
                    Debug.Log("Assembly of program " + program.name + " succeeded");

                    // Setup autosave
                    program.OnChange += () => SaveProgram(program);

                    programs.Add(program);
                } else {
                    Debug.LogWarning("Assembly of file " + filePath + " failed");
                }
            }

            // Populate list
            foreach (Program program in programs) {
                GameObject listEntry = Instantiate(listEntryPrefab, transform);
                listEntry.GetComponent<ListEntry>().Init(program, codeList);
            }
        }

        private void Clear()
        {
            for (int i = transform.childCount - 1; i >= 0; --i) {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void SaveProgram(Program program)
        {
            string programText = Disassembler.Disassemble(program);
            string programPath = programDirectory + "/" + program.name + ".txt";
            StreamWriter programFile = File.CreateText(programPath);
            programFile.Write(programText);
            programFile.Close();
        }
    }
}
