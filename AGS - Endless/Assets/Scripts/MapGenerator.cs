using UnityEngine;
using System.Collections;
using UnityEditor;
public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public bool creating = true;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnDrawGizmos()
    {
        if (Selection.Contains(this.gameObject))
        {
            float x = transform.position.x;
            float z = transform.position.z;
            Gizmos.DrawLine(new Vector3(height / 2 + x, 0.1f, width / 2 + z), new Vector3(height / 2 + x, 0.1f, -width / 2 + z));
            Gizmos.DrawLine(new Vector3(height / 2 + x, 0.1f, width / 2 + z), new Vector3(-height / 2 + x, 0.1f, width / 2 + z));
            Gizmos.DrawLine(new Vector3(-height / 2 + x, 0.1f, -width / 2 + z), new Vector3(height / 2 + x, 0.1f, -width / 2 + z));
            Gizmos.DrawLine(new Vector3(-height / 2 + x, 0.1f, -width / 2 + z), new Vector3(-height / 2 + x, 0.1f, width / 2 + z));
        }
       
    }
}
