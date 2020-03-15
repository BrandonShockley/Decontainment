using Bot;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match
{
    public class MatchManager : SceneSingleton<MatchManager>
    {
        [SerializeField]
        private string returnSceneName = null;
        [SerializeField]
        private float timeLimit = 60;

        private float timeRemaining;

        public event Action<int> OnVictory;
        public event Action OnTimeUp;

        public float TimeRemaining => timeRemaining;

        void Awake()
        {
            Time.timeScale = 1;
            BotManager.Instance.OnTeamDisable += (int teamID) =>
            {
                Time.timeScale = 0;
                Debug.Log("Team " + teamID + " all disabled. Returning to editor.");
                int victorTeamID = teamID == 0 ? 1 : 0;
                OnVictory?.Invoke(victorTeamID);
                StartCoroutine(ReturnToEditor());
            };
        }

        void Start()
        {
            Instantiate(MatchData.Instance.mapPrefab, Vector3.zero, Quaternion.identity);
            timeRemaining = timeLimit;
        }

        void FixedUpdate()
        {
            timeRemaining = Mathf.Max(timeRemaining - Time.fixedDeltaTime, 0);

            if (timeRemaining <= 0) {
                Time.timeScale = 0;
                Debug.Log("Time is up. Returning to editor.");
                OnTimeUp?.Invoke();
                StartCoroutine(ReturnToEditor());
            }
        }

        // TODO: This is a temp thing
        // We should have a proper match results screen
        private IEnumerator ReturnToEditor()
        {
            yield return new WaitForSecondsRealtime(1.5f);
            SceneManager.LoadScene(returnSceneName);
        }
    }
}