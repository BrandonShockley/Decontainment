using System.Collections.Generic;
using UnityEngine;

namespace Asm
{
    /// A collection of static lookup tables for instruction data
    public static class InstructionMaps
    {
        /// OpCode to descriptive string map
        public static Dictionary<OpCode, string> opDescriptiveNameMap = new Dictionary<OpCode, string>()
        {
            {OpCode.NOP, "No operation"},
            {OpCode.BUN, "Branch unconditionally"},
            {OpCode.BEQ, "Branch if equal"},
            {OpCode.BNE, "Branch if not equal"},
            {OpCode.BLT, "Branch if less than"},
            {OpCode.BLE, "Branch if less than or equal"},
            {OpCode.BGT, "Branch if greater than"},
            {OpCode.BGE, "Branch if greater than or equal"},
            {OpCode.CSR, "Call subroutine"},
            {OpCode.RSR, "Return from subroutine"},

            {OpCode.SET, "Set"},
            {OpCode.ADD, "Add"},
            {OpCode.SUB, "Subtract"},
            {OpCode.MUL, "Multiply"},
            {OpCode.DIV, "Divide"},
            {OpCode.MOD, "Modulo"},
            {OpCode.ABS, "Absolute value"},

            {OpCode.TAR, "Acquire target"},
            {OpCode.HED, "Acquire heading to target"},
            {OpCode.SCN, "Scan"},

            {OpCode.DRV, "Drive"},
            {OpCode.TRN, "Turn"},
            {OpCode.SHT, "Shoot"},
            {OpCode.SLP, "Sleep"},
        };

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
            Debug.Assert(opDescriptiveNameMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opArgSpecMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opCodeOpCategoryMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opCategoryOpCodesMap.Count == (int)OpCategory._SIZE);
        }
    }
}