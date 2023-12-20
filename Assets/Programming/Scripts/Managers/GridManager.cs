using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    private Tile[,] loadedLevel = new Tile[4, 4];

    public static Action OnTileMoved;

    //Bool che verrà modificato nel WinCheck() e utilizzato quando DoTween avrà finito l'animazione.
    private bool playerWon;

    #region Variabili per la gestione dei feedback
    //Coroutine che gestirà i feedback grafici della movimento di una pila
    Coroutine coroutineFeedback;

    public delegate void SetTileParentDelegate();

    #endregion

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
        #region Conversione da World a Cell

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

        Tile fromTile = loadedLevel[-cellPos.y, cellPos.x];
        Tile toTile = loadedLevel[-cellPos.y - cellDirection.y, cellPos.x + cellDirection.x];
        
        #endregion

        if (!LegitMoveCheck(fromTile, toTile)) return;

        GameManager.current.AddMove(); //FIXME: Action?

        //La pila viene invertità siccome nel movimento fatto si ruota la pila di componenti del sandwich.
        fromTile.pile.Reverse();

        #region Feedback e parte grafica
        //Controllo della coroutine per renderla il più "safe" possibile.
        if(coroutineFeedback != null)
            StopCoroutine(coroutineFeedback);

        //La funzione che dovrà runnare dopo un check la coroutine successiva, fatta in modo da non dover passare l'intera Tile.
        SetTileParentDelegate SetPileParentFunction = fromTile.SetPileParent;
        //Start della coroutine che svolgerà l'animazione del feedback
        coroutineFeedback = StartCoroutine(FeedbackManager.current.PlayFeedbackMove(fromTile.pile.First(), fromTile.pile.Count, SetPileParentFunction, toTile.pile.First(), direction)); //FiXME: Action?
        
        #endregion
        
        //Muoviamo tutti i pezzi impilati da FromTile alla pila di ToTile
        toTile.AddPile(fromTile.pile);

        loadedLevel[-cellPos.y, cellPos.x] = null;

        OnTileMoved?.Invoke();
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

    private bool LegitMoveCheck(Tile from, Tile to)
    {
        if (from == null || to == null) return false;

        //Controllo mosse necessarie
        int playerMoves = GameManager.current.Moves;
        if (from.MoveToMove > playerMoves || to.MoveToMove > playerMoves) return false;

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
