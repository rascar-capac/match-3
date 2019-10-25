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
    private int _score = 0;

    //public class OnScoreUpdateArgs{

    //    public int multiplier;
    //    public int score;
    //}
	public event EventHandler<EventArgs> onScoreUpdateListener;
	private void OnScoreUpdateEmitter(object sender, EventArgs e)
	{

	}
    private void Update()
	{
		_score += _score;
		_textScore.text = _score.ToString();
	}
}
