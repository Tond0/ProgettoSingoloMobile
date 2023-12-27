using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Racchiuse qui dentro tutte le interfacce necessarie per il command pattern.
public enum CommandStatus { Success, Failure, InProgress }
public interface Command
{
    public CommandStatus Execute(Receiver receiver);
    public void Undo(Receiver receiver);
    public void Redo(Receiver receiver);
}


//Tag abstract class.
//FIXME: Ha senso?
public abstract class Receiver : MonoBehaviour { }


public interface Provider
{
    public Command GetCommand();
}
