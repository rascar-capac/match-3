using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TileType
{
    Normal,
    SpecialA,
    SpecialB,
    SpecialC
}

public enum TileFamily
{
    Meat,
    Vegetable,
    Fruit,
    Special
}

public class Tile : MonoBehaviour
{
    public int _cellIndex;
    public TileType _tileType;
    public string _name;
    public TileFamily _tileFamily;
    public Sprite[] _display;
    public Color _color;
}
