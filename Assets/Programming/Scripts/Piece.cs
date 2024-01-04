using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Informazioni del pezzo stesso.
//Non lo fa Tile? no
//La relazione tra Tile e Piece è del tipo: una Tile può contenere più Piece impilati

//Ma non facevi prima a farla Monobehaviour? si
//Ma se la faccio mono potrebbe succedere che ci si può dimenticare di assegnare lo script quando si crea
//un nuovo prefab di un pezzo, o il designer può dimenticarsi di assegnare qualche variabile (GoTo in questo caso).
//Classe chiusa antidesigner.

public class Piece : MonoBehaviour
{
    [SerializeField] private Transform goTo;

    [SerializeField] private TextMeshProUGUI movesToMoveTxt;

    private int movesToMove;
    private bool justOnce; //In modo tale che anche essendo public, movesToMove sarà assegnabile soltanto alla creazione di questa istanza.
    public int MovesToMove
    {
        set
        {
            if (justOnce) return;

            justOnce = true;
            movesToMove = value;
            SetText(value);
        }
    }

    private void OnEnable()
    {
        Drag.OnMoveFinished += UpdateMovesToMove;
    }

    private void OnDisable()
    {
        Drag.OnMoveFinished -= UpdateMovesToMove;
    }

    public void UpdateMovesToMove(int move)
    {
        movesToMove -= move;
        SetText(movesToMove);
    }

    private void SetText(int moves)
    {
        if (moves <= 0)
            movesToMoveTxt.text = "";
        else
            movesToMoveTxt.text = moves.ToString();

    }

    public Vector3 GoTo
    {
        get
        {
            //Se è negativa (il pezzo è girato a faccia in giù)...
            if (goTo.position.y < transform.position.y)
            {

                //Calcoliamo la posizione opposta che risulta essere quella sopra al panino
                Vector3 pos = new Vector3(goTo.position.x, transform.position.y + Mathf.Abs(goTo.localPosition.y), goTo.position.z);
                return pos;
            }

            return goTo.position;
        }
    }
}