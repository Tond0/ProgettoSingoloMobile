using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Networking;

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

    private GameObject lastTarget;
    private GameObject lastDestinationPiece;

    public void PlayFeedbackMove(GameObject targetPiece, int targetStackedPieces, GameObject destinationPiece)
    {
        //Se stava già andando allora skippa tutta l'animazione
        /*if(fb_Player_move.IsPlaying)
        {
            lastTarget.transform.SetParent(lastDestinationPieceGoTo.parent);
            lastTarget.transform.position = lastDestinationPieceGoTo.position;
            lastTarget.transform.rotation = Quaternion.identity;
        }*/


        //Pos
        fb_pos.AnimatePositionTarget = targetPiece;
        
        Vector3 endingPos = destinationPiece.transform.position + Vector3.up * destinationPosYOffset * targetStackedPieces;
        fb_pos.InitialPosition = endingPos + Vector3.up * initialPosYOffset;
        fb_pos.DestinationPosition = endingPos;
        
        //Rot
        fb_rot.AnimateRotationTarget = targetPiece.transform;
        
        //Parent dei due gameobject
        targetPiece.transform.SetParent(destinationPiece.transform);


        //Last
        lastTarget = targetPiece;
        lastDestinationPiece = destinationPiece;


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