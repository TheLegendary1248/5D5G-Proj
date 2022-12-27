using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(WayPath)), CanEditMultipleObjects]
public class WayPathHelper : Editor
{

    private void OnSceneGUI()
    {
        WayPath script = (WayPath)target;
        WayPath.Waypoints[] points = script.points.ToArray();
        if(!Application.isPlaying)
        {
            EditorGUI.BeginChangeCheck();
            int i = 0; Vector2 pt; Vector2 usedPT = new Vector3(); int usedIndex = 0;
            while (i < points.Length)
            {
                Vector2 pos = Handles.DoPositionHandle(script.Relative(pt = points[i]), Quaternion.identity);
                if (script.transform.parent != null) pos = script.transform.parent.InverseTransformPoint(pos);
                if (pt != pos) { usedPT = pos; usedIndex = i; }
                i++;
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(script, "Move waypoint");
                WayPath.Waypoints newPoint = points[usedIndex]; newPoint.point = usedPT;
                script.points[usedIndex] = newPoint;
            }
        }
        
        if (!script.showGizmo ? !script.showGizmo : points.Length < 2) 
            return;
        for (int l = 0; l < points.Length; l++)
        {
            Handles.Label(script.Relative(points[l].point), points[l].waitTime.ToString());
        }
        for (int k = 0; k < points.Length - 1; k++)
        {
            Handles.Label(script.Relative((points[k].point + points[k + 1].point) / 2), points[k].time.ToString());
        }
        if (script.isLoop) { Handles.Label(script.Relative((points[0].point + points[points.Length - 1].point) / 2), points[points.Length - 1].time.ToString()); }
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WayPath script = (WayPath)target;
        GUILayout.Label($"Total Animation Time : {script.TotalAnimTime}");
        if (!script.isLoop) GUILayout.Label("Last Waypoint's time will not be used in an unlooped cycle");
        if (GUILayout.Button("Add new point at current position", GUILayout.Height(40)))
        {        
            Vector2 pos = script.transform.localPosition;
            SerializedProperty array = serializedObject.FindProperty("points");
            array.InsertArrayElementAtIndex(script.points.Count);
            SerializedProperty point = array.GetArrayElementAtIndex(script.points.Count);
            SerializedProperty vec = point.FindPropertyRelative("point");
            vec.vector2Value = pos;
            SerializedProperty time = point.FindPropertyRelative("time");
            time.floatValue = script.points.Count == 0 ? 
                1f
                :
                Vector3.Distance(pos, script.points[script.points.Count - 1].point);
            serializedObject.ApplyModifiedProperties();
        }
        if (GUILayout.Button("Place Object At Starting Point", GUILayout.Height(40)))
        {
            if (script.points.Count != 0)
            {
                Undo.RecordObject(script.transform, $"Reset to start position of waypoints{script.name}");
                script.transform.position = script.GetLocationAtTime(script.timeOffset);
            }
            else Debug.LogWarning("No points are set");
        }
        if(GUILayout.Button("Fix time of first waypoint"))
        {
            if (script.points.Count <= 1) Debug.LogWarning("Not enough waypoints to calculate");
            else
            {
                float time = Vector2.Distance(script.points[0],script.points[script.points.Count-1]);
                serializedObject.FindProperty("points").GetArrayElementAtIndex(0).FindPropertyRelative("time").floatValue = time;
                serializedObject.ApplyModifiedProperties();
            }
        }
        if (GUILayout.Button("Clear Points"))
        {
            serializedObject.FindProperty("points").ClearArray();
            serializedObject.ApplyModifiedProperties();
        }
        
    }
}
