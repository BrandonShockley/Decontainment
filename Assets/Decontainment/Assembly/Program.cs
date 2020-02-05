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
        public static readonly ArgumentSpec BRANCH_LABEL = ArgumentSpec.MakeOpen("Branch To");
        public static readonly ArgumentSpec DEST_REG = ArgumentSpec.MakeRegOnly("Result");
        public static readonly ArgumentSpec VAL = ArgumentSpec.MakeOpen("Value");
        public static readonly ArgumentSpec VAL1 = ArgumentSpec.MakeOpen("Value 1");
        public static readonly ArgumentSpec VAL2 = ArgumentSpec.MakeOpen("Value 2");
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

        public string name;
        public bool regOnly;
        /// Array of built-in presets
        /// Only valid if regOnly == false
        public string[] presets;
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
    }

    public class Instruction
    {
        public OpCode opCode;
        public Argument[] args;
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
        public event Action OnInstructionChange;
        public event Action OnBranchLabelChange;
        public event Action OnConstLabelChange;

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

    public static class InstructionMaps
    {
        /// OpCode to argument specification array map
        public static Dictionary<OpCode, ArgumentSpec[]> opArgSpecMap = new Dictionary<OpCode, ArgumentSpec[]>()
        {
            {OpCode.NOP, new ArgumentSpec[0]},
            {OpCode.BUN, ArgumentSpec.NO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.BEQ, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.BNE, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.BLT, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.BLE, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.BGT, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.BGE, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.CSR, ArgumentSpec.NO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.RSR, new ArgumentSpec[0]},

            {OpCode.SET, ArgumentSpec.ONE_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.ADD, ArgumentSpec.TWO_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.SUB, ArgumentSpec.TWO_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.MUL, ArgumentSpec.TWO_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.DIV, ArgumentSpec.TWO_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.MOD, ArgumentSpec.TWO_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.ABS, ArgumentSpec.ONE_INPUT_DATA_MANIPULATION_SPECS},

            {OpCode.TAR, new ArgumentSpec[]
                {
                    ArgumentSpec.DEST_REG,
                    new ArgumentSpec("Distance", false, new string[]{ "Nearest", "Farthest" }),
                    new ArgumentSpec("Type", false, new string[]{ "Ally", "Enemy" })
                }
            },
            {OpCode.HED, new ArgumentSpec[]
                {
                    ArgumentSpec.DEST_REG,
                    ArgumentSpec.MakeRegOnly("Target Index")
                }
            },
            {OpCode.SCN, new ArgumentSpec[]
                {
                    ArgumentSpec.DEST_REG,
                    new ArgumentSpec("Type", false, new string[]{ "Projectiles", "Obstacles", "Allies", "Enemies" }),
                    ArgumentSpec.MakeOpen("Radial Offset"),
                    ArgumentSpec.MakeOpen("Radial Width"),
                    ArgumentSpec.MakeOpen("Distance")
                }
            },

            {OpCode.DRV, new ArgumentSpec[]
                {
                    new ArgumentSpec("Direction", false, new string[]{ "Forward", "Backward", "Left", "Right"}),
                    ArgumentSpec.MakeOpen("Distance"),
                    ArgumentSpec.SYNC_PRESETS
                }
            },
            {OpCode.TRN, new ArgumentSpec[]
                {
                    new ArgumentSpec("Direction", false, new string[]{ "Left", "Right" }),
                    ArgumentSpec.MakeOpen("Degrees"),
                    ArgumentSpec.SYNC_PRESETS
                }
            },
            {OpCode.SHT, new ArgumentSpec[]
                {
                    ArgumentSpec.SYNC_PRESETS
                }
            },
            {OpCode.SLP, new ArgumentSpec[]{ ArgumentSpec.MakeOpen("Duration") }},

        };

        /// OpCode to OpCategory map
        public static Dictionary<OpCode, OpCategory> opCodeOpCategoryMap = new Dictionary<OpCode, OpCategory>()
        {
            {OpCode.NOP, OpCategory.CONTROL_FLOW},
            {OpCode.BUN, OpCategory.CONTROL_FLOW},
            {OpCode.BEQ, OpCategory.CONTROL_FLOW},
            {OpCode.BNE, OpCategory.CONTROL_FLOW},
            {OpCode.BLT, OpCategory.CONTROL_FLOW},
            {OpCode.BLE, OpCategory.CONTROL_FLOW},
            {OpCode.BGT, OpCategory.CONTROL_FLOW},
            {OpCode.BGE, OpCategory.CONTROL_FLOW},
            {OpCode.CSR, OpCategory.CONTROL_FLOW},
            {OpCode.RSR, OpCategory.CONTROL_FLOW},

            {OpCode.SET, OpCategory.DATA_MANIPULATION},
            {OpCode.ADD, OpCategory.DATA_MANIPULATION},
            {OpCode.SUB, OpCategory.DATA_MANIPULATION},
            {OpCode.MUL, OpCategory.DATA_MANIPULATION},
            {OpCode.DIV, OpCategory.DATA_MANIPULATION},
            {OpCode.MOD, OpCategory.DATA_MANIPULATION},
            {OpCode.ABS, OpCategory.DATA_MANIPULATION},

            {OpCode.TAR, OpCategory.SENSING},
            {OpCode.HED, OpCategory.SENSING},
            {OpCode.SCN, OpCategory.SENSING},

            {OpCode.DRV, OpCategory.ACTION},
            {OpCode.TRN, OpCategory.ACTION},
            {OpCode.SHT, OpCategory.ACTION},
            {OpCode.SLP, OpCategory.ACTION},
        };

        /// OpCategory to OpCode array
        public static Dictionary<OpCategory, List<OpCode>> opCategoryOpCodesMap = new Dictionary<OpCategory, List<OpCode>>();

        /// OpCode string name to OpCode value map
        public static Dictionary<string, OpCode> nameOpMap = new Dictionary<string, OpCode>();

        static InstructionMaps()
        {
            for (OpCategory opCategory = 0; opCategory < OpCategory._SIZE; ++opCategory) {
                // Init opCategoryOpCodesMap
                opCategoryOpCodesMap[opCategory] = new List<OpCode>();
            }

            for (OpCode opCode = 0; opCode < OpCode._SIZE; ++opCode) {
                // Init nameOpMap
                nameOpMap.Add(opCode.ToString(), opCode);

                // Init opCategoryOpCodesMap
                opCategoryOpCodesMap[opCodeOpCategoryMap[opCode]].Add(opCode);
            }

            // Validate sizes of maps
            Debug.Assert(opArgSpecMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opCodeOpCategoryMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opCategoryOpCodesMap.Count == (int)OpCategory._SIZE);
        }
    }
}