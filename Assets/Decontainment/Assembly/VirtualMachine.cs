using Asm;
using Bot;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMachine
{
    public const int NUM_REGS = 5;
    public const int STACK_SIZE = 20;


    private Program program;
    private int tickCounter;
    private int sleepTickThreshold;
    private int[] regs = new int[NUM_REGS];
    private Stack<int> callStack = new Stack<int>(STACK_SIZE);

    private int pc;
    private Controller controller;

    public VirtualMachine(Controller controller)
    {
        this.controller = controller;
    }

    public event Action OnTick;

    public Program Program
    {
        get { return program; }
        set {
            if (program != null) {
                program.OnInstructionChange -= BoundPC;
            }
            program = value;
            program.OnInstructionChange += BoundPC;
        }
    }

    public int PC { get { return pc; } }

    /// Run the next instruction
    public void Tick()
    {
        if (tickCounter > sleepTickThreshold) {
            Instruction i = program.instructions[pc];
            int newPC = (pc + 1) % program.instructions.Count;
            switch(i.OpCode)
            {
                // Control flow
                case OpCode.NOP:
                    break;
                case OpCode.BUN:
                    newPC = GetArgValue(i.Args[0]);
                    break;
                case OpCode.BEQ:
                    if (GetArgValue(i.Args[1]) == GetArgValue(i.Args[2])) {
                        newPC = GetArgValue(i.Args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.BNE:
                    if (GetArgValue(i.Args[1]) != GetArgValue(i.Args[2])) {
                        newPC = GetArgValue(i.Args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.BLT:
                    if (GetArgValue(i.Args[1]) < GetArgValue(i.Args[2])) {
                        newPC = GetArgValue(i.Args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.BLE:
                    if (GetArgValue(i.Args[1]) <= GetArgValue(i.Args[2])) {
                        newPC = GetArgValue(i.Args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.BGT:
                    if (GetArgValue(i.Args[1]) > GetArgValue(i.Args[2])) {
                        newPC = GetArgValue(i.Args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.BGE:
                    if (GetArgValue(i.Args[1]) >= GetArgValue(i.Args[2])) {
                        newPC = GetArgValue(i.Args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.CSR:
                    if (callStack.Count > STACK_SIZE) {
                        Debug.LogWarning("Stack limit reached on CSR call");
                        break;
                    }
                    callStack.Push(newPC);
                    newPC = GetArgValue(i.Args[0]) % program.instructions.Count;
                    break;
                case OpCode.RSR:
                    if (callStack.Count == 0) {
                        Debug.LogWarning("Empty callstack on RSR call");
                        break;
                    }
                    newPC = callStack.Pop();
                    break;

                // Data manipulation
                case OpCode.SET:
                    regs[i.Args[0].val] = GetArgValue(i.Args[1]);
                    break;
                case OpCode.ADD:
                    regs[i.Args[0].val] = GetArgValue(i.Args[1]) + GetArgValue(i.Args[2]);
                    break;
                case OpCode.SUB:
                    regs[i.Args[0].val] = GetArgValue(i.Args[1]) - GetArgValue(i.Args[2]);
                    break;
                case OpCode.MUL:
                    regs[i.Args[0].val] = GetArgValue(i.Args[1]) * GetArgValue(i.Args[2]);
                    break;
                case OpCode.DIV:
                    regs[i.Args[0].val] = GetArgValue(i.Args[1]) / GetArgValue(i.Args[2]);
                    break;
                case OpCode.MOD:
                    regs[i.Args[0].val] = GetArgValue(i.Args[1]) % GetArgValue(i.Args[2]);
                    break;
                case OpCode.ABS:
                    regs[i.Args[0].val] = Math.Abs(GetArgValue(i.Args[1]));
                    break;

                // Sensing
                case OpCode.TAR:
                    regs[i.Args[0].val] = BotManager.Instance.FindTarget(controller,
                        (BotManager.DistanceType)GetArgValue(i.Args[1]),
                        (BotManager.TargetType)GetArgValue(i.Args[2]));
                    break;
                case OpCode.HED:
                    regs[i.Args[0].val] = BotManager.Instance.GetTargetHeading(controller, GetArgValue(i.Args[1]));
                    break;
                case OpCode.SCN:
                    regs[i.Args[0].val] = controller.Scan((Scanner.Target)GetArgValue(i.Args[1]),
                        GetArgValue(i.Args[2]), GetArgValue(i.Args[3]), GetArgValue(i.Args[4]));
                    break;

                // Actions
                case OpCode.DRV:
                    controller.Drive((Driver.Direction)GetArgValue(i.Args[0]),
                        GetArgValue(i.Args[1]),
                        GetArgValue(i.Args[2]) != 0);
                    break;
                case OpCode.TRN:
                    controller.Turn((Turner.Direction)GetArgValue(i.Args[0]),
                        GetArgValue(i.Args[1]),
                        GetArgValue(i.Args[2]) != 0);
                    break;
                case OpCode.SHT:
                    controller.Shoot(GetArgValue(i.Args[0]) == 1);
                    break;
                case OpCode.SLP:
                    sleepTickThreshold = tickCounter + GetArgValue(i.Args[0]);
                    break;
                default:
                    Debug.LogWarning("Unhandled OpCode " + i.OpCode.ToString());
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
                return arg.label.val;
            default:
                return 0;
        }
    }

    private void BoundPC()
    {
        pc = pc % program.instructions.Count;
    }
}