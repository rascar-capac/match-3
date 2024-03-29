﻿using System.Collections;
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
    public float _switchDuration = 0.3f;

    [SerializeField]
    float _dropDuration = 0.2f;

    Vector2 _cellSize;
    // cell half size
    Vector2 _cellExtents;
    // grid half size
    Vector2 _gridExtents;

    Cell _selectedCell;
    Cell _targetedCell;
    Vector2 _initialMousePosition;
    bool _isDragging;
    #endregion

    #region Coroutines
    public IEnumerator Switching(Cell cell, Cell targetedCell, float switchDuration, bool mustCheckMatch)
    {
        float t = 0;
        float tRatio;
        float tAnim;
        Vector3 startPosition = cell._tileGo.transform.position;
        Vector3 endPosition = targetedCell._tileGo.transform.position;
        while (t < switchDuration)
        {
            tRatio = t / switchDuration;
            tAnim = _switchAnimation.Evaluate(tRatio);
            cell._tileGo.transform.position = Vector3.Lerp(startPosition, endPosition, tAnim);
            targetedCell._tileGo.transform.position = Vector3.Lerp(endPosition, startPosition, tAnim);

            t += Time.deltaTime;
            yield return null;
        }
        cell._tileGo.transform.position = endPosition;
        cell._tileGo.transform.localScale = Vector3.one;

        targetedCell._tileGo.transform.position = startPosition;
        targetedCell._tileGo.transform.localScale = Vector3.one;

        bool tempEmpty = targetedCell._isEmpty;
        cell._isEmpty = tempEmpty;

        GameObject tempTileGo = cell._tileGo;
        cell._tileGo = targetedCell._tileGo;
        targetedCell._tileGo = tempTileGo;

        if(mustCheckMatch)
        {
            OnSwitch(new OnSwitchEventArgs()
            {
                cells = _cells,
                columnsRows = _columnsRows,
                firstCell = cell,
                secondCell = targetedCell
            });
        }
        yield return null;
    }

    //ENUMERATOR DE TEST

    public IEnumerator Dropping()
    {
        // drop animation
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
        return (y * _columnsRows.x + x);
    }
    public int GridPositionToIndex(Vector2Int position)
    {
        return GridPositionToIndex(position.x, position.y);
    }
    #endregion

    #region Event Args
    public class OnDropEventArgs
    {
        public Cell cellToDrop;
        public Cell emptyCell;
    }

    public class OnSwitchEventArgs
    {
        public Cell[] cells;
        public Vector2Int columnsRows;
        public Cell firstCell;
        public Cell secondCell;
    }
    #endregion

    #region Event Listeners
    public event EventHandler<OnSwitchEventArgs> onSwitchListener;

    #endregion

    #region Event Invokers
        public void OnSwitch(OnSwitchEventArgs e)
        {
            onSwitchListener?.Invoke(this, e);
        }
    #endregion

    #region Event Emitters
    public void OnGridInitializeEmitter(object sender, EventArgs e)
    {
        CreateGrid();
    }
    public class DropEventArgs
    {
        public List<Cell> _cells;
    }
    public void OnDropEmitter(object sender, DropEventArgs e)
    {
        foreach (Cell cell in e._cells)
        {
            if(cell._isEmpty && cell._adjacentCells[2] == null)
            {
                GenerateTile(cell);

            }
            if (cell._adjacentCells[2]._isEmpty && cell._adjacentCells != null)
            {
                StartCoroutine(Switching(cell, cell._adjacentCells[2], _dropDuration, true));
            }
        }
    }
    public void OnGridClickEmitter(object sender, GameManager.OnClickEventArgs e)
    {
        _initialMousePosition = _camera.ScreenToWorldPoint(e.mousePos);
        Cell cellToMove = GetCellFromPosition(ClampPositionToGrid(WorldToGridPosition(_initialMousePosition)));
        _selectedCell = cellToMove;
        _isDragging = true;
    }

    public void OnDragEmitter(object sender, EventArgs e)
    {
        if (_isDragging)
        {
            Vector2 currentMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            Tile tempTile = _selectedCell._tile;

            float switchAngle = Mathf.Atan2(currentMousePosition.y - _initialMousePosition.y, currentMousePosition.x - _initialMousePosition.x) * Mathf.Rad2Deg;
            float switchLimit = ((switchAngle < 45 && switchAngle > -45) || (switchAngle > 135 && switchAngle < -135) ? _cellExtents.x : _cellExtents.y);
            if (Vector2.Distance(currentMousePosition, _initialMousePosition) >= switchLimit)
            {
                int switchIndex = Mathf.FloorToInt(Mathf.Repeat((switchAngle + 45), 360) / 90) * 2;
                _targetedCell = _selectedCell._adjacentCells[switchIndex];
                if (_targetedCell != null)
                {
                    StartCoroutine(Switching(_selectedCell, _targetedCell, _switchDuration, true));
                    Debug.Log("(inside emitter) cells have switched; targetedCell: " + _targetedCell._tile + " + selectedCell: " + _selectedCell._tile);
                    _targetedCell = null;
                    _selectedCell = null;
                }
                _isDragging = false;
            }
        }
    }

    public void OnMatchEmitter(object sender, GameManager.OnMatchEventArgs e)
    {
        foreach(List<Cell> cells in e.matches)
        {
            foreach(Cell cell in cells)
            {
                Destroy(cell._tileGo);
                cell._isEmpty = true;
            }
        }

        for(int i = 0 ; i < _columnsRows.y ; i++)
        {
            List<Cell> emptyCells = new List<Cell>();
            for(int j = 0 ; j < _columnsRows.x ; j++)
            {
                Cell currentCell = GetCellFromPosition(j, i);
                if(currentCell._isEmpty)
                {
                    emptyCells.Add(currentCell);
                }
            }

            foreach(Cell cell in emptyCells)
            {
                bool isTilesAbove = true;
                int j = cell._gridPosition.y;
                while(isTilesAbove && GetCellFromPosition(cell._gridPosition.x, j)._isEmpty)
                {
                    if(j == _columnsRows.y - 1)
                    {
                        isTilesAbove = false;
                    }
                    else
                    {
                        j++;
                    }
                }
                if(isTilesAbove)
                {
                    Cell droppingTileCell = GetCellFromPosition(cell._gridPosition.x, j);
                    droppingTileCell._tileGo.transform.position = cell._gridWorldPosition;
                    cell._tileGo.transform.localScale = Vector3.one;
                    cell._tileGo = droppingTileCell._tileGo;
                    cell._tile = droppingTileCell._tile;

                    cell._isEmpty = false;
                    droppingTileCell._isEmpty = true;
                }
                else
                {
                    GenerateTile(cell);
                }
            }
        }

        foreach(Cell cell in _cells)
        {
            Debug.Log(cell._gridPosition + " " + cell._tile._name);
        }
        Dropping();
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

    void GenerateTile(Cell cell)
    {
        TileData tileData = _tileDatas[UnityEngine.Random.Range(0, _tileDatas.Length)];
        GameObject tileGO = new GameObject(tileData._name + " " + cell._index);
        tileGO.transform.position = cell._gridWorldPosition;

        SpriteRenderer sr = tileGO.AddComponent<SpriteRenderer>();

        Tile tempTile = tileGO.AddComponent<Tile>();

        tempTile._name = tileData._name;
        tempTile._cellIndex = cell._index;
        tempTile._tileFamily = tileData._tileFamily;
        tempTile._display = tileData._display;

        BoxCollider2D bx = tileGO.AddComponent<BoxCollider2D>();

        bx.size = new Vector2(_cellExtents.x, _cellExtents.y);
        bx.isTrigger = true;

        sr.sprite = tileData._display;
        sr.color = tileData._color;

        cell._isEmpty = false;
        cell._tile = tempTile;
    }
    void CreateCell(int x, int y)
    {
        int cellIndex = GridPositionToIndex(x, y);

        TileData tileData = _tileDatas[UnityEngine.Random.Range(0, _tileDatas.Length)];
        GameObject tileGO = new GameObject(tileData._name + " " + cellIndex);

        SpriteRenderer sr = tileGO.AddComponent<SpriteRenderer>();

        Tile tempTile = tileGO.AddComponent<Tile>();

        tempTile._name = tileData._name;
        tempTile._cellIndex = cellIndex;
        tempTile._tileFamily = tileData._tileFamily;
        tempTile._display = tileData._display;

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
            cell.getAdjacentCells();
        }
        print("Grid created");
    }

    private void Awake()
    {
        // rajoute la logique interne à l'event
        GameManager gm = GetComponent<GameManager>();
        _camera = Camera.main;
        gm.onGameStartListener += OnGridInitializeEmitter;
        gm.onClickListener += OnGridClickEmitter;
        gm.onDragListener += OnDragEmitter;
        gm.onMatchListener += OnMatchEmitter;
        _selectedCell = null;
        _targetedCell = null;
        _isDragging = false;
    }
    #endregion
}
