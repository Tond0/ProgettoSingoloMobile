using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class Tile
{
    //FIXME: Serve?
    //La posizione nella griglia
    private Vector2Int pos;
    public Vector2Int Pos { get => pos; }

    //Counter: Quante mosse devono essere fatte prima di poter essere spostato?
    [SerializeField] private int movesToMove;
    public int MoveToMove { get => movesToMove; }

    //Scriptable Flyweight
    [SerializeField] private SandwichPieceScriptable pieceInfo;
    public SandwichPieceScriptable PieceInfo { get => pieceInfo; }
}
