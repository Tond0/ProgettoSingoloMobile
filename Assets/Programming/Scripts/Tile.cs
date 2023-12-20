using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    
    
    //Spawned prefab
    private GameObject spawnedPrefab;
    public GameObject SpawnedPrefab
    {
        get { return spawnedPrefab; }
        set
        {
            Piece spawnedPiece = new(value);
            pile.Push(spawnedPiece);

            spawnedPrefab = value;
        }
    }

    //FIXME: Public? o faccio funzioni custom?
    public Stack<Piece> pile = new();


    public void SetPileParent()
    {
        foreach (Piece piece in pile)
        {
            if(pile.First() == piece) 
                piece.transform.SetParent(GameManager.current.transform); //FIXME: Il parent deve essere corretto!
            else
                piece.transform.SetParent(pile.First().transform);
        }
    }

    public void AddPile(Stack<Piece> pileToAdd)
    {
        foreach (Piece piece in pileToAdd)
        {
            pile.Push(piece);
        }
    }

    public Tile(Tile source)
    {
        this.movesToMove = source.movesToMove;
        this.pieceInfo = source.pieceInfo;
    }
}
