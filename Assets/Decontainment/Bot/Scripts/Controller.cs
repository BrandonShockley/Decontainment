using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Asm;

namespace Bot
{
    public class Controller : MonoBehaviour
    {
        private readonly Program FALLBACK_PROGRAM = new Program(){ name = "Fallback", instructions = new List<Instruction> { new Instruction(OpCode.NOP) }};

        public VirtualMachine vm;

        [SerializeField]
        private TextAsset code = null;
        [SerializeField]
        private float clockInterval = 1;
        [SerializeField]
        private Hardpoint[] hardpoints = null;
        [SerializeField]
        private ShooterConfigurations shooterConfigsTemplate = null;

        private float clockTimer;
        private ShooterConfigurations shooterConfigs;

        private Driver driver;
        private Scanner scanner;
        private Shooter[] shooters;
        private Turner turner;
        private Health health;

        void Awake()
        {
            driver = GetComponent<Driver>();
            scanner = GetComponentInChildren<Scanner>();
            turner = GetComponent<Turner>();
            health = GetComponent<Health>();
            shooters = GetComponents<Shooter>();

            shooterConfigs = shooterConfigsTemplate.Clone();

            for (int si = 0; si < shooterConfigs.Length; ++si) {
                ShooterConfigurations.Configuration config = shooterConfigs[si];
                shooters[si].Init(hardpoints[config.hardpointNum], config.weapon);
            }

            vm = new VirtualMachine(this);

            // Load the program
            if (code == null) {
                Debug.LogWarning("No code provided. Using fallback program.");
                vm.Program = FALLBACK_PROGRAM;
            } else {
                Program program = Assembler.Assemble(code.text);
                if (program == null) {
                    Debug.LogWarning("Assembly failed. Using fallback program.");
                    vm.Program = FALLBACK_PROGRAM;
                } else {
                    #if UNITY_EDITOR
                    program.name = AssetDatabase.GetAssetPath(code);
                    #else
                    program.name = code.name + ".txt";
                    #endif

                    // TODO: This is a temporary autosave solution; should be redone when editor is put into own menu
                    program.OnChange += () =>
                    {
                        string progText = Disassembler.Disassemble(vm.Program);
                        StreamWriter progFile = File.CreateText(vm.Program.name);
                        progFile.Write(progText);
                        progFile.Close();
                    };
                    vm.Program = program;
                }
            }



            health.OnDisable += HandleDisabled;

            clockTimer = clockInterval;
        }

        void FixedUpdate()
        {
            bool opRunning = driver.Running || turner.Running;
            foreach (Shooter shooter in shooters) {
                opRunning = opRunning || shooter.Running;
            }

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
            driver.async = async;
        }

        public void Turn(Turner.Direction direction, int degrees, bool async)
        {
            turner.remainingDegrees = degrees;
            turner.direction = direction;
            turner.async = async;
        }

        public void Shoot(int weaponNum, bool async)
        {
            shooters[weaponNum].shotRequested.Value = true;
            shooters[weaponNum].async = async;
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