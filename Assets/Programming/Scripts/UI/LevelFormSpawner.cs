using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Lo spawner delle opzioni di livelli disponibili
public class LevelFormSpawner : MonoBehaviour
{
    [SerializeField] private GameObject levelFormPrefab;
    [SerializeField] private Transform parent;
    private void OnEnable()
    {
        GameManager.OnGameStarted += SpawnLevelContainers;
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= SpawnLevelContainers;
    }
    private void SpawnLevelContainers()
    {
        for (int i = 0; i < GameManager.current.Levels.Length; i++)
        {
            LevelForm form = Instantiate(levelFormPrefab, parent).GetComponent<LevelForm>();
            form.ID = i;
            form.Link();
        }
    }
}
