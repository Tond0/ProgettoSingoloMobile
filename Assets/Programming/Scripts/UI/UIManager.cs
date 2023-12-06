using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] private View mainMenu;
    [SerializeField] private View levelSelection;
    [SerializeField] private View game;
    public View GameView { get => game; }
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

    public void SwitchView(View nextView)
    {
        currentView.gameObject.SetActive(false);

        currentView = nextView;

        currentView.gameObject.SetActive(true);
    }
}
