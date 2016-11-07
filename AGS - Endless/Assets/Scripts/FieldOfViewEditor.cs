using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(fieldOfView))]
public class FieldOfViewEditor : Editor {
    void OnSceneGUI()
    {
        fieldOfView fow = (fieldOfView)target;
        Handles.color = Color.green;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
        Handles.color = Color.yellow;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.GraveRadius);
        Handles.color = Color.red;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.awarenessDistance);
        Handles.color = Color.blue;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.MarkerRadius);
        Vector3 viewangleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewangleB = fow.DirFromAngle(fow.viewAngle / 2, false);
        Handles.color = Color.white;
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewangleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewangleB * fow.viewRadius);
    }
}
