using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Match
{
    public class VictoryBanner : MonoBehaviour
    {
        [SerializeField]
        private AnimationCurve curve = null;

        private TextMeshProUGUI tm;

        void Awake()
        {
            tm = GetComponentInChildren<TextMeshProUGUI>();

            MatchManager.Instance.OnVictory += HandleVictory;
            MatchManager.Instance.OnTimeUp += HandleTimeUp;
        }

        private void HandleVictory(int teamID)
        {
            tm.text = "Team " + (teamID + 1) + " wins!";
            StartCoroutine(GrowRoutine());
        }

        private void HandleTimeUp()
        {
            tm.text = "Time's up!";
            StartCoroutine(GrowRoutine());
        }

        private IEnumerator GrowRoutine()
        {
            float timer = 0;
            while (timer < curve.keys[curve.length - 1].time) {
                timer += Time.unscaledDeltaTime;

                Vector3 scale = transform.localScale;
                scale.y = curve.Evaluate(timer);
                transform.localScale = scale;
                yield return null;
            }
        }
    }
}
