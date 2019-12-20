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
        BEQ,
        BNE,
        BLT,
        BLE,
        BGT,
        BGE,

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

    public struct ArgumentMeta
    {
        public static readonly ArgumentMeta OPEN = new ArgumentMeta(false, null);
        public static readonly ArgumentMeta REG_ONLY = new ArgumentMeta(true, null);
        public static readonly ArgumentMeta SYNC_MACROS = new ArgumentMeta(false, new string[]{ "Sync", "Async" });

        public bool regOnly;
        /// Array of built-in macros
        /// Only valid if regOnly == false
        public string[] macros;
        public ArgumentMeta(bool regOnly, string[] macros)
        {
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

        /// OpCode to argument metadata array map
        public static Dictionary<OpCode, ArgumentMeta[]> opArgMetaMap = new Dictionary<OpCode, ArgumentMeta[]>()
        {
            {OpCode.SET, new ArgumentMeta[]{ ArgumentMeta.REG_ONLY, ArgumentMeta.OPEN }},
            {OpCode.ADD, new ArgumentMeta[]{ ArgumentMeta.REG_ONLY, ArgumentMeta.OPEN, ArgumentMeta.OPEN }},
            {OpCode.SUB, new ArgumentMeta[]{ ArgumentMeta.REG_ONLY, ArgumentMeta.OPEN, ArgumentMeta.OPEN }},
            {OpCode.MUL, new ArgumentMeta[]{ ArgumentMeta.REG_ONLY, ArgumentMeta.OPEN, ArgumentMeta.OPEN }},
            {OpCode.DIV, new ArgumentMeta[]{ ArgumentMeta.REG_ONLY, ArgumentMeta.OPEN, ArgumentMeta.OPEN }},
            {OpCode.MOD, new ArgumentMeta[]{ ArgumentMeta.REG_ONLY, ArgumentMeta.OPEN, ArgumentMeta.OPEN }},
            {OpCode.ABS, new ArgumentMeta[]{ ArgumentMeta.REG_ONLY, ArgumentMeta.OPEN }},

            {OpCode.TAR, new ArgumentMeta[]{ ArgumentMeta.REG_ONLY,
                new ArgumentMeta(false, new string[]{ "Nearest", "Farthest" }) }},
            {OpCode.HED, new ArgumentMeta[]{ ArgumentMeta.REG_ONLY, ArgumentMeta.OPEN }},
            {OpCode.SCN, new ArgumentMeta[]{ ArgumentMeta.REG_ONLY,
                new ArgumentMeta(false, new string[]{ "Projectiles", "Obstacles", "Allies", "Enemies" }),
                ArgumentMeta.OPEN, ArgumentMeta.OPEN, ArgumentMeta.OPEN }},

            {OpCode.DRV, new ArgumentMeta[]{
                new ArgumentMeta(false, new string[]{ "Forward", "Backward", "Left", "Right"}),
                ArgumentMeta.OPEN, ArgumentMeta.SYNC_MACROS }},
            {OpCode.TRN, new ArgumentMeta[]{
                new ArgumentMeta(false, new string[]{ "Left", "Right" }),
                ArgumentMeta.OPEN, ArgumentMeta.SYNC_MACROS }},
            {OpCode.SHT, new ArgumentMeta[]{ ArgumentMeta.SYNC_MACROS }},
            {OpCode.SLP, new ArgumentMeta[]{ ArgumentMeta.OPEN }},

        };

        /// OpCode to OpCategory map
        public static Dictionary<OpCode, OpCategory> opCategoryMap = new Dictionary<OpCode, OpCategory>()
        {
            {OpCode.NOP, OpCategory.CONTROL_FLOW},
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

                // Init any leftover entries in opArgValidImmMap
                // Defaults to immediate allowed with no defined macros
                if (!opArgMetaMap.ContainsKey(opCode)) {
                    ArgumentMeta[] argMetas = new ArgumentMeta[opArgNumMap[opCode]];
                    for (int b = 0; b < argMetas.Length; ++b) {
                        argMetas[b] = ArgumentMeta.OPEN;
                    }
                    opArgMetaMap.Add(opCode, argMetas);
                }

                // Validate opArgMetaMap is filled out
                int numArgs = opArgNumMap[opCode];
                Debug.Assert(opArgMetaMap[opCode].Length == numArgs);
            }

            // Validate sizes of maps
            Debug.Assert(opArgNumMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opArgMetaMap.Count == (int)OpCode._SIZE);
            Debug.Assert(opCategoryMap.Count == (int)OpCode._SIZE);
        }
    }
}