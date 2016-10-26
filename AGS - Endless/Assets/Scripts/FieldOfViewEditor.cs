using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(fieldOfView))]
public class FieldOfViewEditor : Editor {
    void OnSceneGUI()
    {
        fieldOfView fow = (fieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.GraveRadius);
        Vector3 viewangleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewangleB = fow.DirFromAngle(fow.viewAngle / 2, false);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewangleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewangleB * fow.viewRadius);
    }
}
