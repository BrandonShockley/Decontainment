using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asm
{
    public enum OpCategory
    {
        CONTROL_FLOW,
        DATA_MANIP,
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

        // Data manipulation
        SET,
        ADD,
        SUB,
        MUL,
        DIV,
        MOD,
        ABS,

        // Sensing
        TAR,
        HED,
        SCN,

        // Actions
        DRV,
        TRN,
        SHT,
        SLP,
        _SIZE,
    }

    public struct Argument
    {
        public int val;
        public bool isReg;
        public Argument(int val, bool isReg = false)
        {
            this.val = val;
            this.isReg = isReg;
        }
    }

    public struct ArgumentSpec
    {
        public static readonly ArgumentSpec BRANCH_LABEL = ArgumentSpec.MakeOpen("Branch To");
        public static readonly ArgumentSpec DEST_REG = ArgumentSpec.MakeRegOnly("Result");
        public static readonly ArgumentSpec VAL = ArgumentSpec.MakeOpen("Value");
        public static readonly ArgumentSpec VAL1 = ArgumentSpec.MakeOpen("Value 1");
        public static readonly ArgumentSpec VAL2 = ArgumentSpec.MakeOpen("Value 2");
        public static readonly ArgumentSpec SYNC_MACROS = new ArgumentSpec("Concurrent", false, new string[]{ "Sync", "Async" });

        public static readonly ArgumentSpec[] TWO_INPUT_CONTROL_FLOW_SPECS = new ArgumentSpec[]
        {
            ArgumentSpec.BRANCH_LABEL,
            ArgumentSpec.VAL1,
            ArgumentSpec.VAL2
        };
        public static readonly ArgumentSpec[] ONE_INPUT_DATA_MANIP_SPECS = new ArgumentSpec[]
        {
            DEST_REG,
            ArgumentSpec.VAL
        };
        public static readonly ArgumentSpec[] TWO_INPUT_DATA_MANIP_SPECS = new ArgumentSpec[]
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
        /// Array of built-in macros
        /// Only valid if regOnly == false
        public string[] macros;
        public ArgumentSpec(string name, bool regOnly, string[] macros)
        {
            this.name = name;
            this.regOnly = regOnly;
            this.macros = macros;
        }
    }

    public class Instruction
    {
        public OpCode opCode;
        public Argument[] args;
        public Instruction(OpCode opCode, params Argument[] args)
        {
            this.opCode = opCode;
            this.args = new Argument[InstructionMaps.opArgNumMap[opCode]];
            Array.Copy(args, this.args, Math.Min(this.args.Length, args.Length));
        }
    }

    public static class InstructionMaps
    {

        /// OpCode to argument number map
        public static Dictionary<OpCode, int> opArgNumMap = new Dictionary<OpCode, int>()
        {
            {OpCode.NOP, 0},
            {OpCode.BUN, 1},
            {OpCode.BEQ, 3},
            {OpCode.BNE, 3},
            {OpCode.BLT, 3},
            {OpCode.BLE, 3},
            {OpCode.BGT, 3},
            {OpCode.BGE, 3},

            {OpCode.SET, 2},
            {OpCode.ADD, 3},
            {OpCode.SUB, 3},
            {OpCode.MUL, 3},
            {OpCode.DIV, 3},
            {OpCode.MOD, 3},
            {OpCode.ABS, 2},

            {OpCode.TAR, 2},
            {OpCode.HED, 2},
            {OpCode.SCN, 5},

            {OpCode.DRV, 3},
            {OpCode.TRN, 3},
            {OpCode.SHT, 1},
            {OpCode.SLP, 1},
        };


        /// OpCode to argument specification array map
        public static Dictionary<OpCode, ArgumentSpec[]> opArgSpecMap = new Dictionary<OpCode, ArgumentSpec[]>()
        {
            {OpCode.NOP, new ArgumentSpec[0]},
            {OpCode.BUN, new ArgumentSpec[]{ ArgumentSpec.BRANCH_LABEL }},
            {OpCode.BEQ, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.BNE, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.BLT, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.BLE, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.BGT, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},
            {OpCode.BGE, ArgumentSpec.TWO_INPUT_CONTROL_FLOW_SPECS},

            {OpCode.SET, ArgumentSpec.ONE_INPUT_DATA_MANIP_SPECS},
            {OpCode.ADD, ArgumentSpec.TWO_INPUT_DATA_MANIP_SPECS},
            {OpCode.SUB, ArgumentSpec.TWO_INPUT_DATA_MANIP_SPECS},
            {OpCode.MUL, ArgumentSpec.TWO_INPUT_DATA_MANIP_SPECS},
            {OpCode.DIV, ArgumentSpec.TWO_INPUT_DATA_MANIP_SPECS},
            {OpCode.MOD, ArgumentSpec.TWO_INPUT_DATA_MANIP_SPECS},
            {OpCode.ABS, ArgumentSpec.ONE_INPUT_DATA_MANIP_SPECS},

            {OpCode.TAR, new ArgumentSpec[]
                {
                    ArgumentSpec.DEST_REG,
                    new ArgumentSpec("Filter", false, new string[]{ "Nearest", "Farthest" })
                }
            },
            {OpCode.HED, new ArgumentSpec[]
                {
                    ArgumentSpec.DEST_REG,
                    ArgumentSpec.MakeOpen("Target Index")
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
                    ArgumentSpec.SYNC_MACROS
                }
            },
            {OpCode.TRN, new ArgumentSpec[]
                {
                    new ArgumentSpec("Direction", false, new string[]{ "Left", "Right" }),
                    ArgumentSpec.MakeOpen("Degrees"),
                    ArgumentSpec.SYNC_MACROS
                }
            },
            {OpCode.SHT, new ArgumentSpec[]{ ArgumentSpec.SYNC_MACROS }},
            {OpCode.SLP, new ArgumentSpec[]{ ArgumentSpec.MakeOpen("Duration") }},

        };

        /// OpCode to OpCategory map
        public static Dictionary<OpCode, OpCategory> opCategoryMap = new Dictionary<OpCode, OpCategory>()
        {
            {OpCode.NOP, OpCategory.CONTROL_FLOW},
            {OpCode.BUN, OpCategory.CONTROL_FLOW},
            {OpCode.BEQ, OpCategory.CONTROL_FLOW},
            {OpCode.BNE, OpCategory.CONTROL_FLOW},
            {OpCode.BLT, OpCategory.CONTROL_FLOW},
            {OpCode.BLE, OpCategory.CONTROL_FLOW},
            {OpCode.BGT, OpCategory.CONTROL_FLOW},
            {OpCode.BGE, OpCategory.CONTROL_FLOW},

            {OpCode.SET, OpCategory.DATA_MANIP},
            {OpCode.ADD, OpCategory.DATA_MANIP},
            {OpCode.SUB, OpCategory.DATA_MANIP},
            {OpCode.MUL, OpCategory.DATA_MANIP},
            {OpCode.DIV, OpCategory.DATA_MANIP},
            {OpCode.MOD, OpCategory.DATA_MANIP},
            {OpCode.ABS, OpCategory.DATA_MANIP},

            {OpCode.TAR, OpCategory.SENSING},
            {OpCode.HED, OpCategory.SENSING},
            {OpCode.SCN, OpCategory.SENSING},

            {OpCode.DRV, OpCategory.ACTION},
            {OpCode.TRN, OpCategory.ACTION},
            {OpCode.SHT, OpCategory.ACTION},
            {OpCode.SLP, OpCategory.ACTION},
        };

        /// OpCode string name to OpCode value map
        public static Dictionary<string, OpCode> nameOpMap = new Dictionary<string, OpCode>();

        static InstructionMaps()
        {
            for (int i = 0; i < (int)OpCode._SIZE; ++i) {
                OpCode opCode = (OpCode)i;
                // Init nameOpMap
                nameOpMap.Add(opCode.ToString(), opCode);

                // Validate opArgSpecMap is filled out
                int numArgs = opArgNumMap[opCode];
                Debug.Assert(opArgSpecMap[opCode].Length == numArgs);
            }

            // Validate sizes of maps
            Debug.Assert(opArgNumMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opArgSpecMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opCategoryMap.Count == (int)OpCode._SIZE);
        }
    }
}