using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridManager : MonoBehaviour
{
    //* rajouter la logique de switch dans le grid, vérifier le match dans le game

    #region Variables
    [SerializeField]
    Vector2 _gridSize;

    [SerializeField]
    Vector2Int _columnsRows;

    public Cell[] _cells;

    public AnimationCurve _switchAnimation;

    [HideInInspector]
    public Camera _camera;

    [SerializeField]
    TileData[] _tileDatas;

    [SerializeField]
    float _switchDuration = 0.5f;

    Vector2 _cellSize;
    // cell half size
    Vector2 _cellExtents;
    // grid half size
    Vector2 _gridExtents;

	[SerializeField]
	float minSwitchingRadius;

	Cell _selectedCell;
	Cell _targetedCell;
	Vector2 _initialMousePosition;
	bool _isDragging;
	#endregion
	#region Coroutines
	IEnumerator Switching(Cell cell, Cell targetedCell, float switchDuration)
    {
        for (int i = 0; i < cell._adjacentCells.Length; i++)
        {
            if (cell._adjacentCells[i] != null && i == targetedCell._index)
            {
                float t = 0;
                float tRatio;
                float tAnim;
                Vector3 startPosition = cell._tileGo.transform.position;
                Vector3 endPosition = GridPositionToWorldPosition(cell._adjacentCells[i]._gridPosition);
                while (t < switchDuration)
                {
                    tRatio = t / switchDuration;
                    tAnim = _switchAnimation.Evaluate(tRatio);
                    cell._tileGo.transform.position = Vector3.Lerp(startPosition, endPosition, tAnim);
                    /*cell._tileGo.transform.localScale = Vector3.one + Vector3.one * Mathf.Sin(tAnim * Mathf.PI);*/
                    t += Time.deltaTime;
                    yield return null;
                }
                cell._tileGo.transform.position = endPosition;
                cell._tileGo.transform.localScale = Vector3.one;

                GameObject tempTileGo = cell._tileGo;
                cell._tileGo = cell._adjacentCells[i]._tileGo;
                cell._adjacentCells[i]._tileGo = tempTileGo;

                Tile tempTile = cell._tile;
                cell._tile = cell._adjacentCells[i]._tile;
                cell._adjacentCells[i]._tile = tempTile;

                print("I moved " + tempTileGo);
                break;
            }
        }
        yield return null;
    }
    #endregion
    #region Helpers
    public Vector2Int IndexToGridPosition(int index)
    {
        return new Vector2Int(index % _columnsRows.x, index % _columnsRows.y);
    }

    public Vector2Int ClampPositionToGrid(int xPos, int yPos)
    {
        return new Vector2Int(Mathf.Clamp(xPos, 0, _columnsRows.x - 1), Mathf.Clamp(yPos, 0, _columnsRows.y - 1));
    }
    public Vector2Int ClampPositionToGrid(Vector2Int pos)
    {
        return ClampPositionToGrid(pos.x, pos.y);
    }
    public Vector2Int WorldToGridPosition(Vector2 position)
    {
        Vector2 localPosition = (Vector2)transform.InverseTransformPoint(position) + _gridExtents;
        return new Vector2Int(Mathf.FloorToInt(localPosition.x / _cellSize.x), Mathf.FloorToInt(localPosition.y / _cellSize.y));
    }

    public Cell GetCellFromPosition(int positionX, int positionY)
    {
        if (positionX >= 0 && positionX < _columnsRows.x && positionY >= 0 && positionY < _columnsRows.y)
        {
            return _cells[GridPositionToIndex(positionX, positionY)];
        }
        return null;
    }
    public Cell GetCellFromPosition(Vector2Int position)
    {
        return GetCellFromPosition(position.x, position.y);
    }
    public Vector3 GridPositionToWorldPosition(int xPos, int yPos)
    {
        return (Vector3)((new Vector2(xPos, yPos) * _cellSize) + _cellExtents - _gridExtents) + transform.position;
    }

    public Vector3 GridPositionToWorldPosition(Vector2Int pos)
    {
        return GridPositionToWorldPosition(pos.x, pos.y);
    }

    public int GridPositionToIndex(int x, int y)
    {
        return (x * _columnsRows.y + y);
    }
    public int GridPositionToIndex(Vector2Int position)
    {
        return GridPositionToIndex(position.x, position.y);
    }
    #endregion

    #region Event Args
    
    public class OnCellCreationArgs : EventArgs
    {
        public Vector2Int columnsRows;
        public Vector2Int cellSize;
    }
    #endregion

    #region Event Listeners
    public event EventHandler<GameManager.OnSwitchEventArgs> onSwitchListener;
    #endregion

    #region Event Invokers
    
    #endregion

    #region Event Emitters
    public void OnGridInitializeEmitter(object sender, EventArgs e)
    {
        CreateGrid();
        print("I'm inside the event Grid Init");
    }
   
    public void OnSwitchEventEmitter(object sender, GameManager.OnSwitchEventArgs e)
    {
        Tile ttm = e.tileToMove;
        Tile tgt = e.targetedTile;

        print("Switch event triggered");
    }



    public void OnGridClickEmitter(object sender, GameManager.OnClickEventArgs e)
    {
        Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(e.mousePos);
        print("input.mouseposition : " + e.mousePos);
        print("mouse world position : "+ mouseWorldPosition);
        Cell cellToMove = GetCellFromPosition(ClampPositionToGrid(WorldToGridPosition(mouseWorldPosition)));
        print("(inside emitter) cell :" + cellToMove._tile + " grid position: "+cellToMove._gridPosition+" grid world position: "+cellToMove._gridWorldPosition+" neighbors: "+cellToMove._adjacentCells.ToString());
		//StartCoroutine(Switching(new Cell(), new Cell(), _switchDuration));
		_selectedCell = cellToMove;
		_initialMousePosition = e.mousePos;
		_isDragging = true;
    }

	public void OnDragEmitter(object sender, EventArgs e)
	{
		if (_isDragging)
		{
			Vector2 currentMousePosition = Input.mousePosition;
			if (Vector3.Distance(currentMousePosition, _initialMousePosition) >= minSwitchingRadius)
			{
				float switchAngle = Mathf.Atan2(currentMousePosition.y - _initialMousePosition.y, currentMousePosition.x - _initialMousePosition.x) * Mathf.Rad2Deg;
				int switchIndex = Mathf.FloorToInt(Mathf.Repeat((switchAngle + 45), 360) / 90) * 2;
				_targetedCell = _selectedCell._adjacentCells[switchIndex];
				if (_targetedCell != null)
				{

					Debug.Log("(inside emitter) targetedCell :" + _targetedCell._tile + " grid position: " + _targetedCell._gridPosition + " grid world position: " + _targetedCell._gridWorldPosition + " neighbors: " + _targetedCell._adjacentCells.ToString());
				}
				_isDragging = false;
			}
		}
	}
	#endregion

	#region Methods

	void InitializeCellSize()
    {
        _cellSize.x = _gridSize.x / _columnsRows.x;
        _cellSize.y = _gridSize.y / _columnsRows.y;
        _cellExtents = _cellSize * 0.5f;
        _gridExtents = _gridSize * 0.5f;
    }

    /*
    yield return StartCoroutine(Switch(cellToMove, _switchDuration));
    */


    void CreateCell(int x, int y)
    {
        int cellIndex = GridPositionToIndex(x, y);

        TileData tileData = _tileDatas[UnityEngine.Random.Range(0, _tileDatas.Length)];
        GameObject tileGO = new GameObject(tileData._name+ " "+ cellIndex);
        

        SpriteRenderer sr = tileGO.AddComponent<SpriteRenderer>();

        Tile tempTile = tileGO.AddComponent<Tile>();

        tempTile._name = tileData._name;
        tempTile._cellIndex = cellIndex;
        tempTile._tileFamily = tileData._tileFamily;

        BoxCollider2D bx = tileGO.AddComponent<BoxCollider2D>();

        bx.size = new Vector2(_cellExtents.x, _cellExtents.y);
        bx.isTrigger = true;
        
        tileGO.transform.position = GridPositionToWorldPosition(x, y);
        

        sr.sprite = tileData._display;
        sr.color = tileData._color;
        _cells[cellIndex] = new Cell(tileGO.transform.position, new Vector2Int(x, y), this, tempTile, tileGO);
    }

    void CreateGrid()
    {
        InitializeCellSize();

        _cells = new Cell[_columnsRows.y * _columnsRows.x];

        for (int x = 0; x < _columnsRows.x; x++)
        {
            for (int y = 0; y < _columnsRows.y; y++)
            {

                CreateCell(x, y);

            }
        }
        foreach (Cell cell in _cells)
        {
            print("Getting adjacent cells of " + cell);
            cell.getAdjacentCells();
        }
        print("Grid created");
    }
    #endregion

    private void Awake()
    {
        // rajoute la logique interne à l'event
        GameManager gm = GetComponent<GameManager>();
        _camera = Camera.main;
        gm.onGameStartListener += OnGridInitializeEmitter;
        gm.onSwitchListener += OnSwitchEventEmitter;
        gm.onClickListener += OnGridClickEmitter;
		gm.onDragListener += OnDragEmitter;
		_selectedCell = null;
		_targetedCell = null;
		_isDragging = false;
	}
}
