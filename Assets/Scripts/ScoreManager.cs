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

    public class OnScoreUpdateArgs{

        public int multiplier;
        public int score;
    }
	public event EventHandler<EventArgs> onScoreUpdateListener;
	private void OnScoreUpdateEmitter(object sender, OnScoreUpdateArgs e)
	{
        // ajouter la logique interne
        _score += (e.score * e.multiplier);
        _textScore.text = _score.ToString();
        print("I'm inside the event OnScoreUpdateEmitter" + e);

	}

    private void Awake()
    {
        GetComponent<GameManager>().onScoreListener += OnScoreUpdateEmitter;
    }
    //private void Awake()
    //{
    //	UpdateTextScore();
    //}


}
