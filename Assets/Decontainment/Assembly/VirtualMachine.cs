using Asm;
using Bot;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMachine
{
    public static readonly int NUM_REGS = 5;

    public int pc;
    public Program program;

    private int tickCounter;
    private int sleepTickThreshold;
    private int[] regs = new int[NUM_REGS];

    private Controller controller;

    public VirtualMachine(Controller controller)
    {
        this.controller = controller;
    }

    public void LoadProgram(Program program)
    {
        this.program = program;
    }

    public event Action OnTick;

    /// Run the next instruction
    public void Tick()
    {
        if (tickCounter > sleepTickThreshold) {
            Instruction i = program.instructions[pc];
            int newPC = (pc + 1) % program.instructions.Length;
            switch(i.opCode)
            {
                // Control flow
                case OpCode.NOP:
                    break;
                case OpCode.BUN:
                    newPC = GetArgValue(i.args[0]);
                    break;
                case OpCode.BEQ:
                    if (GetArgValue(i.args[1]) == GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]);
                    }
                    break;
                case OpCode.BNE:
                    if (GetArgValue(i.args[1]) != GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]);
                    }
                    break;
                case OpCode.BLT:
                    if (GetArgValue(i.args[1]) < GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]);
                    }
                    break;
                case OpCode.BLE:
                    if (GetArgValue(i.args[1]) <= GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]);
                    }
                    break;
                case OpCode.BGT:
                    if (GetArgValue(i.args[1]) > GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]);
                    }
                    break;
                case OpCode.BGE:
                    if (GetArgValue(i.args[1]) >= GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]);
                    }
                    break;

                // Data manipulation
                case OpCode.SET:
                    regs[i.args[0].val] = GetArgValue(i.args[1]);
                    break;
                case OpCode.ADD:
                    regs[i.args[0].val] = GetArgValue(i.args[1]) + GetArgValue(i.args[2]);
                    break;
                case OpCode.SUB:
                    regs[i.args[0].val] = GetArgValue(i.args[1]) - GetArgValue(i.args[2]);
                    break;
                case OpCode.MUL:
                    regs[i.args[0].val] = GetArgValue(i.args[1]) * GetArgValue(i.args[2]);
                    break;
                case OpCode.DIV:
                    regs[i.args[0].val] = GetArgValue(i.args[1]) / GetArgValue(i.args[2]);
                    break;
                case OpCode.MOD:
                    regs[i.args[0].val] = GetArgValue(i.args[1]) % GetArgValue(i.args[2]);
                    break;
                case OpCode.ABS:
                    regs[i.args[0].val] = Math.Abs(GetArgValue(i.args[1]));
                    break;

                // Sensing
                case OpCode.TAR:
                    regs[i.args[0].val] = BotManager.Instance.FindTarget(controller,
                        (BotManager.DistanceType)GetArgValue(i.args[1]));
                    break;
                case OpCode.HED:
                    regs[i.args[0].val] = BotManager.Instance.GetTargetHeading(controller, GetArgValue(i.args[1]));
                    break;
                case OpCode.SCN:
                    regs[i.args[0].val] = controller.Scan((Scanner.Target)GetArgValue(i.args[1]),
                        GetArgValue(i.args[2]), GetArgValue(i.args[3]), GetArgValue(i.args[4]));
                    break;

                // Actions
                case OpCode.DRV:
                    controller.Drive((Driver.Direction)GetArgValue(i.args[0]),
                        GetArgValue(i.args[1]),
                        GetArgValue(i.args[2]) != 0);
                    break;
                case OpCode.TRN:
                    controller.Turn((Turner.Direction)GetArgValue(i.args[0]),
                        GetArgValue(i.args[1]),
                        GetArgValue(i.args[2]) != 0);
                    break;
                case OpCode.SHT:
                    controller.Shoot(GetArgValue(i.args[0]) == 1);
                    break;
                case OpCode.SLP:
                    sleepTickThreshold = tickCounter + GetArgValue(i.args[0]);
                    break;
                default:
                    Debug.LogWarning("Unhandled OpCode " + i.opCode.ToString());
                    break;
            }
            pc = newPC;
        }
        ++tickCounter;
        OnTick?.Invoke();
    }

    private int GetArgValue(Argument arg)
    {
        switch (arg.type)
        {
            case Argument.Type.IMMEDIATE:
                return arg.val;
            case Argument.Type.REGISTER:
                return regs[arg.val];
            case Argument.Type.LABEL:
                return arg.val;
            default:
                return 0;
        }
    }
}