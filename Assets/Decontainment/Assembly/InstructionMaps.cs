using System.Collections.Generic;
using UnityEngine;

namespace Asm
{
    /// A collection of static lookup tables for instruction data
    public static class InstructionMaps
    {
        /// OpCode to descriptive string map
        public static Dictionary<OpCode, OpInfo> opInfoMap = new Dictionary<OpCode, OpInfo>()
        {
            {OpCode.NOP, new OpInfo("No operation", "Does nothing for a cycle.")},
            {OpCode.BUN, new OpInfo("Branch unconditionally", "Always branch to the given destination.")},
            {OpCode.BEQ, new OpInfo("Branch if equal", "Branch to the given destination if the two values are equal.")},
            {OpCode.BNE, new OpInfo("Branch if not equal", "Branch to the given destination if the two values are not equal.")},
            {OpCode.BLT, new OpInfo("Branch if less than", "Branch to the given destination if the left value is less than the right value.")},
            {OpCode.BLE, new OpInfo("Branch if less than or equal", "Branch to the given destination if the left value is less than or equal to the right value.")},
            {OpCode.BGT, new OpInfo("Branch if greater than", "Branch to the given destination if the left value is greater than the right value.")},
            {OpCode.BGE, new OpInfo("Branch if greater than or equal", "Branch to the given destination if the left value is greater than or equal to the right value.")},
            {OpCode.BRN, new OpInfo("Branch randomly", "Branch to the given destination with the specified chance.")},
            {OpCode.CSR, new OpInfo("Call subroutine", "Push the current program counter onto the callstack and branch to the given destination.")},
            {OpCode.RSR, new OpInfo("Return from subroutine", "Pop the top program counter value from the callstack and branch to it.")},

            {OpCode.SET, new OpInfo("Set", "Set the result register to the given value.")},
            {OpCode.ADD, new OpInfo("Add", "Set the result register to the sum of the two given values.")},
            {OpCode.SUB, new OpInfo("Subtract", "Set the result register to the difference of the two given values.")},
            {OpCode.MUL, new OpInfo("Multiply", "Set the result register to the product of the two given values.")},
            {OpCode.DIV, new OpInfo("Divide", "Set the result register to the quotient of the two given values.")},
            {OpCode.MOD, new OpInfo("Modulo", "Set the result register to the remainder of division of the two given values.")},
            {OpCode.ABS, new OpInfo("Absolute value", "Set the result register to the absolute value of the given value.")},
            {OpCode.RNG, new OpInfo("Generate random number", "Set the result register to a random number between 0 and the given value, non-inclusive.")},

            {OpCode.TAR, new OpInfo("Acquire target", "Choose a target based on the given filters and save a handle to it in the result register.")},
            {OpCode.HED, new OpInfo("Acquire heading to target", "Set the result register to the degrees left required to turn to face the given target.")},
            {OpCode.DIS, new OpInfo("Acquire distance to target", "Set the result register to the distance between the bot and the given target.")},
            {OpCode.SCN, new OpInfo("Scan", "Scan in a cone for the given object type and set the result register to the number of objects found.")},

            {OpCode.DRV, new OpInfo("Drive", "Translate the bot.")},
            {OpCode.TRN, new OpInfo("Turn", "Turn the bot.")},
            {OpCode.SHT, new OpInfo("Shoot", "Fire the bot's weapon.")},
            {OpCode.SLP, new OpInfo("Sleep", "Sleep for the given number of cycles.")},
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
            {OpCode.BRN, new ArgumentSpec[]
                {
                    ArgumentSpec.BRANCH_LABEL,
                    ArgumentSpec.MakeOpen("Branch Chance", "In the range of 0 to 100, how likely it is that the branch is taken.")
                }
            },
            {OpCode.CSR, new ArgumentSpec[]
                {
                    ArgumentSpec.MakeOpen("Subroutine Destination", "Where to jump to after saving the current program counter."),
                }
            },
            {OpCode.RSR, new ArgumentSpec[0]},

            {OpCode.SET, ArgumentSpec.ONE_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.ADD, ArgumentSpec.TWO_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.SUB, ArgumentSpec.TWO_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.MUL, ArgumentSpec.TWO_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.DIV, ArgumentSpec.TWO_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.MOD, ArgumentSpec.TWO_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.ABS, ArgumentSpec.ONE_INPUT_DATA_MANIPULATION_SPECS},
            {OpCode.RNG, new ArgumentSpec[]
                {
                    ArgumentSpec.DEST_REG,
                    new ArgumentSpec("Max", false, null, "Maximum value of generated random number, non-inclusive.")
                }
            },

            {OpCode.TAR, new ArgumentSpec[]
                {
                    ArgumentSpec.DEST_REG,
                    new ArgumentSpec("Distance Filter", false, new string[]{ "Nearest", "Farthest" }, ""),
                    new ArgumentSpec("Type Filter", false, new string[]{ "Ally", "Enemy" }, "Which allegiance to target.")
                }
            },
            {OpCode.HED, ArgumentSpec.HANDLING_TARGET_SENSING_SPECS},
            {OpCode.DIS, ArgumentSpec.HANDLING_TARGET_SENSING_SPECS},
            {OpCode.SCN, new ArgumentSpec[]
                {
                    ArgumentSpec.DEST_REG,
                    new ArgumentSpec("Type", false, new string[]{ "Projectiles", "Obstacles", "Allies", "Enemies" }, "What to scan for."),
                    ArgumentSpec.MakeOpen("Radial Offset", "How many degrees left of center to start the scan at."),
                    ArgumentSpec.MakeOpen("Radial Width", "How many degrees to scan outward from the radial offset."),
                    ArgumentSpec.MakeOpen("Distance", "How far to scan away from the bot. Units are approximately the length of a bot.")
                }
            },

            {OpCode.DRV, new ArgumentSpec[]
                {
                    new ArgumentSpec("Direction", false, new string[]{ "Forward", "Backward", "Left", "Right" }, "The direction to drive."),
                    ArgumentSpec.MakeOpen("Distance", "How far to drive. Units are approximately the length of a bot."),
                    ArgumentSpec.MakeConcurrency(
                        "Sync – Wait for move to complete before continuing execution\n" +
                        "Async – Continue executing instructions during the move")
                }
            },
            {OpCode.TRN, new ArgumentSpec[]
                {
                    new ArgumentSpec("Direction", false, new string[]{ "Left", "Right" }, "The direction to turn."),
                    ArgumentSpec.MakeOpen("Degrees", "How many degrees to turn."),
                    ArgumentSpec.MakeConcurrency(
                        "Sync – Wait for turn to complete before continuing execution\n" +
                        "Async – Continue executing instructions during the turn")
                }
            },
            {OpCode.SHT, new ArgumentSpec[]
                {
                    ArgumentSpec.MakeConcurrency(
                        "Sync – Wait to shoot before continuing execution\n" +
                        "Async – Continue executing instructions while waiting to shoot")
                }
            },
            {OpCode.SLP, new ArgumentSpec[]{ ArgumentSpec.MakeOpen("Duration", "How long to sleep in cycles. Each cycle is 20 milliseconds.") }},

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
            {OpCode.BRN, OpCategory.CONTROL_FLOW},
            {OpCode.CSR, OpCategory.CONTROL_FLOW},
            {OpCode.RSR, OpCategory.CONTROL_FLOW},

            {OpCode.SET, OpCategory.DATA_MANIPULATION},
            {OpCode.ADD, OpCategory.DATA_MANIPULATION},
            {OpCode.SUB, OpCategory.DATA_MANIPULATION},
            {OpCode.MUL, OpCategory.DATA_MANIPULATION},
            {OpCode.DIV, OpCategory.DATA_MANIPULATION},
            {OpCode.MOD, OpCategory.DATA_MANIPULATION},
            {OpCode.ABS, OpCategory.DATA_MANIPULATION},
            {OpCode.RNG, OpCategory.DATA_MANIPULATION},

            {OpCode.TAR, OpCategory.SENSING},
            {OpCode.HED, OpCategory.SENSING},
            {OpCode.DIS, OpCategory.SENSING},
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
            Debug.Assert(opInfoMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opArgSpecMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opCodeOpCategoryMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opCategoryOpCodesMap.Count == (int)OpCategory._SIZE);
        }
    }
}