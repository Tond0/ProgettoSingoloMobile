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


    //Command history
    private List<ICommand> history = new();
    private int historyIndex = 0;


    private void OnEnable()
    {
        undoBtn.onClick.AddListener(HandleUndo);
        redoBtn.onClick.AddListener(HandleRedo);

        GameManager.OnLevelEnded += ResetHistory;
    }

    private void OnDisable()
    {
        undoBtn.onClick.RemoveListener(HandleUndo);
        redoBtn.onClick.RemoveListener(HandleRedo);

        GameManager.OnLevelEnded -= ResetHistory;
    }


    private void ResetHistory()
    {
        history.Clear();
        historyIndex = 0;
    }

    #region Reset
    private Coroutine resetCoroutine;
    public void ResetLevel()
    {
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        resetCoroutine = StartCoroutine(ResetLevelCoroutine());
    }
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

    private void HandleUndo()
    {
        if (historyIndex - 1 < -1 || history.Count <= 0) return;

        history[historyIndex]?.Undo(receiver);
        historyIndex -= 1;
    }

    private void HandleRedo()
    {
        if (historyIndex + 1 > history.Count - 1) return;

        historyIndex += 1;
        history[historyIndex]?.Redo(receiver);
    }


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
}
