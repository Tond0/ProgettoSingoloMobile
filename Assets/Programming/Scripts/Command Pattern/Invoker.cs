using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Un invoker "lite" siccome fare un vero command pattern con classi / interfacce comando sarebbe stato overkill
//sopratutto contando che l'unica cosa che può fare il player è lo spostamento di celle.
//Di conseguenza non andrò nemmeno a fare un'interfaccia per "l'actor" siccome sono sicuro che sarà sempre gridManager.
public class Invoker : MonoBehaviour
{
    [Header("Invoker settings")]
    [HeaderAttribute("Input provider")]
    [SerializeField] private CommandProvider activeInputProvider;
    [SerializeField] private Button undoBtn;
    [SerializeField] private Button redoBtn;

    [HeaderAttribute("Input receiver")]
    [SerializeField] private CommandReceiver receiver;


    //Command history, dove verranno salvati tutti i comandi effettuati
    private List<ICommand> history = new();
    //L'indice che indicherà in che parte della history siamo (Mossa corrente, Mossa precendente (Undo), Mossa successiva (Redo))
    private int historyIndex = 0;

    //Allacciamo tutti le actions
    private void OnEnable()
    {
        undoBtn.onClick.AddListener(HandleUndo);
        redoBtn.onClick.AddListener(HandleRedo);

        GameManager.OnLevelEnded += ClearHistory;
    }

    private void OnDisable()
    {
        undoBtn.onClick.RemoveListener(HandleUndo);
        redoBtn.onClick.RemoveListener(HandleRedo);

        GameManager.OnLevelEnded -= ClearHistory;
    }

    /// <summary>
    /// Metodo per resettare la History e il suo index.
    /// </summary>
    private void ClearHistory()
    {
        history.Clear();
        historyIndex = 0;
    }

    #region Reset moves
    private Coroutine resetCoroutine;

    //Funzione che verrà chiamata dal bottone in gioco Reset, con il compito di far incominciare una coroutine
    public void ResetLevel()
    {
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(ResetLevelCoroutine());
    }

    //Il motivo per il quale è una coroutine è legato all'implementazione grafica dell'AddOn Feel. (vedi riga 74 per spiegazione)
    private IEnumerator ResetLevelCoroutine()
    {
        while (historyIndex >= 0)
        {
            history[historyIndex].Undo(receiver);
            historyIndex--;

            //Skip di 3 frame per non buggare l'animazione.
            //vedi il metodo MMF_Player.SkipToEnd() che dice che può richiedere fino a 3 frame per il suo completamento.

            yield return null;
            yield return null;
            yield return null;

        }
    }
    #endregion

    #region Undo
    private void HandleUndo()
    {
        //Se è presente una mossa prima della corrente e history non è 0 (è stata effettuata almeno una mossa)
        if (historyIndex - 1 < -1 || history.Count <= 0) return;

        history[historyIndex]?.Undo(receiver);

        //Muoviamo l'index indietro di un posto per indicare la mossa precedente a quella corrente.
        historyIndex -= 1;
    }
    #endregion

    #region Redo
    private void HandleRedo()
    {
        //Se è presente una mossa successiva a quella puntata e non usciamo fuori dalla dimensione corrente della history...
        if (historyIndex + 1 > history.Count - 1) return;

        //Aggiorniamo l'index che punta a che punto della cronologia siamo
        historyIndex += 1;
        history[historyIndex]?.Redo(receiver);
    }
    #endregion

    #region Get & Execute commands
    ICommand currentCommand;
    private void Update()
    {
        if (currentCommand == null)
        {
            currentCommand = activeInputProvider.GetCommand();
        }
        else
        {
            switch (currentCommand.Execute(receiver))
            {
                case CommandStatus.Success:

                    if (historyIndex + 1 <= history.Count - 1)
                    {
                        int commandsToRemove = (history.Count - 1) - historyIndex;
                        history.RemoveRange(historyIndex + 1, commandsToRemove);
                    }

                    history.Add(currentCommand);
                    historyIndex = history.Count - 1;

                    currentCommand = null;
                    break;

                case CommandStatus.Failure:
                    currentCommand = null;
                    break;

                case CommandStatus.InProgress:
                    return;
            }
        }
    }
    #endregion

    #region Debug
    //FIXME: per debug da rimuovere.
    public static void DebugList(List<ICommand> list)
    {
        if (list.Count <= 0) return;

        Debug.Log(" ");
        foreach (ICommand item in list)
        {
            Debug.Log(item);
        }
        Debug.Log(" ");
    }
    #endregion
}
