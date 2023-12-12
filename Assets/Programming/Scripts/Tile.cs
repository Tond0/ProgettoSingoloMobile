using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VSCodeEditor;

[Serializable]
public class Tile
{

    //Counter: Quante mosse devono essere fatte prima di poter essere spostato?
    [SerializeField] private int movesToMove;
    public int MoveToMove { get => movesToMove; }

    //Scriptable Flyweight
    [SerializeField] public SandwichPieceScriptable pieceInfo;

    public static Tile Clone(Tile source)
    {
        Tile destination = new(source.movesToMove, source.pieceInfo);
        return destination;
    }

    public Tile(int movesToMove, SandwichPieceScriptable pieceInfo)
    {
        this.movesToMove = movesToMove;
        this.pieceInfo = pieceInfo;
    }
}
