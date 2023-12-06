using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Level", fileName = "Level x", order = 1)]
public class LevelScriptable : ScriptableObject
{
    static int width = 4;
    static int height = 4;

    //Purtroppo per utilizzare il custom inspector sono obbligato a usare array monodimensionale 
    //(non so se valesse la pena spendere ulteriore tempo per farlo andare con un array bidimensionale)
    [SerializeField, HideInInspector] private Tile[] levelArr = new Tile[width * height];

    //Griglia in cui poi convertirò l'array monodimensionale, per comodità e non portarmi dietro in ogni script la formula inversa di index = i * height + j 
    private Tile[,] levelGrid = new Tile[width, height];
    public Tile[,] LevelGrid { get => levelGrid; }

    private void OnEnable()
    {
        //Trasposizione dall'array alla griglia.
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                levelGrid[i, j] = levelArr[i * height + j];
            }
        }
    }

}

[CustomEditor(typeof(LevelScriptable))]
public class LevelScriptable_Editor : Editor
{
    SerializedProperty levelArr;

    //FIXME: C'è bisogno di ripeterli ancora?
    int width = 4;
    int height = 4;

    private void OnEnable()
    {
        levelArr = serializedObject.FindPropertyOrFail("levelArr");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Level creation");
        for (int i = 0; i < width; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < height; j++)
            {
                GUILayout.BeginVertical(i + " : " + j, "window");

                SerializedProperty SandwichPiece = levelArr.GetArrayElementAtIndex(i * height + j).FindPropertyRelative("pieceInfo");
                EditorGUILayout.PropertyField(SandwichPiece, GUIContent.none);

                GUILayout.BeginHorizontal();
                SerializedProperty movesToMove = levelArr.GetArrayElementAtIndex(i * height + j).FindPropertyRelative("movesToMove");

                float originalLabel = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 100;
                EditorGUILayout.PropertyField(movesToMove, new GUIContent("MovesToMove", "Dopo quanto potrà mosse potrà essere mosso?"));
                EditorGUIUtility.labelWidth = originalLabel;

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                GUILayout.Space(5);
            }
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

        }
        serializedObject.ApplyModifiedProperties();
    }
}
