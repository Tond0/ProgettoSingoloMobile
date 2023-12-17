using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelScriptable[] levels;
    public LevelScriptable[] Levels { get => levels; }

    #region Instance
    public static GameManager current;
    private void Awake()
    {
        if (current)
            Destroy(this);
        else
            current = this;
    }
    #endregion

    public static Action OnGameStarted;
    public static Action<LevelScriptable> OnLevelSelected;
    public static Action OnLevelStarted;


    //FIXME: Ricordati di resettare la variabile.
    private int moves;
    public int Moves { get => moves; }
    public void AddMove() => moves++;
    public void RemoveMove() => moves--;

    private void OnEnable()
    {
        OnGameStarted?.Invoke();
    }

    public void LoadLevel(int ID)
    {
        LevelScriptable levelToLoad = levels[ID];

        OnLevelSelected?.Invoke(levelToLoad);
    }
}
