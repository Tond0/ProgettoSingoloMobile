using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    private Tile[,] loadedLevel = new Tile[4,4];

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
                if (level.LevelGrid[i, j].pieceInfo == null) { continue; }

                Debug.Log(i * 4 + j);

                Vector3 worldPos = new(j, 0, -i);
                Vector3 cellPos = GetCellCenter(worldPos);

                //Copy così non andremo in futuro a modificare lo scriptable.
                loadedLevel[i, j] = new Tile(level.LevelGrid[i, j])
                {
                    //Salviamo un riferimento del pezzo spawnato
                    SpawnedPrefab = Instantiate(level.LevelGrid[i, j].pieceInfo.prefab, cellPos, Quaternion.identity, this.transform)
                };

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

        Tile fromTile = loadedLevel[-cellPos.y, cellPos.x];
        Tile toTile = loadedLevel[-cellPos.y - intDirection.y, cellPos.x + intDirection.x];

        if (!LegitMoveCheck(fromTile, toTile)) return;

        Debug.Log(" ");
        Debug.Log("From Pile:");
        foreach (GameObject piece in fromTile.pile)
        {
            Debug.Log(piece);
        }
        Debug.Log(" ");

        FeedbackManager.current.PlayFeedbackMove(fromTile.pile.First(), fromTile.pile.Count, toTile.pile.First());
        
        fromTile.RevesePile();

        foreach (GameObject piece in fromTile.pile)
        {
            toTile.pile.Push(piece);
        }

        Debug.Log(toTile.pile.First());

        loadedLevel[-cellPos.y, cellPos.x] = null;

        OnTileMoved?.Invoke();
    }

    private bool LegitMoveCheck(Tile from, Tile to)
    {
        if (from == null || to == null) return false;


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
        int pieces = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if(loadedLevel[i,j] == null) continue;

                pieces++;
            }
        }

        //Se ci sono più di 2 pezzi di sandwich (le 2 fette di pane)...
        if (pieces != 2) return false;

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
