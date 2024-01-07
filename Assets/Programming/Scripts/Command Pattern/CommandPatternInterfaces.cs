using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Racchiuse qui dentro tutte le interfacce necessarie per il command pattern.
public enum CommandStatus { Success, Failure, InProgress }
public interface ICommand
{
    public CommandStatus Execute(CommandReceiver receiver);
    public void Undo(CommandReceiver receiver);
    public void Redo(CommandReceiver receiver);
}


//Tag abstract class.
public abstract class CommandReceiver : MonoBehaviour { }


public abstract class CommandProvider : MonoBehaviour
{
    public abstract ICommand GetCommand();
}
