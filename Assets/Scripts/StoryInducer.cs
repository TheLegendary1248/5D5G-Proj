using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StoryInducer : MonoBehaviour
{
    public Bounds box;
    Color color;
    TextMesh tex;
    bool activated = false;
    private void Start()
    {
        tex = GetComponent<TextMesh>();
        color = tex.color;
        tex.color = Color.clear;
    }
    private void FixedUpdate()
    {
        if(Player.self & !activated)
        {
            if (box.Contains(Player.self.transform.position)) StartCoroutine(AnimColor());
        }
    }

    IEnumerator AnimColor()
    {
        float stamp = Time.time + 1f;
        while(stamp > Time.time)
        {
            tex.color = Color.Lerp(color,Color.clear, stamp - Time.time);
            yield return new WaitForFixedUpdate();
        }
        tex.color = color;
        enabled = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0,0,1,0.5f);
        Gizmos.DrawWireCube(box.center, box.size);
    }
}
[CustomEditor(typeof(StoryInducer))]
public class StoryHelp : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Set detect bounds at current position"))
        {
            serializedObject.FindProperty("box").boundsValue = new Bounds(((StoryInducer)target).transform.position, Vector2.one * 5);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
