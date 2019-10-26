using UnityEngine;

public class SoundController : Singleton<SoundController>
{
    public Camera cam;

    public AudioClip[] splashSounds;
    public AudioClip errorSound;
    public AudioClip explosionSound;

    public void PlayRandomSound(AudioClip[] sounds)
    {
        this.PlaySound(sounds[Random.Range(0, sounds.Length)]);
    }

    public void PlaySound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, this.cam.transform.position, 0.8f);
    }
}
