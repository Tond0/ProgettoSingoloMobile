using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Informazioni del pezzo stesso.
//Non lo fa Tile? no
//La relazione tra Tile e Piece è del tipo: una Tile può contenere più Piece impilati

//Ma non facevi prima a farla Monobehaviour? si
//Ma se la faccio mono potrebbe succedere che ci si può dimenticare di assegnare lo script quando si crea
//un nuovo prefab di un pezzo, o il designer può dimenticarsi di assegnare qualche variabile (GoTo in questo caso).
//Classe chiusa antidesigner.

public class Piece
{
    private GameObject _gameObject;
    public GameObject gameObject { get => _gameObject; }

    private Transform _transform;
    public Transform transform { get => _transform; }

    private Transform goTo;
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

    public Piece(GameObject gameObject)
    {
        this._gameObject = gameObject;
        _transform = gameObject.transform;
        goTo = gameObject.GetComponentInChildren<GoTo>().transform;
    }
}