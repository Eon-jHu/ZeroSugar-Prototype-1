using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Sound
{
    card_draw,    
    card_select,
    card_place_down,
    card_place_deny,
    impact,
    weapon_throw,
    attack_vocal
}

public class AudioPlayer : Singleton<AudioPlayer>
{
    private Dictionary<Sound, AudioClip> sounds = new();

    protected new void Awake()
    {
        base.Awake();
        
        foreach (var clip in Resources.LoadAll<AudioClip>("Sounds").ToList())
        {
            if (Enum.TryParse(clip.name, out Sound sound))
            {
                sounds.Add(sound, clip);
            }
        }
    }

    public static void PlaySound2D(Sound sound, float volume = 1)
    {
        Instance.PlaySound(sound, 0.0f, Vector3.zero, volume);
    }
    
    public static void PlaySound3D(Sound sound, Vector3 position, float volume = 1)
    {
        Instance.PlaySound(sound, 0.0f, Vector3.zero, volume);
    }

    private void PlaySound(Sound sound, float spatialBlend, Vector3 position, float volume = 1)
    {
        if (sounds.TryGetValue(sound, out AudioClip clip))
        {
            AudioSource source = new GameObject(clip.name + " sound").AddComponent<AudioSource>();
            source.spatialBlend = spatialBlend;
            source.transform.position = position;
            
            source.clip = clip;
            source.volume = volume;
            source.playOnAwake = false;
            source.Play();
            
            Destroy(source.gameObject, clip.length);
        }
    }
}
