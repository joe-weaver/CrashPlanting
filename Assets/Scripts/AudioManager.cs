using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
	public Sound[] sounds;

    void Awake() {
        foreach(Sound sound in sounds) {
			sound.source = gameObject.AddComponent<AudioSource>();
			sound.source.clip = sound.clip;

			sound.source.volume = sound.volume;
			sound.source.pitch = sound.pitch;
			sound.source.loop = sound.loop;
		}
    }

    public void Play(string name) {
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if(s == null) {
			Debug.Log("Error: Attempted to play sound \"" + name + "\", which does not exist.");
			return;
		}
		s.source.Play();
	}
}
