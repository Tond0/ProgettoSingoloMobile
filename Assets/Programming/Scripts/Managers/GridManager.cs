using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    private Tile[,] loadedLevel = new Tile[4, 4];

    public static Action OnTileMoved;

    //Bool che verrà modificato nel WinCheck() e utilizzato quando DoTween avrà finito l'animazione.
    private bool playerWon;

    private void OnEnable()
    {
        GameManager.OnLevelSelected += DrawGrid;
        InputHandler.OnDrag += Move;
    }

    private void OnDisable()
    {
        GameManager.OnLevelSelected -= DrawGrid;
        InputHandler.OnDrag -= Move;
    }

    //Funzione che OnGameStarted creerà la griglia instanziando gli oggetti necessari
    private void DrawGrid(LevelScriptable level)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (level.LevelGrid[i, j].pieceInfo == null) continue;

                Debug.Log(i * 4 + j);

                Vector3 worldPos = new(j, 0, -i);
                Vector3 cellPos = GetCellCenter(worldPos);

                Instantiate(level.LevelGrid[i, j].pieceInfo.prefab, cellPos, Quaternion.identity, this.transform);
            }
        }

        //Copy
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                loadedLevel[i, j] = Tile.Clone(level.LevelGrid[i, j]);
            }
        }
    }

    //FIXME: Se viene usata al di fuori di questa classe considerare di renderla statica.
    private Vector3 GetCellCenter(Vector3 worldPos)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        Vector3 cellCenterPos = tilemap.GetCellCenterWorld(cellPos);

        return cellCenterPos;
    }

    private void Move(Vector3 startPos, Vector2 direction)
    {
        Vector3Int cellPos = tilemap.WorldToCell(startPos);

        Debug.Log(direction);
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            direction.x = Mathf.Sign(direction.x) * 1;
            direction.y = 0;
        }
        else
        {
            direction.y = Mathf.Sign(direction.y) * 1;
            direction.x = 0;
        }

        Vector2Int intDirection = Vector2Int.CeilToInt(direction);

        Debug.Log("IntDirection " + intDirection);

        Tile fromTile = loadedLevel[-cellPos.y, cellPos.x];
        Debug.Log("From " + -cellPos.y + " " + cellPos.x);

        Debug.Log("To " + (-cellPos.y - intDirection.y) + " " + (cellPos.x + intDirection.x));
        Tile toTile = loadedLevel[-cellPos.y - intDirection.y, cellPos.x + intDirection.x];

        Debug.Log(" ");

        if (!LegitMoveCheck(fromTile, toTile)) return;

        fromTile.pieceInfo = null;

        OnTileMoved?.Invoke();
    }

    private bool LegitMoveCheck(Tile from, Tile to)
    {
        //FIXME: sarebbe meglio se fosse Tile a essere null non PieceInfo
        if (from.pieceInfo == null || to.pieceInfo == null) return false;


        //Controllo mosse necessarie
        int playerMoves = GameManager.current.Moves;
        if (from.MoveToMove < playerMoves || to.MoveToMove < playerMoves) return false;

        //Confronto del tipo
        PieceType aType = from.pieceInfo.type;
        PieceType bType = to.pieceInfo.type;

        if (aType == PieceType.Bread)
        {
            //Switch case così che in caso (gioco di parole) si debba aggiungere un PieceType sarà più comodo
            switch (bType)
            {
                case PieceType.Topping:

                    if (aType == PieceType.Bread) return false;

                    break;

                case PieceType.Bread:

                    if (aType == PieceType.Bread) return WinCheck();

                    break;
            }
        }
        return true;
    }

    private bool WinCheck()
    {
        bool oneBread = false;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (loadedLevel[i, j].pieceInfo.type == PieceType.Bread)
                {
                    if (oneBread) return false;
                    else oneBread = true;
                }
            }
        }

        //"Non succede, ma se succede..." da errore
        //E ritorna true in modo tale che anche se non è una vittoria "meritata" il player non si hardstuckki nel livello.
        if (!oneBread) Debug.LogError("Come mai la griglia è interamente vuota bro?");

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector3 worldPos = new(j, 0, -i);
                Vector3 cellPos = GetCellCenter(worldPos);

                Gizmos.DrawWireCube(cellPos, Vector3.one);
            }
        }
    }
}
