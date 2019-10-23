using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/Tile")]

public class TileData : ScriptableObject
{
    public TileType _tileType;
    public TileFamily _tileFamily;
    public string _name;
    public Sprite _display;
    public Color _color;
}
