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
    public static Action OnLevelEnded;


    //FIXME: Ricordati di resettare la variabile.
    private int moves;
    public int Moves { get => moves; }
    public void UpdateMoveCount(int amount) => moves += amount;

    private void OnEnable()
    {
        Drag.OnMoveFinished += UpdateMoveCount;
    }

    private void OnDisable()
    {
        Drag.OnMoveFinished -= UpdateMoveCount;
    }

    private void Start()
    {
        OnGameStarted.Invoke();
    }

    public void LoadLevel(int ID)
    {
        Debug.Log("passato");
        LevelScriptable levelToLoad = levels[ID];
        OnLevelSelected?.Invoke(levelToLoad);
    }

    public void EndLevel() => OnLevelEnded.Invoke();
}
