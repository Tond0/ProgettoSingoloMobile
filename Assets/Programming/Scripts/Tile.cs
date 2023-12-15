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
    
    
    //Spawned prefab
    public GameObject SpawnedPrefab
    {
        get { return spawnedPrefab; }
        set
        {
            //Fatto per comodità
            goTo = value.transform.GetChild(0);
            spawnedPrefab = value;
        }
    }
    private GameObject spawnedPrefab;

    //La posizione in cui deve andare durante la transizione
    public Transform goTo;

    public Tile(Tile source)
    {
        this.movesToMove = source.movesToMove;
        this.pieceInfo = source.pieceInfo;
    }
}
