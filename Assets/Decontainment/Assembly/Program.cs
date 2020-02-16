using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asm
{
    public enum OpCategory
    {
        CONTROL_FLOW,
        DATA_MANIPULATION,
        SENSING,
        ACTION,
        _SIZE,
    }

    public enum OpCode
    {
        // Control flow
        NOP,
        BUN, // Branch unconditionally
        BEQ, // Branch if equal to
        BNE, // Branch if not equal to
        BLT, // Branch if less than
        BLE, // Branch if less than or equal to
        BGT, // Branch if greater than
        BGE, // Branch if greater than or equal to
        CSR, // Call subroutine
        RSR, // Return from subroutine

        // Data manipulation
        SET, // Set
        ADD, // Add
        SUB, // Subtract
        MUL, // Multiply
        DIV, // Divide
        MOD, // Modulo
        ABS, // Absolute value

        // Sensing
        TAR, // Target
        HED, // Heading
        SCN, // Scan

        // Actions
        DRV, // Drive
        TRN, // Turn
        SHT, // Shoot
        SLP, // Sleep

        _SIZE,
    }

    public class Argument
    {
        public enum Type
        {
            IMMEDIATE,
            REGISTER,
            LABEL,
        }

        public Type type;
        public int val;
        public Label label;

        public Argument(Type type, int val)
        {
            this.type = type;
            this.val = val;
            this.label = default;
        }
        public Argument(Type type, Label label)
        {
            this.type = type;
            this.val = label.val;
            this.label = label;
        }

        public Argument ShallowCopy()
        {
            return (Argument)this.MemberwiseClone();
        }

        public void CopyValues(Argument arg)
        {
            type = arg.type;
            val = arg.val;
            label = arg.label;
        }
    }

    public struct ArgumentSpec
    {
        public static readonly ArgumentSpec BRANCH_LABEL = ArgumentSpec.MakeOpen("Branch destination");
        public static readonly ArgumentSpec DEST_REG = ArgumentSpec.MakeRegOnly("Destination register");
        public static readonly ArgumentSpec VAL = ArgumentSpec.MakeOpen("Value");
        public static readonly ArgumentSpec VAL1 = ArgumentSpec.MakeOpen("Left value");
        public static readonly ArgumentSpec VAL2 = ArgumentSpec.MakeOpen("Right value");
        public static readonly ArgumentSpec SYNC_PRESETS = new ArgumentSpec("Concurrent", false, new string[]{ "Sync", "Async" });

        public static readonly ArgumentSpec[] NO_INPUT_CONTROL_FLOW_SPECS = new ArgumentSpec[]
        {
            ArgumentSpec.BRANCH_LABEL
        };
        public static readonly ArgumentSpec[] TWO_INPUT_CONTROL_FLOW_SPECS = new ArgumentSpec[]
        {
            ArgumentSpec.BRANCH_LABEL,
            ArgumentSpec.VAL1,
            ArgumentSpec.VAL2
        };
        public static readonly ArgumentSpec[] ONE_INPUT_DATA_MANIPULATION_SPECS = new ArgumentSpec[]
        {
            DEST_REG,
            ArgumentSpec.VAL
        };
        public static readonly ArgumentSpec[] TWO_INPUT_DATA_MANIPULATION_SPECS = new ArgumentSpec[]
        {
            DEST_REG,
            ArgumentSpec.VAL1,
            ArgumentSpec.VAL2
        };

        /// Open arguments can take registers or immediate values
        public static ArgumentSpec MakeOpen(string name)
        {
            return new ArgumentSpec(name, false, null);
        }
        public static ArgumentSpec MakeRegOnly(string name)
        {
            return new ArgumentSpec(name, true, null);
        }

        public readonly string name;
        public readonly bool regOnly;
        /// Array of built-in presets
        /// Only valid if regOnly == false
        public readonly string[] presets;
        public ArgumentSpec(string name, bool regOnly, string[] presets)
        {
            this.name = name;
            this.regOnly = regOnly;
            this.presets = presets;
        }
    }

    public class Label
    {
        public enum Type { BRANCH, CONST }
        public string name;
        public int val;
        public Type type;
        public Label(string name, int val, Type type)
        {
            this.name = name;
            this.val = val;
            this.type = type;
        }

        public override string ToString() { return name; }
    }

    public class Instruction
    {
        public readonly OpCode opCode;
        public readonly Argument[] args;

        public Instruction(OpCode opCode, params Argument[] args)
        {
            this.opCode = opCode;
            ArgumentSpec[] argSpecs = InstructionMaps.opArgSpecMap[opCode];
            this.args = new Argument[argSpecs.Length];
            Array.Copy(args, this.args, Math.Min(this.args.Length, args.Length));

            // Fill in any unspecified args
            for (int ai = args.Length; ai < this.args.Length; ++ai) {
                Argument.Type argType = argSpecs[ai].regOnly
                    ? Argument.Type.REGISTER
                    : Argument.Type.IMMEDIATE;
                this.args[ai] = new Argument(argType, 0);
            }
        }
    }

    public class Program
    {
        public string name = "Unnamed";
        public List<Instruction> instructions = new List<Instruction>();
        public Dictionary<string, Label> labelMap = new Dictionary<string, Label>();
        public List<Label> branchLabelList = new List<Label>();
        public List<Label> constLabelList = new List<Label>();

        public event Action OnChange;
        public event Action OnArgumentChange;
        public event Action OnInstructionChange;
        public event Action OnBranchLabelChange;
        public event Action OnConstLabelChange;

        public override string ToString() { return name; }

        public void BroadcastArgumentChange()
        {
            OnArgumentChange?.Invoke();
            OnChange?.Invoke();
        }
        public void BroadcastInstructionChange()
        {
            OnInstructionChange?.Invoke();
            OnChange?.Invoke();
        }
        public void BroadcastBranchLabelChange()
        {
            OnBranchLabelChange?.Invoke();
            OnChange?.Invoke();
        }
        public void BroadcastConstLabelChange()
        {
            OnConstLabelChange?.Invoke();
            OnChange?.Invoke();
        }

        public void RemoveLabel(Label label)
        {
            labelMap.Remove(label.name);
            Label.Type labelType = label.type;
            List<Label> labelList = labelType == Label.Type.BRANCH ? branchLabelList : constLabelList;
            labelList.Remove(label);

            // Remove references in instructions
            foreach (Instruction i in instructions) {
                foreach (Argument arg in i.args) {
                    if (arg.type == Argument.Type.LABEL && arg.label == label) {
                        arg.label = null;
                        arg.type = Argument.Type.IMMEDIATE;
                        arg.val = 0;
                    }
                }
            }

            if (labelType == Label.Type.BRANCH) {
                BroadcastBranchLabelChange();
            } else {
                BroadcastConstLabelChange();
            }
        }
    }
}