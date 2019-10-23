using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
	[SerializeField]
	private AudioClip _mainMusic;

	public event EventHandler<EventArgs> onPlaySoundListener;

	public void OnPlayMainMusicEmitter(object sender, EventArgs e)
	{
		// ajouter la logique interne
		print("I play main music");
		onPlaySoundListener?.Invoke(this, new EventArgs());
	}
	public void OnPlaySwipeSoundEmitter(object sender, EventArgs e)
	{

	}
	public void OnPlayBonusSoundEmitter(object sender, EventArgs e)
	{

	}
}
