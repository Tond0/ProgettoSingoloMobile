using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Struct utilizzata per rendere la firma meno abominevole ma soprattutto per trovare un adeguato candidato per la traduzione tra 
//GridManager e FeedbackManager facendo modo che questi due non debbano contenere variabili che non li interessano.
public struct MoveFeedbackInfo
{
    private Piece targetPiece;
    public Piece Target { get => targetPiece; }

    private int piecesInPile;
    public int PiecesInPile { get => piecesInPile; }

    private Drag.SetTileParentDelegate setTileParent;
    public Drag.SetTileParentDelegate SetTileParent { get => setTileParent; }

    private Vector3 destinationPos;
    public Vector3 DestinationPos { get => destinationPos; }

    private Vector2 direction;
    public Vector2 Direction { get => direction; }

    private bool lastTransition;
    public bool LastTransition { get => lastTransition; }

    //Primo costruttore usato nell'Execute
    public MoveFeedbackInfo(Tile fromTile, Tile toTile, Vector2 direction, bool playerWinningState)
    {
        targetPiece = fromTile.Pile.First();
        piecesInPile = fromTile.Pile.Count;
        setTileParent = fromTile.SetPileParent;

        destinationPos = toTile.Pile.First().GoTo;

        this.direction = direction;

        this.lastTransition = playerWinningState;
    }

    //Primo costruttore usato nell'Undo
    public MoveFeedbackInfo(Tile fromTile, Vector3 destinationPos, Vector2 direction)
    {
        targetPiece = fromTile.Pile.First();
        piecesInPile = fromTile.Pile.Count;
        setTileParent = fromTile.SetPileParent;

        this.destinationPos = destinationPos;

        this.direction = direction;

        lastTransition = false;
    }
}
