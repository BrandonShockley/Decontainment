using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asm
{
    public static class Assembler
    {
        public static Instruction[] Assemble(string codeString)
        {
            codeString = Preprocess(codeString);

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
                                ArgumentMeta argMeta = InstructionMaps.opArgMetaMap[instruction.opCode][argNum];
                                if (argMeta.regOnly && !instruction.args[argNum].isReg) {
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

                            bool isReg = word[0] == 'R';
                            ArgumentMeta argMeta = InstructionMaps.opArgMetaMap[opCode][argCount];
                            if (isReg) {
                                word = word.Substring(1, word.Length - 1);
                            } else if (argMeta.regOnly) {
                                Debug.LogError("Invalid use of immediate value as argument " + argCount
                                    + " for operation " + opCode.ToString() + " on line " + lineCount);
                            }

                            bool valid = int.TryParse(word, out int argVal);
                            if (!valid) {
                                // Could be a built-in macro
                                int macroValue = -1;
                                if (argMeta.macros != null) {
                                    macroValue = Array.FindIndex<string>(argMeta.macros, s => s == word);
                                }

                                if (macroValue != -1) {
                                    argVal = macroValue;
                                } else {
                                    Debug.LogError("Invalid argument " + argCount + " for operation " + opCode.ToString()
                                        + " on line " + lineCount);
                                    return null;
                                }
                            } else if (isReg && (argVal < 0 || argVal >= VirtualMachine.NUM_REGS)) {
                                Debug.LogError("Invalid register number " + argVal + " for operation " + opCode.ToString()
                                     + " on line " + lineCount + ". Max register number is " + (VirtualMachine.NUM_REGS - 1));
                            }
                            instruction.args[argCount++] = new Argument(argVal, isReg);
                        }
                        i = wordEnd;
                        break;
                    }
                }
            }
            if (instruction != null) {
                instructions.Add(instruction);
            }
            return instructions.ToArray();
        }

        // NOTE: Possible optimization could be to use the macro map in the assembler
        // and just sub the values into the instruction initialization

        /// Finds macros and replaces them with their defined values
        private static string Preprocess(string codeString)
        {
            Dictionary<string, string> macros = new Dictionary<string, string>();

            char[] wordEndChars = {' ', '\n', '\r'};
            char[] lineEndChars = {';', '\n', '\r'};

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
                            string macroName = word.Substring(0, word.Length - 1);
                            string macroDef = (macroEnd == wordEnd
                                ? pseudoPC.ToString() // Branch label
                                : codeString.Substring(wordEnd, macroEnd - wordEnd)); // Text macro

                            macros.Add(macroName, macroDef);
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

            // Macro replacement
            for (int i = 0; i < codeString.Length;) {
                char c = codeString[i];

                switch (c) {
                    // Handle Windows line endings (\r\n)
                    case '\r': goto case ';';
                    case '\n': goto case ';';
                    case ';':
                        i = codeString.IndexOfToEnd('\n', i) + 1; // Go to next line
                        break;
                    case ' ':
                        ++i;
                        break;

                    default: {
                        int wordEnd = codeString.IndexOfAnyToEnd(wordEndChars, i);
                        string word = codeString.Substring(i, wordEnd - i);

                        if (macros.TryGetValue(word, out string macroDef)) {
                            codeString = codeString.Substring(0, i) + macroDef + codeString.Substring(wordEnd);
                        } else {
                            i = wordEnd;
                        }
                        break;
                    }
                }
            }
            return codeString;
        }
    }
}