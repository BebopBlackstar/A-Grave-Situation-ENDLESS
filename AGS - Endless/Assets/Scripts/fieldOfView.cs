using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class fieldOfView : MonoBehaviour
{
    [Tooltip("Detail of the FOV")]
    public int MeshResolution;
    [Tooltip("How far out the view will go")]
    public float viewRadius;    
    [Range(0, 360), Tooltip("Angle that guard will see anything")]
    public float viewAngle;
    [Tooltip("Wall layer")]
    public LayerMask walls;
    [Tooltip("How far out he will see empty graves")]
    public float GraveRadius = 1;
    [Tooltip("How far out he will find markers")]
    public float MarkerRadius = 10;
    [Tooltip("The mesh child to the guard")]
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    void Start()
    {

        viewMesh = new Mesh();
        viewMesh.name = "view Mesh";
        viewMeshFilter.mesh = viewMesh;

    }
    public void Find()
    {

        var Targets = Physics.OverlapSphere(transform.position, viewRadius);
        foreach (var target in Targets)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                RaycastHit hit;
                Physics.Linecast(transform.position, target.transform.position, out hit);
                Physics.Raycast(new Ray(transform.position, dirToTarget), out hit, viewRadius);
                var guard = GetComponentInParent<MoveToNewIntersection>();
                if (target.GetComponent<Seeable>() != null && hit.transform.tag ==  target.tag)
                    if (target.GetComponent<Seeable>().Seen() && target.tag == "diggable" && Physics.Raycast(new Ray(transform.position, dirToTarget), GraveRadius, walls))
                    {
                        target.GetComponent<Seeable>().alreadySeen = true;
                        guard.FoundEmptyGrave(target.gameObject);
                    }
                    else if (hit.transform.tag == "Player" && target.tag == "Player")
                    {
                        target.GetComponent<Seeable>().Seen();
                        target.GetComponent<Seeable>().alreadySeen = true;
                        guard.FoundPlayer();
                    }
                    else if (target.GetComponent<Seeable>().Seen() && target.tag == "coin" && guard.currentPathing is follow)
                    {
                        target.GetComponent<Seeable>().alreadySeen = true;
                        guard.FoundCoin(target.transform);
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
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, ~walls))
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