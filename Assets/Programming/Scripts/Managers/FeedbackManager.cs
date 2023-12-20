using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using MoreMountains.Feedbacks;
using UnityEditor;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    #region Instance
    public static FeedbackManager current;
    private void Awake()
    {
        if (current)
            Destroy(this);
        else
            current = this;
    }
    #endregion

    [Header("Player")]
    [SerializeField] private MMF_Player fb_Player_move;
    [Header("Feedbacks")]
    [SerializeField, Tooltip("Il nome dato al feedback che gestirà la posizione")] private string fb_pos_name;
    MMF_Position fb_pos;
    [SerializeField, Tooltip("Il punto di inizio della transizione"), Min(0)] private float initialPosYOffset = 3;
    [SerializeField, Tooltip("Quanto più in alto del pezzo si deve fermare?"), Min(0)] private float destinationPosYOffset = 0.25f;
    [Space()]

    [SerializeField, Tooltip("Il nome dato al feedback che gestirà la rotazione")] private string fb_rot_name;
    MMF_Rotation fb_rot;

    /* FIXME: Spiegami
    [Space()]

    [SerializeField, Tooltip("Il nome dato al feedback che gestirà il cambio parent")] private string fb_parent_name;
    MMF_SetParent fb_mesh_parent;
    */

    private void OnEnable()
    {
        fb_pos = fb_Player_move.GetFeedbackOfType<MMF_Position>(fb_pos_name);
        fb_rot = fb_Player_move.GetFeedbackOfType<MMF_Rotation>(fb_rot_name);
        //fb_mesh_parent = fb_Player_move.GetFeedbackOfType<MMF_SetParent>(fb_parent_name);
    }


    public IEnumerator PlayFeedbackMove(Tile fromTile, Tile toTile, Vector2 direction)
    {
        Piece targetPiece = fromTile.pile.First();
        int piecesInPile = fromTile.pile.Count;

        Piece destinationPiece = toTile.pile.First();

        //Se stava già andando allora skippa tutta l'animazione
        if (fb_Player_move.IsPlaying)
        {
            fb_Player_move.SkipToTheEnd();

            while (fb_Player_move.SkippingToTheEnd)
            {
                yield return null;
            }
        }

        fromTile.SetPileParent();

        //Pos
        fb_pos.AnimatePositionTarget = targetPiece.gameObject;

        fb_pos.InitialPosition = destinationPiece.GoTo + Vector3.up * (initialPosYOffset + piecesInPile);
        fb_pos.DestinationPosition = destinationPiece.GoTo;

        //Rot
        fb_rot.AnimateRotationTarget = targetPiece.transform;

        //FIXME: Magnitude?
        if (direction.x - direction.y > 0)
            fb_rot.RemapCurveOne = -180;
        else
            fb_rot.RemapCurveOne = 180;

        //Su che asse?
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            fb_rot.AnimateZ = true;
            fb_rot.AnimateX = false;
        }
        else
        {
            fb_rot.AnimateX = true;
            fb_rot.AnimateZ = false;
        }

        fb_Player_move.PlayFeedbacks();
    }
}


/* Roba che ho provato per capire che non mi sarebbe servita
Factory pattern?
-- Non c'è n'è il bisogno perché sarà sempre lo stesso tipo di feedback ma con un target diverso.

public static class PositionFeedbackFactory
{
    public static MMF_Position CreateFeedBack(GameObject target, Vector3 initialVector, Vector3 destinationVector)
    {
        MMF_Position fb_pos = new();

        fb_pos.AnimatePositionTarget = target;

        fb_pos.InitialPosition = initialVector;
        fb_pos.DestinationPosition = destinationVector;

        return fb_pos;
    }
}


public static class RotationFeedbackFactory
{

    public static MMF_Feedback CreateFeedBack(GameObject target, float remapCurveZero, float remapCurveOne)
    {
        MMF_Rotation fb_rot = new();

        fb_rot.AnimateRotationTarget = target.transform;

        fb_rot.RemapCurveZero = remapCurveZero;
        fb_rot.RemapCurveOne = remapCurveOne;

        return fb_rot;
    }
}
*/
