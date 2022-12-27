using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class contains all functionality to create triggers
/// </summary>
public class Triggerable : MonoBehaviour
{
    [SerializeField]
    int signal;
    /// <summary>
    /// The amount of points on triggerable
    /// </summary>
    public int Signal { get => Signal; }
    /// <summary>
    /// Will return true if signal is 0 or below
    /// </summary>
    public bool isTriggered { get; }
    /// <summary>
    /// Deducts an amount from 'Signal' and sets off the trigger if it goes to or below zero
    /// </summary>
    /// <param name="setTrigger"></param>
    public void Trigger(int setTrigger) { }
    /// <summary>
    /// Immediately activates/deactives the triggerable
    /// </summary>
    public void Toggle() { }


}
