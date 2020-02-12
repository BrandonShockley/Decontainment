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
        [SerializeField]
        private Color[] teamColors = new Color[]
        {
            Color.cyan,
            Color.red,
        };
        [SerializeField]
        private Hardpoint hardpoint = null;
        [SerializeField]
        private BotData data = null;
        [SerializeField]
        private float clockInterval = 1;
        /// 0 for player, 1 for enemy
        [SerializeField]
        private int teamID = 0;

        private float clockTimer;

        private VirtualMachine vm;
        private Driver driver;
        private Scanner scanner;
        private Shooter shooter;
        private Turner turner;
        private Health health;
        private SpriteRenderer sr;

        public int TeamID { get { return teamID; } }
        public VirtualMachine VM { get { return vm; } }
        public Health Health { get { return health; } }

        void Awake()
        {
            driver = GetComponent<Driver>();
            scanner = GetComponentInChildren<Scanner>();
            turner = GetComponent<Turner>();
            health = GetComponent<Health>();
            shooter = GetComponent<Shooter>();
            sr = GetComponent<SpriteRenderer>();

            vm = new VirtualMachine(this);

            health.OnDisable += HandleDisabled;

            clockTimer = clockInterval;
        }

        void Start()
        {
            shooter.Init(hardpoint, data.WeaponData);
            sr.color = teamColors[teamID];
            vm.Program = data.Program;
        }

        void FixedUpdate()
        {
            bool opRunning = driver.Running || turner.Running || shooter.Running;

            if (!opRunning) {
                clockTimer -= Time.fixedDeltaTime;
                if (clockTimer <= 0) {
                    clockTimer = clockInterval;
                    vm.Tick();
                }
            }
        }

        public static void CreateBot(GameObject prefab, BotData data, int teamID, Vector2 position, Vector2 look)
        {
            GameObject go = GameObject.Instantiate(prefab, position, Quaternion.identity);
            Controller controller = go.GetComponent<Controller>();
            controller.Init(data, teamID);
            go.transform.right = look;

            BotManager.Instance.AddBot(controller);
        }

        public void Init(BotData data, int teamID)
        {
            this.data = data;
            this.teamID = teamID;
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

        public void Shoot(bool async)
        {
            shooter.shotRequested.Value = true;
            shooter.async = async;
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