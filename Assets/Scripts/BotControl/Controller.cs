using System;
using System.Collections.Generic;
using UnityEngine;
using Asm;

namespace Bot
{
    public class Controller : MonoBehaviour
    {
        public float moveSpeed = 1;
        public float turnSpeed = 45;

        [SerializeField]
        private TextAsset code = null;
        [SerializeField]
        private float clockInterval = 1;
        [SerializeField]
        private GameObject projectilePrefab = null;
        [SerializeField]
        private AudioClip projectileClip = null;

        private VirtualMachine vm;
        private Driving driving;
        private Turning turning;
        private float clockTimer;
        private bool opRunning;

        private Scanner scan;
        private Health health;
        private SoundModulator sm;

        void Awake()
        {
            scan = GetComponentInChildren<Scanner>();
            health = GetComponent<Health>();
            sm = GetComponent<SoundModulator>();

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

            driving = new Driving(this);
            turning = new Turning(this);
            clockTimer = clockInterval;
        }

        void FixedUpdate()
        {
            if (!health.Disabled) {
                if (!opRunning) {
                    clockTimer -= Time.deltaTime;
                    if (clockTimer <= 0) {
                        clockTimer = clockInterval;
                        vm.Tick();
                    }
                }

                driving.Update();
                turning.Update();
            }
        }

        public void Drive(Driving.Direction direction, int distance, bool async)
        {
            driving.remainingDistance = distance;
            driving.direction = direction;
            if (!async) {
                opRunning = true;
                driving.onComplete = () => opRunning = false;
            }
        }

        public void Turn(Turning.Direction direction, int degrees, bool async)
        {
            turning.remainingDegrees = degrees;
            turning.direction = direction;
            if (!async) {
                opRunning = true;
                turning.onComplete = () => opRunning = false;
            }
        }

        public void Shoot()
        {
            Projectile.CreateProjectile(this, projectilePrefab, transform.position, transform.right);
            sm.PlayModClip(projectileClip);
        }

        public int Scan(Scanner.Target target, float direction, float range, float width)
        {
            return scan.Scan(target, direction, range, width);
        }
    }
}