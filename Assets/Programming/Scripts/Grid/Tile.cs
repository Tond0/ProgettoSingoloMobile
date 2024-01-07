using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Tile
{
    //Counter: Quante mosse devono essere fatte prima di poter essere spostato?
    [SerializeField] private int movesToMove;
    public int MoveToMove { get => movesToMove; }

    private Vector2Int gridPos;
    public Vector2Int GridPos { get => gridPos; }

    //Scriptable Flyweight
    [SerializeField] public SandwichPieceScriptable pieceInfo;


    //Spawned prefab
    private GameObject spawnedPrefab;
    public GameObject SpawnedPrefab
    {
        get { return spawnedPrefab; }
        set
        {
            Piece spawnedPiece = value.GetComponent<Piece>();
            spawnedPiece.MovesToMove = movesToMove;

            pile.Push(spawnedPiece);

            spawnedPrefab = value;
        }
    }

    //FIXME: Public? o faccio funzioni custom?
    public Stack<Piece> Pile { get => pile; }
    private Stack<Piece> pile = new();


    public void SetPileParent()
    {
        foreach (Piece piece in pile)
        {
            if (pile.First() == piece)
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

    public void RemovePile(Stack<Piece> pileToSubstract)
    {
        foreach (Piece piece in pileToSubstract)
        {
            pile.Pop();
        }
    }

    //FIXME: Da rimuovere
    public void DebugStack()
    {
        Debug.Log(" ");
        foreach (Piece piece in pile)
        {
            Debug.Log(piece.name);
        }
        Debug.Log(" ");
    }

    //FIXME: Non c'è un modo migliore per clonare?
    //Si, avrei dovuto far essere Tile una struct così da non doverla clonare ogni volta per evitare il rischio che vecchi puntatori modificassero una Tile condivisa.
    //ma ho provato a cambiare in struct e ci sarebbe troppo lavoro da fare e io voglio chiudere sto gioco.
    //Next time penserò più alla possibilità di struct.

    public Tile(Tile source)
    {
        this.pieceInfo = source.pieceInfo;
        this.movesToMove = source.movesToMove;
        this.gridPos = source.gridPos;
    
        if (source.pile == null) return;
        this.pile = new Stack<Piece>(source.Pile);
    }

    public Tile(Tile source, Vector2Int gridPos)
    {
        this.pieceInfo = source.pieceInfo;
        this.movesToMove = source.movesToMove;
        this.gridPos = gridPos;

        if (source.pile == null) return;
        this.pile = source.Pile;
    }
}
