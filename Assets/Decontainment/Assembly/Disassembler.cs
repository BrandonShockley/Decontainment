using Extensions;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace Asm
{
    public static class Disassembler
    {
        public static string Disassemble(Program program)
        {
            StringBuilder programSB = new StringBuilder();

            // Write all constant labels
            foreach (Label constLabel in program.constLabelList) {
                programSB.Append(constLabel.name + ": " + constLabel.val + "\n");
            }

            int nextLabelIndex = 0;
            for (int lineNumber = 0; lineNumber < program.instructions.Count; ++lineNumber) {
                // Write all branch labels for line
                while (nextLabelIndex != program.branchLabelList.Count && program.branchLabelList[nextLabelIndex].val == lineNumber) {
                    Label branchLabel = program.branchLabelList[nextLabelIndex];
                    programSB.Append(branchLabel.name + ":" + "\n");
                    ++nextLabelIndex;
                }

                // Write instruction
                Instruction instruction = program.instructions[lineNumber];
                ArgumentSpec[] argSpecs = InstructionMaps.opArgSpecMap[instruction.opCode];
                programSB.Append(instruction.opCode.ToString());
                for (int argNum = 0; argNum < instruction.args.Length; ++argNum) {
                    Argument arg = instruction.args[argNum];
                    programSB.Append(" ");
                    switch (arg.type)
                    {
                        case Argument.Type.IMMEDIATE:
                            if (argSpecs[argNum].presets != null) {
                                programSB.Append(argSpecs[argNum].presets[arg.val]);
                            } else {
                                programSB.Append("$" + arg.val);
                            }
                            break;
                        case Argument.Type.REGISTER:
                            programSB.Append("%" + arg.val);
                            break;
                        case Argument.Type.LABEL:
                            programSB.Append(arg.label.name);
                            break;
                    }
                }

                programSB.Append("\n");
            }

            // Write any remaining labels
            for (; nextLabelIndex < program.branchLabelList.Count; ++nextLabelIndex) {
                Label branchLabel = program.branchLabelList[nextLabelIndex];
                programSB.Append(branchLabel.name + ":" + "\n");
                ++nextLabelIndex;
            }

            return programSB.ToString();
        }
    }
}