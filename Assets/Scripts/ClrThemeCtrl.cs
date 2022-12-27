using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClrThemeCtrl : MonoBehaviour
{
    /// <summary>
    /// Used for overral level collection theme
    /// </summary>
    public static ColorPts[] themeColors = null;
    /// <summary>
    /// Used for the particular level
    /// </summary>
    public static ColorPts[] levelOverride = null;
    public enum ColorModes
    {
        Analgous, 
        Complementary,
        Custom,
    }
    /// <summary>
    /// Used to describe key color points when syncronizing with music
    /// </summary>
    public struct ColorPts
    {
        /// <summary>
        /// Primary defines the primary color theme. Ground and Air are used respectively when Custom is used;
        /// </summary>
        Color Primary, Ground, Air;
    }
}
