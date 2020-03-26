using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Match
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI timeText = null;

        void Update()
        {
            float totalSeconds = MatchManager.Instance.TimeRemaining;

            int minutes = (int)(totalSeconds / 60);
            int seconds = (int)totalSeconds - minutes * 60;
            int millis = (int)((totalSeconds - minutes * 60 - seconds) * 100);

            timeText.text = minutes.ToString("D1") + ":" + seconds.ToString("D2") + "." + millis.ToString("D2");
        }
    }
}