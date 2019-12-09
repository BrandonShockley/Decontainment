using System;
using System.Collections.Generic;

namespace Asm
{
    public enum OpCode
    {
        // Control flow
        NOP,
        BEQ,
        BNE,
        BLT,
        BLE,
        BGT,
        BGE,

        // Data manipulation
        ADD,
        SUB,
        MUL,
        DIV,
        MOD,
        ABS,

        // Sensing
        // TODO: Make more documentation like this
        /// Acquire target
        /// TAR Rd n=[Near, Far]
        /// Stores nearest/farthest target index in Rd
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
        /// OpCode string name to OpCode value map
        public static Dictionary<string, OpCode> nameOpMap = new Dictionary<string, OpCode>();
        /// OpCode to argument number map
        public static Dictionary<OpCode, int> opArgNumMap = new Dictionary<OpCode, int>()
        {
            {OpCode.NOP, 0},
            {OpCode.BEQ, 3},
            {OpCode.BNE, 3},
            {OpCode.BLT, 3},
            {OpCode.BLE, 3},
            {OpCode.BGT, 3},
            {OpCode.BGE, 3},

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
        /// OpCode to immediate value argument validity array map
        public static Dictionary<OpCode, bool[]> opArgValidImmMap = new Dictionary<OpCode, bool[]>()
        {
            {OpCode.ADD, new bool[]{ false, true, true }},
            {OpCode.SUB, new bool[]{ false, true, true }},
            {OpCode.MUL, new bool[]{ false, true, true }},
            {OpCode.DIV, new bool[]{ false, true, true }},
            {OpCode.MOD, new bool[]{ false, true, true }},
            {OpCode.ABS, new bool[]{ false, true }},

            {OpCode.TAR, new bool[]{ false, true }},
            {OpCode.HED, new bool[]{ false, true }},
            {OpCode.SCN, new bool[]{ false, true, true, true, true }},
        };

        static InstructionMaps()
        {
            for (int i = 0; i < (int)OpCode._SIZE; ++i) {
                OpCode opCode = (OpCode)i;
                // Init nameOpMap
                nameOpMap.Add(opCode.ToString(), opCode);

                // Init any leftover entries in opArgValidImmMap
                // Defaults to all valid
                if (!opArgValidImmMap.ContainsKey(opCode)) {
                    bool[] validArgs = new bool[opArgNumMap[opCode]];
                    for (int b = 0; b < validArgs.Length; ++b) {
                        validArgs[b] = true;
                    }
                    opArgValidImmMap.Add(opCode, validArgs);
                }
            }
        }
    }
}