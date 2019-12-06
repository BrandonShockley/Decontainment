using System;
using UnityEngine;

namespace Bot
{
    public class Driving : BaseAction
    {
        public enum Direction
        {
            FORWARD,
            BACKWARD,
            LEFT,
            RIGHT,
        }

        public Direction direction;
        public float remainingDistance;

        public Driving(Controller controller) : base(controller) {}

        override public void Update()
        {
            if (remainingDistance > 0 || remainingDistance < 0) {
                int sign = Math.Sign(remainingDistance);
                float maxDeltaDistance = Time.deltaTime * c.moveSpeed;

                bool willArrive = remainingDistance * sign <= maxDeltaDistance;
                float deltaDistance = willArrive ? remainingDistance : maxDeltaDistance * sign;

                Vector3 directionVector = direction == Direction.FORWARD ? c.transform.right
                    : direction == Direction.BACKWARD ? -c.transform.right
                    : direction == Direction.LEFT ? c.transform.up
                    : -c.transform.up;
                c.transform.position += directionVector * deltaDistance;
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
    }
}