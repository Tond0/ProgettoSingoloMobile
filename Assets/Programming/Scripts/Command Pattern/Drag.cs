using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;

public class Drag : ICommand
{
    //Variabili
    private Vector3 origin;
    private Vector2 direction;

    public delegate void SetTileParentDelegate();
    //FIXME: Da eliminare se non serve, usato solo per debug per ora.
    public GridMove playerMove;

    public static Action<MoveFeedbackInfo> OnMoveStart;
    public static Action<int> OnMoveFinished;

    public CommandStatus Execute(CommandReceiver logicReceiver)
    {
        if (logicReceiver is GridManager grid)
        {
            //Traduzione to Tile
            playerMove = grid.PosToGridMove(origin, direction);
            
            //Check della mossa
            if (!grid.LegitMoveCheck(playerMove)) return CommandStatus.Failure;

            //Cloniamo le Tile così da non andare ad intaccare più l'istanza delle Tile 
            //siccome potrebbe servirci in futuro per un Undo e Redo.
            Tile fromTile = new(playerMove.originTile);
            Tile toTile = new(playerMove.destinationTile);

            #region Parte grafica
            MoveFeedbackInfo info = new(fromTile, toTile, direction, grid.PlayerWon);
            
            OnMoveStart?.Invoke(info);
            #endregion

            //Modifiche alle Tile
            fromTile.Pile.Reverse();

            toTile.AddPile(fromTile.Pile);

            //Update della grid livello
            grid.UpdateTile(fromTile.GridPos, null);

            grid.UpdateTile(toTile.GridPos, toTile);


            OnMoveFinished.Invoke(1);

            return CommandStatus.Success;
        }
        else
        {
            Debug.LogError("Mi hai dato il receiver errato");
            return CommandStatus.Failure;
        }
    }

    public void Undo(CommandReceiver receiver)
    {
        if (receiver is GridManager grid)
        {

            //Utilizziamo le Tile non intaccate nell'Execute (possiamo anche modificarle tanto l'unica cosa che può succedere
            //dopo un undo è un Redo che funziona come un Execute e che quindi riprenderà delle instanza nuove e corrette).

            SetTileParentDelegate SetTileParentFunction = playerMove.originTile.SetPileParent;

            Vector2Int destinationTilePos = playerMove.originTile.GridPos;
            Vector3 worldPos = new Vector3(destinationTilePos.y, 0, -destinationTilePos.x);
            Vector3 targetDestination = grid.GetCellCenter(worldPos);


            #region Parte grafica
            MoveFeedbackInfo info = new(playerMove.originTile, targetDestination, -direction);
            OnMoveStart?.Invoke(info);
            #endregion


            //Reset delle tile al valore originale (prima dell'Execute accaduto prima di questo Undo)
            grid.UpdateTile(playerMove.originTile.GridPos, playerMove.originTile);

            grid.UpdateTile(playerMove.destinationTile.GridPos, playerMove.destinationTile);

            OnMoveFinished.Invoke(-1);
        }
        else
        {
            Debug.LogError("Mi hai dato il receiver errato");
        }
    }

    //Anche se si poteva direttamente richiamare Execute ho creato questo metodo per diminuire il più possibile 
    //la confusione tra gli script.
    public void Redo(CommandReceiver receiver) => Execute(receiver);

    //FIXME: Serve davvero o possiamo utilizzare l'Execute?

    // public void Redo(Receiver receiver)
    // {
    //    if (receiver is GridManager grid)
    //     {
    //         //Parte grafica
    //         SetTileParentDelegate SetTileParentFunction = playerMove.originTile.SetPileParent;

    //         //FIXME: non c'è un modo migliore di farlo? magari usando l'action in gridmanager.Move()?
    //         //Start della coroutine che svolgerà l'animazione del feedback
    //         FeedbackManager.current.StartCoroutine(FeedbackManager.current.PlayFeedbackMove(playerMove.originTile.Pile.First(), playerMove.originTile.Pile.Count, SetTileParentFunction, playerMove.destinationTile.Pile.First().GoTo, playerMove.direction));

    //         //Modifiche alle Tile
    //         playerMove.originTile.Pile.Reverse();

    //         playerMove.destinationTile.AddPile(playerMove.originTile.Pile);


    //         //Update della grid livello
    //         grid.UpdateTile(playerMove.originTile.GridPos, null);

    //         grid.UpdateTile(playerMove.destinationTile.GridPos, playerMove.destinationTile);


    //         GameManager.current.AddMove();  //FIXME: Usare l'action sotto.
    //     }
    //     else
    //     {
    //         Debug.LogError("Mi hai dato il receiver errato");
    //     } 
    // }

    public Drag(Vector3 origin, Vector2 direction)
    {
        this.origin = origin;
        this.direction = direction;
    }
}

//La mossa effettuata
public struct GridMove
{
    public Tile originTile;
    public Tile destinationTile;
}
