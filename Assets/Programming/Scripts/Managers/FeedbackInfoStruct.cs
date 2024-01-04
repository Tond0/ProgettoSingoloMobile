using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Per rendere la firma un pò meno una merda
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

    public MoveFeedbackInfo(Tile fromTile, Tile toTile, Vector2 direction, bool playerWinningState)
    {
        targetPiece = fromTile.Pile.First();
        piecesInPile = fromTile.Pile.Count;
        setTileParent = fromTile.SetPileParent;

        destinationPos = toTile.Pile.First().GoTo;

        this.direction = direction;

        this.lastTransition = playerWinningState;
    }

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
