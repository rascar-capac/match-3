using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

enum CellDirection
{
	Right,
	UpRight,
	Up,
	UpLeft,
	Left,
	DownLeft,
	Down,
	DownRight
}

public class Cell
{
    public int _index;
    
    public GameObject _tileGo;
    public Tile _tile;
    public Vector2Int _gridPosition;
    public Vector3 _gridWorldPosition;
    public GridManager _manager;
    public int _cellColor;
    public Cell[] _adjacentCells;
    string _name;

    public bool isEmpty;
   
    public void getAdjacentCells()
    {
        _adjacentCells = new Cell[8];
        _adjacentCells[(int)CellDirection.Right] = _manager.GetCellFromPosition(_gridPosition.x + 1, _gridPosition.y);
        _adjacentCells[(int)CellDirection.UpRight] = _manager.GetCellFromPosition(_gridPosition.x + 1, _gridPosition.y + 1);
        _adjacentCells[(int)CellDirection.Up] = _manager.GetCellFromPosition(_gridPosition.x, _gridPosition.y + 1);
        _adjacentCells[(int)CellDirection.UpLeft] = _manager.GetCellFromPosition(_gridPosition.x - 1, _gridPosition.y + 1);
        _adjacentCells[(int)CellDirection.Left] = _manager.GetCellFromPosition(_gridPosition.x - 1, _gridPosition.y);
        _adjacentCells[(int)CellDirection.DownLeft] = _manager.GetCellFromPosition(_gridPosition.x - 1, _gridPosition.y - 1);
        _adjacentCells[(int)CellDirection.Down] = _manager.GetCellFromPosition(_gridPosition.x, _gridPosition.y - 1);
        _adjacentCells[(int)CellDirection.DownRight] = _manager.GetCellFromPosition(_gridPosition.x + 1, _gridPosition.y - 1);
    }

    public Cell(Vector3 gridWorldPosition, Vector2Int cellPosition, GridManager manager, Tile tile, GameObject tileGo)
    {
        _gridWorldPosition = gridWorldPosition;
        _gridPosition = cellPosition;
        _tile = tile;
        _manager = manager;
        _tileGo = tileGo;
    }
}
