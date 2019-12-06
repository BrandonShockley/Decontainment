using System;
using UnityEngine;

namespace Bot
{
    public class Turning : BaseAction
    {
        public enum Direction
        {
            LEFT,
            RIGHT,
        }

        public Direction direction;
        public float remainingDegrees;

        public Turning(Controller controller) : base(controller) {}

        override public void Update()
        {
            if (remainingDegrees > 0 || remainingDegrees < 0) {
                int sign = Math.Sign(remainingDegrees);
                float maxDeltaDegrees = Time.deltaTime * c.turnSpeed;

                bool willArrive = remainingDegrees * sign <= maxDeltaDegrees;
                float deltaDegrees = willArrive ? remainingDegrees : maxDeltaDegrees * sign;

                float directionVector = direction == Direction.LEFT ? 1 : -1;
                c.transform.Rotate(0, 0, deltaDegrees * directionVector);
                remainingDegrees -= deltaDegrees;

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