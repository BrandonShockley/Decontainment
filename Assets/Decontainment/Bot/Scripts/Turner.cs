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

        public Action onComplete;

        public float speed = 45;
        public float remainingDegrees;
        public Direction direction;

        void Awake()
        {
            GetComponent<Health>().OnDisable += HandleDisabled;
        }

        void Update()
        {
            if (remainingDegrees > 0 || remainingDegrees < 0) {
                int sign = Math.Sign(remainingDegrees);
                float maxDeltaDegrees = Time.deltaTime * speed;

                bool willArrive = remainingDegrees * sign <= maxDeltaDegrees;
                float deltaDegrees = willArrive ? remainingDegrees : maxDeltaDegrees * sign;

                float directionVector = direction == Direction.LEFT ? 1 : -1;
                transform.Rotate(0, 0, deltaDegrees * directionVector);
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

        private void HandleDisabled()
        {
            enabled = false;
        }
    }
}