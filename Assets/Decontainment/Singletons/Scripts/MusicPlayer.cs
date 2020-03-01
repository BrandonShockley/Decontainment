using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : PersistentSingleton<MusicPlayer>
{
    private const float CROSSFADE_DURATION = 1.5f;

    private MusicLoopData mainLoop;
    private MusicLoopData fadeLoop;
    private float crossfadeTimer;

    private AudioSource mainAudio;
    private AudioSource fadeAudio;

    [RuntimeInitializeOnLoadMethod]
    static void InitOnStartup()
    {
        MusicPlayer instance = Instance;
    }

    void Awake()
    {
        mainAudio = gameObject.AddComponent<AudioSource>();
        fadeAudio = gameObject.AddComponent<AudioSource>();
        mainAudio.playOnAwake = false;

        SceneManager.activeSceneChanged += (Scene current, Scene next) =>
        {
            if (SceneMusic.Instance != null) {
                SceneMusic.Instance.OnMusicLoopDataChange += HandleMusicChange;
                if (SceneMusic.Instance.MusicLoopData != mainLoop) {
                    HandleMusicChange();
                }
            } else {
                HandleMusicChange();
            }
        };
        HandleMusicChange();
    }

    void Update()
    {
        crossfadeTimer += Time.unscaledDeltaTime;
        mainAudio.volume = Mathf.Lerp(0, 1, crossfadeTimer / CROSSFADE_DURATION);
        fadeAudio.volume = Mathf.Lerp(1, 0, crossfadeTimer / CROSSFADE_DURATION);

        // Loop if we need to
        if (mainLoop != null && mainAudio.timeSamples > mainLoop.LoopOriginSample) {
            mainAudio.timeSamples = mainAudio.timeSamples - mainLoop.LoopOriginSample + mainLoop.LoopDestinationSample;
        }

        if (fadeLoop != null && fadeAudio.timeSamples > fadeLoop.LoopOriginSample) {
            fadeAudio.timeSamples = fadeAudio.timeSamples - fadeLoop.LoopOriginSample + fadeLoop.LoopDestinationSample;
        }
    }

    private void HandleMusicChange()
    {
        SwapAudio();
        crossfadeTimer = 0;
        SceneMusic sceneMusic = SceneMusic.Instance;
        if (sceneMusic == null) {
            mainLoop = null;
            mainAudio.clip = null;
        } else {
            mainLoop = sceneMusic.MusicLoopData;

            mainAudio.clip = mainLoop.Clip;
            if (mainLoop.Clip != null) {
                mainAudio.timeSamples = 0;
                mainAudio.Play();
            }
        }
    }

    private void SwapAudio()
    {
        MusicLoopData tempLoop = mainLoop;
        mainLoop = fadeLoop;
        fadeLoop = tempLoop;

        AudioSource tempAudio = mainAudio;
        mainAudio = fadeAudio;
        fadeAudio = tempAudio;
    }
}
