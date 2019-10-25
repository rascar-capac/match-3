using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
	[SerializeField]
	private AudioClip[] _audioClip;
	private bool _play;
	private bool _toogleChange = false;
	private AudioSource _audioSource;

	private void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		FindObjectOfType<GameManager>().onGameStartListener += OnPlayMusicEmitter;
		FindObjectOfType<GameManager>().onGameEndListener += OnStopMusicEmitter;
		FindObjectOfType<GridManager>().onSwitchListener += OnPlaySwitchSoundEmitter;
	}
	public void OnPlayMusicEmitter(object sender, EventArgs e)
	{
		_audioSource.clip = _audioClip[0];
		_audioSource.Play();
	}
	public void OnStopMusicEmitter(object sender, EventArgs e)
	{
		_audioSource.clip = _audioClip[0];
		_audioSource.Stop();
	}
	public void OnPlaySwitchSoundEmitter(object sender, GridManager.OnSwitchEventArgs e)
	{
		//_audioSource.clip = _audioClip[1];
		//if (!_audioSource.isPlaying)
		//{
			_audioSource.PlayOneShot(_audioClip[1]);
			//_audioSource.clip = _audioClip[0];
		//}
		
	}
}
