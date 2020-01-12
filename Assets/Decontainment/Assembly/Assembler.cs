using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asm
{
    public static class Assembler
    {
        public static Program Assemble(string codeString)
        {
            Program output = new Program();
            if (!Preprocess(ref codeString, out output.branchLabelList, out output.constLabelList)) {
                return null;
            }

            // Build label maps
            output.labelMap = new Dictionary<string, Label>();
            foreach (var label in output.branchLabelList) {
                output.labelMap.Add(label.name, label);
            }
            foreach (var label in output.constLabelList) {
                output.labelMap.Add(label.name, label);
            }

            List<Instruction> instructions = new List<Instruction>();
            char[] endChars = {' ', '\n', '\r'};
            int lineCount = 1;

            Instruction instruction = null;
            int argCount = 0;

            for (int i = 0; i < codeString.Length;) {
                char c = codeString[i];

                switch (c) {
                    // Handle Windows line endings (\r\n)
                    case '\r': goto case ';';
                    case '\n': goto case ';';
                    case ';':
                        if (instruction != null) {
                            // Check that register arguments are assigned
                            for (int argNum = 0; argNum < instruction.args.Length; ++argNum) {
                                ArgumentSpec argSpec = InstructionMaps.opArgSpecMap[instruction.opCode][argNum];
                                if (argSpec.regOnly && instruction.args[argNum].type != Argument.Type.REGISTER) {
                                    Debug.LogError("Register number not provided for argument " + argNum
                                        + " for operation " + instruction.opCode.ToString()
                                        + " on line " + lineCount);
                                    return null;
                                }
                            }

                            instructions.Add(instruction);
                            instruction = null;
                            argCount = 0;
                        }
                        i = codeString.IndexOfToEnd('\n', i) + 1; // Go to next line
                        ++lineCount;
                        break;
                    case ' ':
                        ++i;
                        break;

                    default: {
                        int wordEnd = codeString.IndexOfAnyToEnd(endChars, i);
                        string word = codeString.Substring(i, wordEnd - i);

                        if (instruction == null) {
                            // New instruction
                            bool valid = InstructionMaps.nameOpMap.TryGetValue(word, out OpCode opCode);
                            if (!valid) {
                                Debug.LogError("Invalid operation " + word + " on line " + lineCount);
                                return null;
                            }

                            instruction = new Instruction(opCode);
                        } else {
                            // Argument
                            OpCode opCode = instruction.opCode;
                            if (argCount == InstructionMaps.opArgNumMap[opCode]) {
                                Debug.LogError("Invalid number of arguments for operation " + opCode.ToString()
                                    + " on line " + lineCount);
                                return null;
                            }

                            ArgumentSpec argSpec = InstructionMaps.opArgSpecMap[opCode][argCount];
                            // Check for preset value
                            int presetValue = -1;
                            if (argSpec.presets != null) {
                                presetValue = Array.FindIndex<string>(argSpec.presets, s => s == word);
                            }

                            Argument.Type type;
                            if (word[0] == '%') {
                                type = Argument.Type.REGISTER;
                                word = word.Substring(1, word.Length - 1);
                            } else if (word[0] == '$') {
                                type = Argument.Type.IMMEDIATE;
                                word = word.Substring(1, word.Length - 1);
                            } else if (presetValue != -1) {
                                type = Argument.Type.IMMEDIATE;
                            } else {
                                type = Argument.Type.LABEL;
                            }

                            if (argSpec.regOnly && type != Argument.Type.REGISTER) {
                                Debug.LogError("Argument " + argCount
                                    + " for operation " + opCode.ToString() + " on line " + lineCount
                                    + " must be a register value");
                                return null;
                            }

                            if (type == Argument.Type.IMMEDIATE || type == Argument.Type.REGISTER) {
                                int argVal;
                                if (presetValue != -1) {
                                    argVal = presetValue;
                                } else if (int.TryParse(word, out argVal)) {
                                    if (type == Argument.Type.REGISTER && (argVal < 0 || argVal >= VirtualMachine.NUM_REGS)) {
                                        Debug.LogError("Invalid register number " + argVal + " for operation " + opCode.ToString()
                                            + " on line " + lineCount + ". Max register number is " + (VirtualMachine.NUM_REGS - 1));
                                        return null;
                                    }
                                } else {
                                    Debug.LogError("Invalid argument " + argCount + " for operation " + opCode.ToString()
                                        + " on line " + lineCount);
                                    return null;
                                }

                                instruction.args[argCount++] = new Argument(type, argVal);
                            } else {
                                bool validLabel = output.labelMap.ContainsKey(word);
                                if (!validLabel) {
                                    Debug.LogError("Invalid label " + word + " on line " + lineCount);
                                    return null;
                                }
                                instruction.args[argCount++] = new Argument(type, output.labelMap[word]);
                            }
                        }
                        i = wordEnd;
                        break;
                    }
                }
            }
            if (instruction != null) {
                instructions.Add(instruction);
            }
            output.instructions = instructions;
            return output;
        }

        /// Discovers and removes label definitions
        /// Returns map of discovered labels
        public static bool Preprocess(ref string codeString, out List<Label> branchLabels, out List<Label> constLabels)
        {
            branchLabels = new List<Label>();
            constLabels = new List<Label>();

            char[] wordEndChars = {' ', '\n', '\r'};
            char[] lineEndChars = {';', '\n', '\r'};

            int lineCount = 1;
            int pseudoPC = 0;
            bool parsingInstruction = false;

            // Macro discovery
            for (int i = 0; i < codeString.Length;) {
                char c = codeString[i];

                switch (c) {
                    // Handle Windows line endings (\r\n)
                    case '\r': goto case ';';
                    case '\n': goto case ';';
                    case ';':
                        if (parsingInstruction) {
                            ++pseudoPC;
                            parsingInstruction = false;
                        }
                        i = codeString.IndexOfToEnd('\n', i) + 1; // Go to next line
                        ++lineCount;
                        break;
                    case ' ':
                        ++i;
                        break;

                    default: {
                        int wordEnd = codeString.IndexOfAnyToEnd(wordEndChars, i);
                        string word = codeString.Substring(i, wordEnd - i);

                        if (word.Length > 1 && word.IndexOf(':') == word.Length - 1) {
                            // We've found a macro defenition
                            int macroEnd = codeString.IndexOfAnyToEnd(lineEndChars, i);
                            bool isBranchLabel = macroEnd == wordEnd;
                            string macroName = word.Substring(0, word.Length - 1);
                            string macroDef = (isBranchLabel
                                ? pseudoPC.ToString()
                                : codeString.Substring(wordEnd, macroEnd - wordEnd));

                            if (!int.TryParse(macroDef, out int macroVal)) {
                                Debug.LogError("Invalid macro definition " + macroDef + " on line " + lineCount);
                                return false;
                            }
                            if (isBranchLabel) {
                                branchLabels.Add(new Label(macroName, macroVal, Label.Type.BRANCH));
                            } else {
                                constLabels.Add(new Label(macroName, macroVal, Label.Type.CONST));
                            }

                            // Delete definition
                            codeString = codeString.Substring(0, i) + codeString.Substring(macroEnd);
                        } else {
                            parsingInstruction = true;
                            i = wordEnd;
                        }
                        break;
                    }
                }
            }
            return true;
        }
    }
}