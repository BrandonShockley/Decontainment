using UnityEngine;

public class SoundModulator : MonoBehaviour
{
    private new AudioSource audio;

	// Use this for initialization
	void Awake () {
        audio = GetComponent<AudioSource>();
        if (audio == null) {
            audio = gameObject.AddComponent<AudioSource>();
            audio.playOnAwake = false;
        }
	}

    public void PlayModClip(AudioClip clip, float minPitch = .95f, float maxPitch = 1.05f, float volume = 1.0f) {
        audio.pitch = Random.Range(minPitch, maxPitch);
        audio.volume = volume;
        audio.PlayOneShot(clip);
    }

    // public void PlayModClipLate(AudioClip clip, float minPitch = .95f, float maxPitch = 1.05f) {
    //     AudioSource audio = GameObject.Find("AudioPlayer").GetComponent<AudioSource>();
    //     audio.pitch = Random.Range(minPitch, maxPitch);
    //     audio.PlayOneShot(clip);
    // }
}