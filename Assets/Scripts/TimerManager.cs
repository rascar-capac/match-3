using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class TimerManager : MonoBehaviour
{
	[SerializeField]
	private Text _textTimer;
	[SerializeField]
	private float _timer;
	private bool _endGame = false;
	

	public event EventHandler<EventArgs> onGameEndTimerListener;
	private void OnGameEndTimerEmitter(object sender, EventArgs e)
	{
		onGameEndTimerListener?.Invoke(this, new EventArgs());
	}
	private void UpdateTextTimer()
	{
		_textTimer.text = Mathf.Round(_timer * 10 / 10f).ToString();
	}
	private void Awake()
	{
		FindObjectOfType<GameManager>().onGameEndListener += OnGameEndTimerEmitter;
		UpdateTextTimer();
	}
	private void Update()
	{
		if (_timer > 0)
		{
			UpdateTextTimer();
			_timer -= Time.deltaTime;
		}
		else if (!_endGame)
		{
			OnGameEndTimerEmitter(this, new EventArgs());
			_endGame = true;
			_timer = 0;
			UpdateTextTimer();
		}
	}
}
