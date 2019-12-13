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

        public Action onComplete;

        public float speed = 1;
        public float remainingDistance;
        public Direction direction;

        void Awake()
        {
            GetComponent<Health>().OnDisable += HandleDisabled;
        }

        void Update()
        {
            if (remainingDistance > 0 || remainingDistance < 0) {
                int sign = Math.Sign(remainingDistance);
                float maxDeltaDistance = Time.deltaTime * speed;

                bool willArrive = remainingDistance * sign <= maxDeltaDistance;
                float deltaDistance = willArrive ? remainingDistance : maxDeltaDistance * sign;

                Vector3 directionVector = direction == Direction.FORWARD ? transform.right
                    : direction == Direction.BACKWARD ? -transform.right
                    : direction == Direction.LEFT ? transform.up
                    : -transform.up;
                transform.position += directionVector * deltaDistance;
                remainingDistance -= deltaDistance;

                if (willArrive) {
                    onComplete?.Invoke();
                    onComplete = null;
                }
            } else {
                onComplete?.Invoke();
                onComplete = null;
            }
        }

        private void HandleDisabled()
        {
            enabled = false;
        }
    }
}