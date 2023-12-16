using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            Debug.LogWarning("passato");
            pile.Push(value);
            spawnedPrefab = value;
        }
    }

    //FIXME: Public? o faccio funzioni custom?
    public Stack<GameObject> pile = new();

    public void RevesePile()
    {
        pile.Reverse();

        foreach (GameObject piece in pile)
        {
            if(pile.First() == piece) 
                piece.transform.SetParent(null);
            else
                piece.transform.SetParent(pile.First().transform);
        }
    }

    public Tile(Tile source)
    {
        this.movesToMove = source.movesToMove;
        this.pieceInfo = source.pieceInfo;
    }
}
