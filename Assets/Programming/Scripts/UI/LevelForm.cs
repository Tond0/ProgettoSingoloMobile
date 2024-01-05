using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelForm : MonoBehaviour
{
    //Che livello dovrà caricare?
    [NonSerialized] public int ID;
    [SerializeField] private TextMeshProUGUI TMP;
    [SerializeField] private Button button;
    private void Start()
    {
        //Mi da fastidio vedere scritto "Livello 0"
        TMP.text = (ID + 1).ToString();
    }

    public void Link()
    {
        button.onClick.AddListener(() => GameManager.current.LoadLevel(ID));

        button.onClick.AddListener(() => UIManager.current.SwitchView(UIManager.current.GameView));
    }
}
