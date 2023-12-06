using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    private LevelScriptable loadedLevel;
    private void OnEnable()
    {
        GameManager.OnLevelSelected += DrawGrid;
        InputHandler.OnDrag += Move;
    }

    private void OnDisable()
    {
        GameManager.OnLevelSelected -= DrawGrid;
        InputHandler.OnDrag -= Move;
    }

    //Funzione che OnGameStarted creerà la griglia instanziando gli oggetti necessari
    private void DrawGrid(LevelScriptable level)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (level.LevelGrid[i, j].PieceInfo == null) continue;

                Debug.Log(i * 4 + j);

                Vector3 worldPos = new(j, 0, -i);
                Vector3 cellPos = GetCellCenter(worldPos);

                Instantiate(level.LevelGrid[i, j].PieceInfo.prefab, cellPos, Quaternion.identity, this.transform);
            }
        }

        loadedLevel = level;
    }

    //FIXME: Se viene usata al di fuori di questa classe considerare di renderla statica.
    private Vector3 GetCellCenter(Vector3 worldPos)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        Vector3 cellCenterPos = tilemap.GetCellCenterWorld(cellPos);

        return cellCenterPos;
    }

    private void Move(Vector3 startPos, Vector2 direction)
    {
        Vector3Int cellPos = tilemap.WorldToCell(startPos);
        Debug.Log(cellPos);
        if(cellPos.x > 4 || cellPos.x < 0 || cellPos.z > 4 || cellPos.z < 0) return;

        if (loadedLevel.LevelGrid[cellPos.x, cellPos.z].PieceInfo == null)
            Debug.Log("nothing");
        else
            Debug.Log(loadedLevel.LevelGrid[cellPos.x, cellPos.z].PieceInfo);
    }
}
