using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bug : MonoBehaviour
{
    public bugState state = bugState.notEaten;
}
public enum bugState
{
    eatenAfterCP,
    eatenBeforeCP,
    notEaten,
}
