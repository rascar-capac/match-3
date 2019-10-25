using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private Button _pauseButton, _musicButton, _soundButton;
	public bool _gameIsPaused = false;
	[SerializeField]
	GameObject _pauseMenuUI;

    GridManager _helpers;
    #endregion

	#region Co-routines
	public IEnumerator Playing()
    {
        while (true)
        {

            if (Input.GetMouseButtonDown(0))
            {
                OnClick(new OnClickEventArgs()
                {
                    mousePos = Input.mousePosition
                });
            }

			if (Input.GetMouseButton(0))
			{
				OnDrag();
			}
			yield return null;
        }
    }
    #endregion

    #region EventArgs
    public class OnClickEventArgs
    {
        public Vector3 mousePos;
    }
    #endregion

    #region Event Listeners
    public event EventHandler<EventArgs> onGameStartListener;

	public event EventHandler<EventArgs> onGamePauseListener;

    public event EventHandler<EventArgs> onGameResumeListener;

    public event EventHandler<EventArgs> onGameEndListener;

    public event EventHandler<OnClickEventArgs> onClickListener;

	public event EventHandler<EventArgs> onDragListener;

    public event EventHandler<GridManager.OnDropEventArgs> onDropListener;

    public event EventHandler<ScoreManager.OnScoreUpdateArgs> onScoreListener;
	#endregion

	#region Event Invokers
	public void OnGameStart()
    {
        onGameStartListener?.Invoke(this, new EventArgs());
    }
    public void OnGamePause()
    {
        onGamePauseListener?.Invoke(this, new EventArgs());
    }
    public void OnGameResume()
    {
        onGameResumeListener?.Invoke(this, new EventArgs());
    }
    public void OnGameEnd()
    {
        onGameEndListener?.Invoke(this, new EventArgs());
	}
    public void OnClick(OnClickEventArgs e)
    {
        onClickListener?.Invoke(this, e);
    }
    public void OnDrop(GridManager.OnDropEventArgs e)
    {
        onDropListener?.Invoke(this, e);
    }
	public void OnDrag()
	{
		onDragListener?.Invoke(this, new EventArgs());
	}
	#endregion

    #region Event Emitters

    public void OnSwitchEmitter(object sender, GridManager.OnSwitchEventArgs e)
    {

        Tile tempTile = e.firstCell._tile;
        e.firstCell._tile = e.secondCell._tile;
        e.secondCell._tile = tempTile;

        List<List<Cell>> matches = new List<List<Cell>>();
        CheckMatches(e.cells, e.columnsRows, e.firstCell, matches);
        CheckMatches(e.cells, e.columnsRows, e.secondCell, matches);

        if(matches.Count == 0)
        {
            tempTile = e.firstCell._tile;
            e.firstCell._tile = e.secondCell._tile;
            e.secondCell._tile = tempTile;

            Debug.Log("no match");
            StartCoroutine(_helpers.Switching(e.firstCell, e.secondCell, _helpers._switchDuration));
        }
        else
        {
            Debug.Log(matches.Count + " matches");
            foreach(List<Cell> match in matches)
            {
                String matchDescription = "";
                foreach(Cell cell in match)
                {
                    matchDescription += cell._gridPosition + " - ";
                }
                Debug.Log(matchDescription);
            }
            // onMatchListener?.Invoke(this, new EventArgs(){})// match event -> score et cascades
        }
    }

    public void OnGameStartEmitter(object sender, EventArgs e)
    {
        // ajouter la logique interne
        print("I'm inside the event OnGameStartEmitter" + e);
    }
    public void OnGamePauseEmitter(object sender, EventArgs e)
    {
        // ajouter la logique interne
        print("I'm inside the event OnGamePauseEmitter" + e);
    }
    public void OnGameEndEmitter(object sender, EventArgs e)
    {
        print("I'm inside the event OnGameEndEmitter" + e);
		//this.enabled = false;

		SceneManager.LoadScene("EndScene");
	}
    #endregion

    #region Methods
    private void Awake()
    {
        _helpers = GetComponent<GridManager>();
        _helpers.onSwitchListener += OnSwitchEmitter;
	}
    private void Start()
    {
		#region souscriptions en début de partie
        FindObjectOfType<TimerManager>().onGameEndTimerListener += OnGameEndEmitter;
        FindObjectOfType<ScoreManager>().onScoreUpdateListener += OnGameStartEmitter;
		onGameStartListener += OnGameStartEmitter;
		onGamePauseListener += OnGamePauseEmitter;
		_pauseButton.onClick.AddListener(OnGamePause);

		#endregion
		OnGameStart();
        StartCoroutine(Playing());
    }

    public void CheckMatches(Cell[] cells, Vector2Int columnsRows, Cell comparedCell, List<List<Cell>> matches)
    {
        CheckLine(true, cells, columnsRows, comparedCell, matches);
        CheckLine(false, cells, columnsRows, comparedCell, matches);
    }

    public void CheckLine(bool horizontalLine, Cell[] cells, Vector2Int columnsRows, Cell comparedCell, List<List<Cell>> matches)
    {
        List<Cell> matchingCells = new List<Cell>();
        matchingCells.Add(comparedCell);
        int initialIndex = horizontalLine ? comparedCell._gridPosition.x : comparedCell._gridPosition.y;
        int cellToCheckIndex;

        int i = initialIndex;
        bool isMatching = true;
        while(isMatching && --i >= 0)
        {
            cellToCheckIndex = horizontalLine ? comparedCell._gridPosition.y * columnsRows.x + i : i * columnsRows.x + comparedCell._gridPosition.x;
            isMatching = CheckCell(comparedCell, cells[cellToCheckIndex], matchingCells);
        }

        i = initialIndex;
        isMatching = true;
        while(isMatching && ++i < (horizontalLine ? columnsRows.x : columnsRows.y))
        {
            cellToCheckIndex = horizontalLine ? comparedCell._gridPosition.y * columnsRows.x + i : i * columnsRows.x + comparedCell._gridPosition.x;
            isMatching = CheckCell(comparedCell, cells[cellToCheckIndex], matchingCells);
        }

        if(matchingCells.Count > 2)
        {
            matches.Add(matchingCells);
        }
    }

    public bool CheckCell(Cell comparedCell, Cell cellToCheck, List<Cell> matchingCells)
    {
        Debug.Log(cellToCheck._tile._name + " " + cellToCheck._gridPosition + " - " + comparedCell._tile._name + " " + comparedCell._gridPosition);
        if(cellToCheck._tile._name == comparedCell._tile._name)
        {
            matchingCells.Add(cellToCheck);
            return true;
        }
        else
        {
            return false;
        }
    }
	#region PauseManager
	public void TogglePause()
	{
		if (_gameIsPaused)
		{
			Resume();
		} else
		{
			Pause();
		}
	}
	public void Resume()
	{
		_pauseMenuUI.SetActive(false);
		Debug.Log("pause menu " + _pauseMenuUI);
		Time.timeScale = 1f;
		_gameIsPaused = false;
	}
	void Pause()
	{
		_pauseMenuUI.SetActive(true);
		Time.timeScale = 0f;
		_gameIsPaused = true;
	}
	public void LoadMenu()
	{
		SceneManager.LoadScene("MenuScene");
	}
	public void QuitGame()
	{
		Debug.Log("Quitting game...");
		Application.Quit();
	}
	#endregion
	#endregion
}
