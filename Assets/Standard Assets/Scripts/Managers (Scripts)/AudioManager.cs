using UnityEngine;
using System.Collections.Generic;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
	AudioSource currentMusic;
	AudioSource[] musics;
	public float nextMusicDelay;
	
	public override void Awake ()
	{
		base.Awake ();
		musics = GetComponentsInChildren<AudioSource>();
	}
	
	public virtual void Update ()
	{
		if (currentMusic == null)
			NextMusic (nextMusicDelay);
		else if (!currentMusic.isPlaying)
		{
			currentMusic.Stop();
			NextMusic (nextMusicDelay);
		}
	}
	
	public virtual void NextMusic (float delay = 0)
	{
		if (musics.Length > 1)
		{
			AudioSource previousMusic = currentMusic;
			while (previousMusic == currentMusic)
				currentMusic = musics[Random.Range(0, musics.Length)];
		}
		else
			currentMusic = musics[0];
		currentMusic.PlayDelayed(delay);
	}
}
