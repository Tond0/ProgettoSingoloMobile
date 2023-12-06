using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelFormSpawner : MonoBehaviour
{
    [SerializeField] private GameObject levelFormPrefab;
    void Start() 
    {
        for(int i = 0; i < GameManager.current.Levels.Length; i++)
        {
            LevelForm form = Instantiate(levelFormPrefab, this.transform).GetComponent<LevelForm>();
            form.ID = i;
        }
    }
}
