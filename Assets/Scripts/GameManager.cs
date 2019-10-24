using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{

    #region Params
    [SerializeField]
    private Button _button;
    private bool _isPaused = false;

    GridManager helpers;
    #endregion

    #region EventArgs
    public class OnClickEventArgs
    {
        public Vector3 mousePos;
    }
    #endregion

    #region Event Invokers
    public void OnAppInitialize()
    {
        onAppInitializeListener?.Invoke(this, new EventArgs());
    }
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
	public void OnDrag()
	{
		onDragListener?.Invoke(this, new EventArgs());
	}
	#endregion

    #region Event Listeners
    public event EventHandler<EventArgs> onAppInitializeListener;

    public event EventHandler<EventArgs> onGameStartListener;

    public event EventHandler<OnClickEventArgs> onClickListener;

	public event EventHandler<EventArgs> onDragListener;

	public event EventHandler<EventArgs> onGamePauseListener;

    public event EventHandler<EventArgs> onGameResumeListener;

    public event EventHandler<EventArgs> onGameEndListener;
    #endregion

    #region Event Emitters
    public void OnAppInitializeEmitter(object sender, EventArgs e)
    {
        // ajouter la logique interne
        print("I'm inside the event OnAppInitializeEmitter" + e);
    }

    public void OnSwitchEmitter(object sender, GridManager.OnSwitchEventArgs e)
    {
        List<List<Cell>> matches = new List<List<Cell>>();
        CheckMatches(e.cells, e.columnsRows, e.switchedCell, e.targetedCell, matches);
        CheckMatches(e.cells, e.columnsRows, e.targetedCell, e.switchedCell, matches);

        if(matches.Count == 0)
        {

        }
        else
        {
            // switch
            // match event -> score et cascades
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
        // ajouter la logique interne
        print("I'm inside the event OnGameEndEmitter" + e);
        this.enabled = false;
    }

    #endregion

    #region Methodes
    private void Awake()
    {
        //FindObjectOfType<SoundManager>().onPlaySoundListener += OnAppInitializeEmitter;
        GetComponent<GridManager>().onSwitchListener += OnSwitchEmitter;
        OnAppInitialize();
        helpers = GetComponent<GridManager>();
    }
    private void Start()
    {

        #region souscriptions en début de partie
        FindObjectOfType<TimerManager>().onGameEndTimerListener += OnGameEndEmitter;
        FindObjectOfType<ScoreManager>().onScoreUpdateListener += OnGameStartEmitter;
        //helpers.onSwitchListener += OnSwitchEmitter;
        //onGameStartListener += OnGameStartEmitter;
        #endregion
        OnGameStart();
        StartCoroutine(Playing());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Je suis en pause");
            TogglePause();
        }
    }
    public void TogglePause()
    {
        _isPaused = !_isPaused;

    }

    public void CheckMatches(Cell[] cells, Vector2Int columnsRows, Cell comparedCell, Cell otherCell, List<List<Cell>> matches)
    {
        matches.Add(CheckLine(true, cells, columnsRows, comparedCell, otherCell));
        matches.Add(CheckLine(false, cells, columnsRows, comparedCell, otherCell));
    }

    public List<Cell> CheckLine(bool horizontalLine, Cell[] cells, Vector2Int columnsRows, Cell comparedCell, Cell otherCell)
    {
        List<Cell> matchingCells = new List<Cell>();
        int initialIndex = horizontalLine ? otherCell._gridPosition.x : otherCell._gridPosition.y;
        int cellToCheckIndex;

        int i = initialIndex;
        bool isMatching = true;
        while(isMatching && --i >= 0)
        {
            cellToCheckIndex = horizontalLine ? otherCell._gridPosition.y * columnsRows.x + i : i * columnsRows.x + otherCell._gridPosition.x;
            isMatching = CheckCell(comparedCell, cells[cellToCheckIndex], matchingCells);
        }

        i = initialIndex;
        isMatching = true;
        while(isMatching && ++i < (horizontalLine ? columnsRows.x : columnsRows.y))
        {
            cellToCheckIndex = horizontalLine ? otherCell._gridPosition.y * columnsRows.x + i : i * columnsRows.x + otherCell._gridPosition.x;
            isMatching = CheckCell(comparedCell, cells[cellToCheckIndex], matchingCells);
        }

        if(matchingCells.Count > 2)
        {
            return matchingCells;
        }
        return null;
    }

    public bool CheckCell(Cell comparedCell, Cell cellToCheck, List<Cell> matchingCells)
    {
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
    #endregion

    #region Co-routines
    public IEnumerator Playing()
    {
        //Touch touch = Input.GetTouch(0);
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Je clique");

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
}
