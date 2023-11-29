using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private float masterVolumePercent = .2f;
    private float sfxVolumePercent = 1;
    private float musicVolumePercent = 1;

    private AudioSource[] musicSources;
    int activeMusicSourceIndex;

    private Transform audioListener;
    private Transform playerTf;

    private void Awake()
    {
        instance = this;
        musicSources = new AudioSource[2];
        for (int i = 0; i < 2; i++)
        {
            GameObject newMusicSource = new GameObject("Music source " + (i + 1));
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();
            newMusicSource.transform.parent = transform;
        }
        audioListener = FindObjectOfType<AudioListener>().transform;
        if (FindObjectOfType<Player>() != null)
        {
            playerTf = FindObjectOfType<Player>().transform;
        }
    }

    private void Update()
    {
        if (playerTf != null)
        {
            audioListener.position = playerTf.position;
        }
    }
    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();
        StartCoroutine(AnimateMusicCrossFade(fadeDuration));
        
    }

    public IEnumerator AnimateMusicCrossFade(float fadeDuration = 1)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1-activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
}


