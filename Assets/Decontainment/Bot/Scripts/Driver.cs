using System;
using UnityEngine;

namespace Bot
{
    public class Driver : MonoBehaviour
    {
        public enum Direction
        {
            FORWARD,
            BACKWARD,
            LEFT,
            RIGHT,
        }

        public bool async;
        public float speed = 1;
        public float remainingDistance;
        public Direction direction;

        public bool Running { get { return !async && remainingDistance != 0; } }

        void Awake()
        {
            GetComponent<Health>().OnDisable += HandleDisabled;
        }

        void FixedUpdate()
        {
            if (remainingDistance > 0 || remainingDistance < 0) {
                int sign = Math.Sign(remainingDistance);
                float maxDeltaDistance = Time.fixedDeltaTime * speed;

                bool willArrive = remainingDistance * sign <= maxDeltaDistance;
                float deltaDistance = willArrive ? remainingDistance : maxDeltaDistance * sign;

                Vector3 directionVector = direction == Direction.FORWARD ? transform.right
                    : direction == Direction.BACKWARD ? -transform.right
                    : direction == Direction.LEFT ? transform.up
                    : -transform.up;
                transform.position += directionVector * deltaDistance;
                remainingDistance -= deltaDistance;

            }
        }

        private void HandleDisabled()
        {
            enabled = false;
        }
    }
}