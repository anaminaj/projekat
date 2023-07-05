using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffects 
{ 
    land,
    swap,
    resolve,
    upgrade,
    powerup,
    score

}

[RequireComponent(typeof(AudioSource))]
public class AudioMixer : Singleton<AudioMixer>
{
    [SerializeField]
    private AudioSource music, soundEffects;

    [
        Header
        (
             "0 = land\n" +
             "1 = swap\n" +
             "2 = resolve\n" +
             "3 = upgrade\n" +
             "4 = powerup\n" +
             "5 = score\n" 


        )
        
    ]
    [SerializeField]
    private AudioClip[] sounds;

    protected override void Init()
    {
        soundEffects = GetComponent<AudioSource>();
    }
    // play backround music
    public void PlayMusic()
    {
        music.Play();
    }

    // pause/unpause backround music
    public void PauseMusic(bool pause)
    {
        if (pause)
        {
            music.Pause();
        }
        else
            music.UnPause();
    }

    // play a sound effect
    public void PlaySound(SoundEffects effect)
    {
        soundEffects.PlayOneShot(sounds[ (int) effect]);
    }

    // play a sound effect after a time delay
    public IEnumerator PlayDelayedSound(SoundEffects effect, float t)
    {
        yield return new WaitForSeconds(t);
        PlaySound(effect);
    }
}