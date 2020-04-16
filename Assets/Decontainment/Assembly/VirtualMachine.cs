using Asm;
using Bot;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMachine
{
    public const int NUM_LOCAL_REGS = 5;
    public const int NUM_SHARED_REGS = 5;
    public const int NUM_TOTAL_REGS = NUM_LOCAL_REGS + NUM_SHARED_REGS;
    public const int STACK_SIZE = 20;

    private static readonly Instruction NOP = new Instruction(OpCode.NOP);

    private Program program;
    private int tickCounter;
    private int sleepTickThreshold;
    private int[] regs = new int[NUM_TOTAL_REGS];
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
                program.OnChange -= HandleProgramChange;
            }
            program = value;
            program.OnChange += HandleProgramChange;
        }
    }

    public int PC { get { return pc; } }

    /// Run the next instruction
    public void Tick()
    {
        if (tickCounter > sleepTickThreshold) {
            Instruction i;
            int newPC;
            if (program == null || program.instructions.Count == 0) {
                i = NOP;
                newPC = 0;
            } else {
                i = program.instructions[pc];
                newPC = (pc + 1) % program.instructions.Count;
            }

            switch(i.opCode)
            {
                // Control flow
                case OpCode.NOP:
                    break;
                case OpCode.BUN:
                    newPC = GetArgValue(i.args[0]) % program.instructions.Count;
                    break;
                case OpCode.BEQ:
                    if (GetArgValue(i.args[1]) == GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.BNE:
                    if (GetArgValue(i.args[1]) != GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.BLT:
                    if (GetArgValue(i.args[1]) < GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.BLE:
                    if (GetArgValue(i.args[1]) <= GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.BGT:
                    if (GetArgValue(i.args[1]) > GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.BGE:
                    if (GetArgValue(i.args[1]) >= GetArgValue(i.args[2])) {
                        newPC = GetArgValue(i.args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.BRN:
                    if (GetArgValue(i.args[1]) > UnityEngine.Random.Range(0, 100)) {
                        newPC = GetArgValue(i.args[0]) % program.instructions.Count;
                    }
                    break;
                case OpCode.CSR:
                    if (callStack.Count > STACK_SIZE) {
                        Debug.LogWarning("Stack limit reached on CSR call");
                        break;
                    }
                    callStack.Push(newPC);
                    newPC = GetArgValue(i.args[0]) % program.instructions.Count;
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
                    regs[i.args[0].val] = GetArgValue(i.args[1]);
                    if (i.args[0].val > NUM_LOCAL_REGS) { BotManager.Instance.PropagateRegister(i.args[0].val, regs[i.args[0].val], controller.TeamID); }
                    break;
                case OpCode.ADD:
                    regs[i.args[0].val] = GetArgValue(i.args[1]) + GetArgValue(i.args[2]);
                    if (i.args[0].val > NUM_LOCAL_REGS) { BotManager.Instance.PropagateRegister(i.args[0].val, regs[i.args[0].val], controller.TeamID); }
                    break;
                case OpCode.SUB:
                    regs[i.args[0].val] = GetArgValue(i.args[1]) - GetArgValue(i.args[2]);
                    if (i.args[0].val > NUM_LOCAL_REGS) { BotManager.Instance.PropagateRegister(i.args[0].val, regs[i.args[0].val], controller.TeamID); }
                    break;
                case OpCode.MUL:
                    regs[i.args[0].val] = GetArgValue(i.args[1]) * GetArgValue(i.args[2]);
                    if (i.args[0].val > NUM_LOCAL_REGS) { BotManager.Instance.PropagateRegister(i.args[0].val, regs[i.args[0].val], controller.TeamID); }
                    break;
                case OpCode.DIV:
                    regs[i.args[0].val] = GetArgValue(i.args[1]) / GetArgValue(i.args[2]);
                    if (i.args[0].val > NUM_LOCAL_REGS) { BotManager.Instance.PropagateRegister(i.args[0].val, regs[i.args[0].val], controller.TeamID); }
                    break;
                case OpCode.MOD:
                    regs[i.args[0].val] = GetArgValue(i.args[1]) % GetArgValue(i.args[2]);
                    if (i.args[0].val > NUM_LOCAL_REGS) { BotManager.Instance.PropagateRegister(i.args[0].val, regs[i.args[0].val], controller.TeamID); }
                    break;
                case OpCode.ABS:
                    regs[i.args[0].val] = Math.Abs(GetArgValue(i.args[1]));
                    if (i.args[0].val > NUM_LOCAL_REGS) { BotManager.Instance.PropagateRegister(i.args[0].val, regs[i.args[0].val], controller.TeamID); }
                    break;
                case OpCode.RNG:
                    regs[i.args[0].val] = UnityEngine.Random.Range(0, GetArgValue(i.args[1]));
                    if (i.args[0].val > NUM_LOCAL_REGS) { BotManager.Instance.PropagateRegister(i.args[0].val, regs[i.args[0].val], controller.TeamID); }
                    break;

                // Sensing
                case OpCode.TAR:
                    regs[i.args[0].val] = BotManager.Instance.FindTarget(controller,
                        (BotManager.DistanceType)GetArgValue(i.args[1]),
                        (BotManager.TargetType)GetArgValue(i.args[2]));
                    break;
                case OpCode.HED:
                    regs[i.args[0].val] = BotManager.Instance.GetTargetHeading(controller, GetArgValue(i.args[1]));
                    break;
                case OpCode.DIS:
                    regs[i.args[0].val] = BotManager.Instance.GetTargetDistance(controller, GetArgValue(i.args[1]));
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

    public void UpdateSharedReg(int registerNumber, int registerValue) {
        regs[registerNumber] = registerValue;
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

    private void HandleProgramChange(Program.Change change)
    {
        if (change.instruction) {
            BoundPC();
        }
    }

    private void BoundPC()
    {
        pc = pc % program.instructions.Count;
    }
}