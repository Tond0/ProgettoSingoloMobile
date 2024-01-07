using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FIXME: Reso conto ora che lo UIManager è stato praticamente diviso in due con il LevelFormSpawner.
public class UIManager : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] private View mainMenu;
    [SerializeField] private View levelSelection;
    [SerializeField] private View game;
    public View GameView { get => game; }
    [SerializeField] private View winView;
    [HeaderAttribute("Current one / First one")]
    [SerializeField] private View currentView;

    #region Instance
    public static UIManager current;
    private void Awake()
    {
        if (current)
            Destroy(this);
        else
            current = this;
    }
    #endregion

    private void OnEnable()
    {
        GameManager.OnLevelEnded += () => SwitchView(winView);
    }

    private void OnDisable() {
        
        GameManager.OnLevelEnded -= () => SwitchView(winView);
    }

    public void SwitchView(View nextView)
    {
        currentView.gameObject.SetActive(false);

        currentView = nextView;

        currentView.gameObject.SetActive(true);
    }
}
