using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using MoreMountains.Feedbacks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

//Classe con il compito di gestire tutti i feedback e transizioni del gioco.
public class FeedbackManager : MonoBehaviour
{
    #region Instance & SetUp
    public static FeedbackManager current;
    private void Awake()
    {
        if (current)
            Destroy(this);
        else
            current = this;
    }
    #endregion

    [Header("Players")]
    [HeaderAttribute("MoveTile")]
    [SerializeField] private MMF_Player fb_Player_move;
    [SerializeField, Tooltip("Il punto di inizio della transizione"), Min(0)] private float initialPosYOffset = 3;

    [HeaderAttribute("ViewTransition")]
    [SerializeField] private MMF_Player fb_Player_ViewTransition;

    #region Actions
    private void OnEnable()
    {
        Drag.OnMoveStart += PlayFeedbackMove;
    }

    private void OnDisable()
    {
        Drag.OnMoveStart -= PlayFeedbackMove;
    }
    #endregion

    #region MoveFeedback
    Coroutine moveFBCoroutine;
    public void PlayFeedbackMove(MoveFeedbackInfo info)
    {
        if (moveFBCoroutine != null)
            StopCoroutine(moveFBCoroutine);

        moveFBCoroutine = StartCoroutine(PlayFeedbackMoveCoroutine(info));
    }

    //Una firma molto brutta, però farò davvero di tutto pur di non inserire Tile in questo script di Feedback a cui del Tile non gliene deve fregare nulla.
    private IEnumerator PlayFeedbackMoveCoroutine(MoveFeedbackInfo info)
    {

        //Se stava già andando allora skippa tutta l'animazione
        if (fb_Player_move.IsPlaying)
        {
            fb_Player_move.SkipToTheEnd();

            while (fb_Player_move.SkippingToTheEnd)
            {
                yield return null;
            }
        }
        
        //Funzione che abbiamo passato tramite un delegate in modo da non passare nessuna informazione che possa toccare le Tile
        info.SetTileParent();
        
        MMF_Position fb_pos = fb_Player_move.GetFeedbackOfType<MMF_Position>();
        MMF_Rotation fb_rot = fb_Player_move.GetFeedbackOfType<MMF_Rotation>();
        MMF_Events fb_gameWonEvent = fb_Player_move.GetFeedbackOfType<MMF_Events>();

        //Pos
        fb_pos.AnimatePositionTarget = info.Target.gameObject;

        fb_pos.InitialPosition = info.DestinationPos + Vector3.up * (initialPosYOffset + info.PiecesInPile);
        fb_pos.DestinationPosition = info.DestinationPos;


        //Rot
        fb_rot.AnimateRotationTarget = info.Target.transform;

        //FIXME: Magnitude?
        if (info.Direction.x - info.Direction.y > 0)
            fb_rot.RemapCurveOne = -180;
        else
            fb_rot.RemapCurveOne = 180;

        //Su che asse?
        if (Mathf.Abs(info.Direction.x) > Mathf.Abs(info.Direction.y))
        {
            fb_rot.AnimateZ = true;
            fb_rot.AnimateX = false;
        }
        else
        {
            fb_rot.AnimateX = true;
            fb_rot.AnimateZ = false;
        }


        if(info.LastTransition)
            fb_gameWonEvent.Active = true;
        else
            fb_gameWonEvent.Active = false;


        fb_Player_move.PlayFeedbacks(); 
    }
    #endregion
    
    #region ViewTransitionFeedback
    //FIXME: Mi finirai?
    //Spoiler: no. Metodo non finito di essere implementato, puntava a essere un feedback per rendere la transizione tra ViewUi più smooth
    public void PlayFeedbackViewTransition(GameObject target)
    {
        MMF_Position fb_pos = fb_Player_move.GetFeedbackOfType<MMF_Position>();

        fb_pos.AnimatePositionTarget = target;

        fb_Player_ViewTransition.PlayFeedbacks();
    }
    #endregion
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
