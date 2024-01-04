using System;
using System.Collections;
using UnityEngine;

public enum PieceType { Bread, Topping }

[CreateAssetMenu(menuName = "Scriptable/SandwichPiece", fileName = "SandwichPieceScriptable", order = 0)]
public class SandwichPieceScriptable : ScriptableObject
{
    //Il prefab che verrà instanziato
    public GameObject prefab;
    
    //Il tipo di panino
    public PieceType type;

}
