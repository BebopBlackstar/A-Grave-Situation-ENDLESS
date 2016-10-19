using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class fieldOfView : MonoBehaviour
{
    public int MeshResolution;
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask walls;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    void Start()
    {

        viewMesh = new Mesh();
        viewMesh.name = "view Mesh";
        viewMeshFilter.mesh = viewMesh;

    }
    public void FindPlayer()
    {

        var Targets = Physics.OverlapSphere(transform.position, viewRadius);
        foreach (var target in Targets)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                if (target.GetComponent<Seeable>() != null)
                    if (target.GetComponent<Seeable>().Seen() && target.tag != "Player")
                    {
                        GetComponentInParent<MoveToNewIntersection>().FoundEmptyGrave(target.gameObject);
                    }
                    else
                    {
                        GetComponentInParent<MoveToNewIntersection>().FoundPlayer();
                    }
            }
        }
    }
    public void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * MeshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, walls))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }

    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

public struct ViewCastInfo
{
    public bool hit;
    public Vector3 point;
    public float distance;
    public float angle;
    public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
    {
        hit = _hit;
        point = _point;
        distance = _dist;
        angle = _angle;
    }
}