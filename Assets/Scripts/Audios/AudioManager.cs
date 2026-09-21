using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public Sound[] sounds;

    private List<AudioSource> activeSources = new List<AudioSource>();

    void Awake()
    {
        Instance = this;
       // DontDestroyOnLoad(gameObject); 
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
       // Play("Music");
    }

    void Update()
    {

    }

    /*  public void Play(string name)
      {
          Sound s = Array.Find(sounds, sounds => sounds.name == name);
          s.source.Play();
      } */

    public void Play(string name, AudioSource source)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
        //    Debug.LogWarning("Sound not found: " + name);
            return;
        }

        source.clip = s.clip;
        source.volume = s.volume;
        source.pitch = s.pitch;
        source.loop = s.loop;

        source.Play();

        if (!activeSources.Contains(source))
            activeSources.Add(source);
    }

    public void PlayAtPosition(string soundName, Vector3 position)
      {
          Sound s = Array.Find(sounds, sound => sound.name == soundName);

          if (s == null)
          {
          //    Debug.LogWarning("Sound not found: " + soundName);
              return;
          }

          AudioSource.PlayClipAtPoint(
              s.clip,
              position,
              s.volume
          );
      }

    public void StopAll()
    {
        // Stop the AudioSources passed to Play()
        foreach (AudioSource source in activeSources)
        {
            if (source != null)
                source.Stop();
        }

        // Stop the AudioSources belonging to Sound objects too
        foreach (Sound s in sounds)
        {
            if (s.source != null)
                s.source.Stop();
        }

        activeSources.Clear();
    }

    /*  public void PlayAtPosition(string soundName, Vector3 position)
      {
          Debug.Log("Looking for sound: " + soundName);
          Debug.Log("Number of sounds in array: " + sounds.Length);

          foreach (Sound sound in sounds)
          {
              Debug.Log("Sound in array: [" + sound.name + "]");
          }

          Sound s = Array.Find(
              sounds,
              sound => sound.name.Trim() == soundName.Trim()
          );

          if (s == null)
          {
              Debug.LogWarning("Sound not found: [" + soundName + "]");
              return;
          }

          Debug.Log("FOUND SOUND: " + s.name);

          GameObject soundObject = new GameObject("3D Sound - " + soundName);
          soundObject.transform.position = position;

          AudioSource source = soundObject.AddComponent<AudioSource>();

          source.clip = s.clip;
          source.volume = s.volume;
          source.pitch = s.pitch;
          source.spatialBlend = 1f;

          source.Play();

          Destroy(soundObject, s.clip.length / Mathf.Abs(s.pitch));
      } */
}
