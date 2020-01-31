using UnityEngine;

namespace Bot
{
    public class Hardpoint : MonoBehaviour
    {
        private SpriteRenderer sr;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public void Init(Color color)
        {
            sr.color = color;
            sr.enabled = true;
        }
    }
}
