using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private List<AudioClip> musicClips;

    [SerializeField] private float pauseBetweenTracks = 4.0f;
    [SerializeField] private float volume = 0.5f;
    IEnumerator Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource && musicClips.Count > 0)
        {
            audioSource.spatialBlend = 0.0f;
            audioSource.volume = volume;
            int clips = musicClips.Count;
            int currentClip = 0;

            while (true)
            {
                if (currentClip >= clips)
                {
                    currentClip = 0;
                }

                audioSource.clip = musicClips[currentClip];
                audioSource.Play();

                yield return new WaitUntil(() => !audioSource.isPlaying);

                yield return new WaitForSeconds(pauseBetweenTracks);

                currentClip++;
            }
        }
    }
}
