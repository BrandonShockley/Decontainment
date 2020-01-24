using System;
using UnityEngine;

namespace Bot
{
    public class Turner : MonoBehaviour
    {
        public enum Direction
        {
            LEFT,
            RIGHT,
        }

        public bool async;
        public float speed = 45;
        public float remainingDegrees;
        public Direction direction;

        public bool Running { get { return !async && remainingDegrees != 0; } }

        void Awake()
        {
            GetComponent<Health>().OnDisable += HandleDisabled;
        }

        void FixedUpdate()
        {
            if (remainingDegrees > 0 || remainingDegrees < 0) {
                int sign = Math.Sign(remainingDegrees);
                float maxDeltaDegrees = Time.fixedDeltaTime * speed;

                bool willArrive = remainingDegrees * sign <= maxDeltaDegrees;
                float deltaDegrees = willArrive ? remainingDegrees : maxDeltaDegrees * sign;

                float directionVector = direction == Direction.LEFT ? 1 : -1;
                transform.Rotate(0, 0, deltaDegrees * directionVector);
                remainingDegrees -= deltaDegrees;
            }
        }

        private void HandleDisabled()
        {
            enabled = false;
        }
    }
}