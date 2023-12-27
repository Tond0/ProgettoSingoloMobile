using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : Receiver
{
    [SerializeField] private Tilemap tilemap;
    private Tile[,] loadedLevel = new Tile[4, 4];

    public static Action OnTileMoved;

    //Bool che verrà modificato nel WinCheck() e utilizzato quando DoTween avrà finito l'animazione.
    private bool playerWon;

    #region Variabili per la gestione dei feedback
    //Coroutine che gestirà i feedback grafici della movimento di una pila
    Coroutine coroutineFeedback;

    #endregion

    private void OnEnable()
    {
        GameManager.OnLevelSelected += DrawGrid;
    }

    private void OnDisable()
    {
        GameManager.OnLevelSelected -= DrawGrid;
    }

    //Funzione che OnGameStarted creerà la griglia instanziando gli oggetti necessari
    private void DrawGrid(LevelScriptable level)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (level.LevelGrid[i, j].pieceInfo == null) { continue; } //FIXME: Da provare a mettere == Null non pieceinfo ma l'intera Tile.

                Vector3 worldPos = new(j, 0, -i);
                Vector3 cellPos = GetCellCenter(worldPos);

                //Copy così non andremo in futuro a modificare lo scriptable, overridando la posizione nella griglia.
                loadedLevel[i, j] = new Tile(level.LevelGrid[i, j], new Vector2Int(i, j))
                {
                    //Salviamo un riferimento del pezzo spawnato
                    SpawnedPrefab = Instantiate(level.LevelGrid[i, j].pieceInfo.prefab, cellPos, Quaternion.identity, this.transform)
                };

            }
        }
    }

    //FIXME: Se viene usata al di fuori di questa classe considerare di renderla statica.
    public Vector3 GetCellCenter(Vector3 worldPos)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        Vector3 cellCenterPos = tilemap.GetCellCenterWorld(cellPos);

        return cellCenterPos;
    }


    public void UpdateTile(Vector2Int tilePos, Tile newValue)
    {
        if (loadedLevel[tilePos.x, tilePos.y] == null)
            Debug.LogWarning("Da vuoto" + " pos " + tilePos.x + " " + tilePos.y);
        else
            Debug.LogWarning("Da " + loadedLevel[tilePos.x, tilePos.y].Pile.First().name + " pos " + tilePos.x + " " + tilePos.y);
        if (newValue != null)
            Debug.LogWarning("A " + newValue.Pile.First().name);
        else
            Debug.LogWarning("A vuoto");

        loadedLevel[tilePos.x, tilePos.y] = newValue;

    }


    /// <summary>
    /// Traduce da posizione e direzione a cella d'inizio e cella di destinazione.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public GridMove PosToGridMove(Vector3 startPos, Vector2 direction)
    {
        //Otteniamo, data la posizione d'inizio tocco, la cella che vogliamo spostare.
        Vector3Int cellPos = tilemap.WorldToCell(startPos);

        /* Spiegazione del perché di questo passaggio
        1. Avere più sicurezza siccome con i procedimenti dopo arrecati siamo sicuri 100% che risulterà in un vettore
        contenente un asse a 0 e un asse a 1 o -1.

        2. Come nel gioco originale, se si fa un movimento in diagonale non finisce che il movimento viene dato vano o
        annullato ma viene "immaginato" che movimento intenzionale si volesse fare, tradotto in orizzontale o verticale.
        */

        //Siccome la direzione non avrà sia X che Y diversi da 0 (siccome non esiste il movimento diagonale)
        //cerchiamo qual'è l'asse su cui ci vogliamo spostare... 

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            //...e ne definiamo la direzione limitandola ad 1 tenendo conto del segno
            direction.x = Mathf.Sign(direction.x) * 1;
            direction.y = 0;
        }
        else
        {
            //...e ne definiamo la direzione limitandola ad 1 tenendo conto del segno
            direction.y = Mathf.Sign(direction.y) * 1;
            direction.x = 0;
        }

        //Con le coordinate create prima abbiamo la direzione 
        Vector2Int cellDirection = Vector2Int.CeilToInt(direction);

        //Formazione della mossa effettuata dal player sottoforma di Tile.
        GridMove playerMove = new();

        //Creiamo una nuova Tile anche se in Move dovremmo ritrovarla nella griglia in modo tale che 
        //il command abbia sempre la Tile di origine per un Undo senza rischiare che questa diventi Null.
        Tile fromTile = new(loadedLevel[-cellPos.y, cellPos.x]);
        playerMove.originTile = fromTile;

        //Stessa storia per la ToTile.
        Tile toTile = new(loadedLevel[-cellPos.y - cellDirection.y, cellPos.x + cellDirection.x]);
        playerMove.destinationTile = toTile;

        return playerMove;
    }

    //FIXME: Per debug, da rimuovere prima di ship?
    private void DebugPile(Stack<Piece> pile)
    {
        Debug.Log(" ");
        Debug.Log("From Pile:");

        foreach (Piece piece in pile)
        {
            Debug.Log(piece.gameObject);
        }
        Debug.Log(" ");
    }

    public bool LegitMoveCheck(GridMove playerMove)
    {
        if (playerMove.originTile == null || playerMove.destinationTile == null) return false;

        //Controllo mosse necessarie
        int playerMoves = GameManager.current.Moves;
        if (playerMove.originTile.MoveToMove > playerMoves || playerMove.destinationTile.MoveToMove > playerMoves) return false;

        //Confronto del tipo
        PieceType aType = playerMove.originTile.pieceInfo.type;
        PieceType bType = playerMove.destinationTile.pieceInfo.type;

        if (aType == PieceType.Bread)
        {
            //Switch case così che in caso (gioco di parole) si debba aggiungere un PieceType sarà più comodo
            switch (bType)
            {
                case PieceType.Topping:

                    if (aType == PieceType.Bread) return false;

                    break;

                case PieceType.Bread:

                    if (aType == PieceType.Bread) { playerWon = WinCheck(); return playerWon; }

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
                if (loadedLevel[i, j] == null) continue;

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
