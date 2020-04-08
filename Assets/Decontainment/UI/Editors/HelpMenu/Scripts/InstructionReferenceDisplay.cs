using Asm;
using TMPro;
using UnityEngine;

namespace Editor.Help
{
    public class InstructionReferenceDisplay : ContentDisplay<Instruction, InstructionsList>
    {
        protected override void Generate()
        {
            if (contentList.SelectedItem == null) {
                return;
            }

            OpCode opCode = contentList.SelectedItem.opCode;
            OpInfo opInfo = InstructionMaps.opDescriptiveNameMap[opCode];
            ArgumentSpec[] argSpecs = InstructionMaps.opArgSpecMap[opCode];

            // Title
            GenerateText(titlePrefab, opCode.ToString(), "Title");
            Instantiate(smallSpacerPrefab, transform);

            // Summary
            string summaryText = opInfo.descriptiveName;
            foreach (ArgumentSpec argSpec in argSpecs) {
                summaryText += " [" + argSpec.name + "]";
            }
            GenerateText(headerPrefab, summaryText, "Summary");

            // Description
            GenerateText(textPrefab, opInfo.description, "Description");
            Instantiate(bigSpacerPrefab, transform);

            if (argSpecs.Length > 0) {
                // Arguments
                foreach (ArgumentSpec argSpec in argSpecs) {
                    string argSpecText = argSpec.name + ": ";
                    if (argSpec.regOnly) {
                        argSpecText += "Register";
                    } else if (argSpec.presets == null) {
                        argSpecText += "Value";
                    } else {
                        argSpecText += "(";
                        for (int pi = 0; pi < argSpec.presets.Length; ++pi) {
                            argSpecText += argSpec.presets[pi];
                            if (pi + 1 < argSpec.presets.Length) {
                                argSpecText += ", ";
                            }
                        }
                        argSpecText += ")";
                    }
                    GenerateText(headerPrefab, argSpecText, "ArgHeader");
                    GenerateText(textPrefab, argSpec.description, "ArgDescription");
                    Instantiate(smallSpacerPrefab, transform);
                }
            }
        }
    }
}