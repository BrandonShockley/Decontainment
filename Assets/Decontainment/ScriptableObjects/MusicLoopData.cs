using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicLoopData", menuName = "ScriptableObjects/MusicLoopData", order = 1)]
public class MusicLoopData : ScriptableObject
{
    [SerializeField]
    private AudioClip clip = null;
    [SerializeField]
    private int loopDestinationSample = 0;
    [SerializeField]
    private int loopOriginSample = 0;

    public AudioClip Clip { get { return clip; } }
    public int LoopDestinationSample { get { return loopDestinationSample; } }
    public int LoopOriginSample { get { return loopOriginSample; } }

    void OnValidate()
    {
        // Wrap around if samples are negative
        if (clip != null) {
            if (loopDestinationSample < 0) {
                loopDestinationSample = clip.samples + loopDestinationSample;
            }
            if (loopOriginSample < 0) {
                loopOriginSample = clip.samples + loopOriginSample;
            }
        }
    }
}
