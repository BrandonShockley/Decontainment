using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asm
{
    public enum OpCategory
    {
        ACTION,
        SENSING,
        DATA_MANIPULATION,
        CONTROL_FLOW,
        _SIZE,
    }

    public enum OpCode
    {
        // Actions
        DRV, // Drive
        TRN, // Turn
        SHT, // Shoot
        SLP, // Sleep

        // Sensing
        TAR, // Target
        HED, // Heading
        DIS, // Distance
        SCN, // Scan

        // Data manipulation
        SET, // Set
        ADD, // Add
        SUB, // Subtract
        MUL, // Multiply
        DIV, // Divide
        MOD, // Modulo
        ABS, // Absolute value
        RNG, // Random number generate

        // Control flow
        NOP,
        BUN, // Branch unconditionally
        BEQ, // Branch if equal to
        BNE, // Branch if not equal to
        BLT, // Branch if less than
        BLE, // Branch if less than or equal to
        BGT, // Branch if greater than
        BGE, // Branch if greater than or equal to
        BRN, // Branch arg * 1 times out of a hundred
        CSR, // Call subroutine
        RSR, // Return from subroutine

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

        public event Action OnChange;

        private Argument() {}

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
            return new Argument() { type = type, val = val, label = label };
        }

        public void CopyValues(Argument arg)
        {
            type = arg.type;
            val = arg.val;
            label = arg.label;
        }

        public void BroadcastChange()
        {
            OnChange?.Invoke();
        }
    }

    public struct OpInfo
    {
        public readonly string descriptiveName;
        public readonly string description;

        public OpInfo(string descriptiveName, string description)
        {
            this.descriptiveName = descriptiveName;
            this.description = description;
        }
    }

    public struct ArgumentSpec
    {
        public static readonly ArgumentSpec BRANCH_LABEL = ArgumentSpec.MakeOpen("Branch Destination", "Where to branch to in the program.");
        public static readonly ArgumentSpec DEST_REG = ArgumentSpec.MakeRegOnly("Result Register", "Which register to store the result in.");
        public static readonly ArgumentSpec VAL = ArgumentSpec.MakeOpen("Value", "");
        public static readonly ArgumentSpec VAL1 = ArgumentSpec.MakeOpen("Left Value", "");
        public static readonly ArgumentSpec VAL2 = ArgumentSpec.MakeOpen("Right Value", "");

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
        public static readonly ArgumentSpec[] HANDLING_TARGET_SENSING_SPECS = new ArgumentSpec[]
        {
            DEST_REG,
            ArgumentSpec.MakeRegOnly("Target Handle", "")
        };

        /// Open arguments can take registers or immediate values
        public static ArgumentSpec MakeOpen(string name, string description)
        {
            return new ArgumentSpec(name, false, null, description);
        }
        public static ArgumentSpec MakeRegOnly(string name, string description)
        {
            return new ArgumentSpec(name, true, null, description);
        }
        public static ArgumentSpec MakeConcurrency(string description)
        {
            return new ArgumentSpec("Concurrency", false, new string[]{ "Sync", "Async" }, description);
        }

        public readonly string name;
        public readonly bool regOnly;
        /// Array of built-in presets
        /// Only valid if regOnly == false
        public readonly string[] presets;
        public readonly string description;
        public ArgumentSpec(string name, bool regOnly, string[] presets, string description)
        {
            this.name = name;
            this.regOnly = regOnly;
            this.presets = presets;
            this.description = description;
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

        public Instruction DeepCopy()
        {
            Argument[] newArgs = new Argument[args.Length];
            for (int i = 0; i < args.Length; ++i) {
                newArgs[i] = args[i].ShallowCopy();
            }
            return new Instruction(opCode, newArgs);
        }
    }

    public class Program
    {
        public struct Change
        {
            public bool argument;
            public bool instruction;
            public bool branchLabel;
            public bool constLabel;
        }

        public string name = "Unnamed";
        public List<Instruction> instructions = new List<Instruction>();
        public Dictionary<string, Label> labelMap = new Dictionary<string, Label>();
        public List<Label> branchLabelList = new List<Label>();
        public List<Label> constLabelList = new List<Label>();

        public event Action<Change> OnChange;

        public override string ToString() { return name; }

        public void BroadcastArgumentChange()
        {
            OnChange?.Invoke(new Change(){ argument = true });
        }
        public void BroadcastInstructionChange()
        {
            OnChange?.Invoke(new Change(){ instruction = true });
        }
        public void BroadcastBranchLabelChange()
        {
            OnChange?.Invoke(new Change(){ branchLabel = true });
        }
        public void BroadcastConstLabelChange()
        {
            OnChange?.Invoke(new Change(){ constLabel = true });
        }
        public void BroadcastMultiChange(Change change)
        {
            OnChange?.Invoke(change);
        }

        /// Removes from label map/list and instructions
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
                        arg.BroadcastChange();
                    }
                }
            }
        }
    }
}