using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainAudioController : MonoBehaviour
{
    public static ChainAudioController instance;
    AudioSource activeMusic;

    private void Awake()
    {
        instance = this;
        activeMusic = GetComponent<AudioSource>();
    }

    public void AddTempSource(AudioClip clip, float fadeTime, float volume)
    {
        StartCoroutine(PlaySoundEffect(clip, fadeTime, volume));
    }

    public void CrossFade(AudioClip clip, float fadeTime, bool loop, bool playOriginalAfter, float originalClipFadeTime, float volume)
    {
        StartCoroutine(CrossFadeMusic(clip, fadeTime, loop, playOriginalAfter, originalClipFadeTime, volume));
    }

    IEnumerator PlaySoundEffect(AudioClip clip, float fadeTime, float volume)
    {
        volume = Mathf.Clamp(volume, 0, 1);

        AudioSource soundEffectSource = gameObject.AddComponent<AudioSource>();
        soundEffectSource.clip = clip;
        soundEffectSource.loop = false;
        float fadeAmountPerSecond;

        if (fadeTime != 0)
        {
            soundEffectSource.volume = 0;
            fadeAmountPerSecond = volume / fadeTime;
        }
        else
        {
            soundEffectSource.volume = volume;
            fadeAmountPerSecond = 0;
        }


        soundEffectSource.Play();
        float timePlaying = 0;

        do
        {
            soundEffectSource.volume += fadeAmountPerSecond * Time.deltaTime;

            yield return new WaitForEndOfFrame();
            timePlaying += Time.deltaTime;
        } while (timePlaying <= fadeTime && timePlaying <= clip.length);

        soundEffectSource.volume = volume;

        do
        {
            yield return new WaitForEndOfFrame();
            timePlaying += Time.deltaTime;
        } while (timePlaying <= clip.length);

        Destroy(soundEffectSource);
    }

    IEnumerator CrossFadeMusic(AudioClip clip, float fadeTime, bool loop, bool playOriginalAfter, float fadeTimeToOriginal, float newVolume)
    {
        float activeVolume = activeMusic.volume;
        float fadeAmountPerSecond = activeVolume / fadeTime;
        float fadeNewAmountPerSecond = newVolume / fadeTime;

        AudioClip originalClip = activeMusic.clip;
        float timeOfClip = activeMusic.time;

        AudioSource newMusicSource = gameObject.AddComponent<AudioSource>();
        newMusicSource.clip = clip;
        newMusicSource.volume = 0;
        newMusicSource.loop = loop;
        newMusicSource.priority = activeMusic.priority;
        newMusicSource.pitch = activeMusic.pitch;
        newMusicSource.panStereo = activeMusic.panStereo;
        newMusicSource.spatialBlend = activeMusic.spatialBlend;
        newMusicSource.outputAudioMixerGroup = activeMusic.outputAudioMixerGroup;
        newMusicSource.mute = activeMusic.mute;
        newMusicSource.bypassEffects = activeMusic.bypassEffects;
        newMusicSource.bypassListenerEffects = activeMusic.bypassListenerEffects;
        newMusicSource.bypassReverbZones = activeMusic.bypassReverbZones;
        newMusicSource.reverbZoneMix = activeMusic.reverbZoneMix;

        newMusicSource.Play();

        float timePlaying = 0;
        do
        {
            activeMusic.volume -= fadeAmountPerSecond * Time.deltaTime;
            newMusicSource.volume += fadeNewAmountPerSecond * Time.deltaTime;

            yield return new WaitForEndOfFrame();
            timePlaying += Time.deltaTime;
        } while (timePlaying <= fadeTime);

        newMusicSource.volume = newVolume;

        if (playOriginalAfter)
        {
            StartCoroutine(PlaySourceAfterOtherStops(activeMusic, newMusicSource, originalClip, timeOfClip, fadeTimeToOriginal));
        }
        else
        {

            Destroy(activeMusic);
            activeMusic = newMusicSource;
        }
    }

    IEnumerator PlaySourceAfterOtherStops(AudioSource sourceToPlay, AudioSource sourceToWaitFor, AudioClip clip, float clipTime, float fadeTime)
    {
        sourceToPlay.Pause();
        do
        {
            yield return new WaitForEndOfFrame();
        } while (sourceToWaitFor.isPlaying);
        Destroy(sourceToWaitFor);
        sourceToPlay.Play();
        sourceToPlay.time = clipTime;

        float timePlaying = 0;
        float fadeAmountPerSecond;

        if (fadeTime != 0)
        {
            sourceToPlay.volume = 0;
            fadeAmountPerSecond = 1 / fadeTime;
        }
        else
        {
            sourceToPlay.volume = 1;
            fadeAmountPerSecond = 0;
        }

        do
        {
            try
            {
                sourceToPlay.volume += fadeAmountPerSecond * Time.deltaTime;
            }
            catch (System.Exception)
            {
                activeMusic.volume = 1;
                break;
            }

            yield return new WaitForEndOfFrame();
            timePlaying += Time.deltaTime;
        } while (timePlaying <= fadeTime && timePlaying <= clip.length);

        
    }
}
