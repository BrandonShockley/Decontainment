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
                CreateListEntry(program);
            }
        }

        public void AddProgram()
        {
            string defaultName = "Program";
            string newName;
            for (int i = 0;; ++i) {
                newName = defaultName + i.ToString();
                bool nameGood = true;
                foreach (Program p in programs) {
                    if (p.name == newName) {
                        nameGood = false;
                        break;
                    }
                }

                if (nameGood) {
                    break;
                }
            }

            Program newProgram = new Program();
            newProgram.name = newName;
            newProgram.OnChange += () => SaveProgram(newProgram);
            SaveProgram(newProgram);

            // Insert alphabetically TODO: Pull this out into a utility that can be used with any list
            int index;
            for (index = 0; index < programs.Count; ++index) {
                if (string.Compare(newProgram.name, programs[index].name) <= 0) {
                    break;
                }
            }
            programs.Insert(index, newProgram);

            // Update the front end
            CreateListEntry(newProgram, index);
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

        private void CreateListEntry(Program program, int siblingIndex = -1)
        {
            GameObject listEntry = Instantiate(listEntryPrefab, transform);
            listEntry.GetComponent<ListEntry>().Init(program, codeList);
            if (siblingIndex >= 0) {
                listEntry.transform.SetSiblingIndex(siblingIndex);
            }
        }
    }
}
