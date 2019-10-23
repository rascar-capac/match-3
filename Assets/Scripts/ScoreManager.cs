using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
	[SerializeField]
	private Text _textScore;
	[SerializeField]
	private float _score = 0;
	

	public event EventHandler<EventArgs> onScoreUpdateListener;
	private void OnScoreUpdateEmitter(object sender, EventArgs e)
	{
		// ajouter la logique interne
		print("I'm inside the event OnScoreUpdateEmitter" + e);
		onScoreUpdateListener?.Invoke(this, new EventArgs());
		UpdateTextScore();
	}
	private void UpdateTextScore()
	{
		_textScore.text = "Score : " + Mathf.Round(_score * 10 / 10f);
	}
	//private void Awake()
	//{
	//	UpdateTextScore();
	//}

	private void Update()
	{
		if(_score > 0)
		{
			_score--;
		}
		UpdateTextScore();
	}
}
