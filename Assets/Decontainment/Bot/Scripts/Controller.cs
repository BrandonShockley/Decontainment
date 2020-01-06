using System;
using System.Collections.Generic;
using UnityEngine;
using Asm;

namespace Bot
{
    public class Controller : MonoBehaviour
    {
        public VirtualMachine vm;

        [SerializeField]
        private TextAsset code = null;
        [SerializeField]
        private float clockInterval = 1;

        private float clockTimer;
        private bool opRunning;

        private Driver driver;
        private Scanner scanner;
        private Shooter shooter;
        private Turner turner;
        private Health health;

        void Awake()
        {
            driver = GetComponent<Driver>();
            shooter = GetComponent<Shooter>();
            scanner = GetComponentInChildren<Scanner>();
            turner = GetComponent<Turner>();
            health = GetComponent<Health>();

            vm = new VirtualMachine(this);
            Instruction[] instructions = null;
            if (code == null) {
                Debug.LogWarning("No code provided. Using default program.");
                instructions = new Instruction[] { new Instruction(OpCode.NOP) };
            } else {
                instructions = Assembler.Assemble(code.text);
                if (instructions == null) {
                    Debug.LogWarning("Assembly failed. Using default program.");
                    instructions = new Instruction[] { new Instruction(OpCode.NOP) };
                }
            }
            vm.LoadProgram(instructions);

            health.OnDisable += HandleDisabled;

            clockTimer = clockInterval;
        }

        void FixedUpdate()
        {
            if (!opRunning) {
                clockTimer -= Time.fixedDeltaTime;
                if (clockTimer <= 0) {
                    clockTimer = clockInterval;
                    vm.Tick();
                }
            }
        }

        public void Drive(Driver.Direction direction, int distance, bool async)
        {
            driver.remainingDistance = distance;
            driver.direction = direction;
            if (!async) {
                opRunning = true;
                driver.onComplete = () => opRunning = false;
            }
        }

        public void Turn(Turner.Direction direction, int degrees, bool async)
        {
            turner.remainingDegrees = degrees;
            turner.direction = direction;
            if (!async) {
                opRunning = true;
                turner.onComplete = () => opRunning = false;
            }
        }

        public void Shoot(bool async)
        {
            shooter.shotRequested.Value = true;
            if (!async) {
                opRunning = true;
                shooter.onComplete = () => opRunning = false;
            }
        }

        public int Scan(Scanner.Target target, float direction, float range, float width)
        {
            return scanner.Scan(target, direction, range, width);
        }

        private void HandleDisabled()
        {
            enabled = false;
        }
    }
}