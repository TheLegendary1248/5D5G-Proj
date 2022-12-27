using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFileControl : MonoBehaviour
{
    static void LoadLevel() { }
    void Test()
    {
        
    }
}
interface ICustomSerialize
{
    byte[] Serialize();
    void Deserialize(in GameObject parent);
}
/// <summary>
/// Apply to member that can be read by the in-game level editor. 
/// </summary>
[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
sealed class LevelEditorAttribute : System.Attribute
{
    // This is a named argument
    public string Description { get; set; }
    public string TrueName { get; set; }
}
/// <summary>
/// Defines a class as an addable component in the level editor
/// </summary>
sealed class AddableComponentAttribute : System.Attribute
{

}