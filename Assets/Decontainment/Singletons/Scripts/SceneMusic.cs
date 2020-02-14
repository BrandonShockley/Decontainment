using System;
using UnityEngine;

public class SceneMusic : SceneSingleton<SceneMusic>
{
    [SerializeField]
    private MusicLoopData musicLoopData = null;

    public event Action OnMusicLoopDataChange;

    public MusicLoopData MusicLoopData
    {
        get { return musicLoopData; }
        set {
            musicLoopData = value;
            OnMusicLoopDataChange?.Invoke();
        }
    }
}