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
    int walls;
    [Tooltip("How close to guard until he will find without fov")]
    public float awarenessDistance;
    [Tooltip("Layer of grave hitboxes")]
    int gravehit;
    [Tooltip("How far out he will see empty graves")]
    public float GraveRadius = 1;
    [Tooltip("How far out he will find markers")]
    public float MarkerRadius = 10;
    [Tooltip("The mesh child to the guard")]
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    void Start()
    {
        int tempLayer = ~(1 << LayerMask.NameToLayer("Walls"));
        int othertemp = ~(1 << LayerMask.NameToLayer("graveHit"));
        walls = tempLayer;
        gravehit = othertemp;
        viewMesh = new Mesh();
        viewMesh.name = "view Mesh";
        viewMeshFilter.mesh = viewMesh;
    }
    public void Find()
    {

        var Targets = Physics.OverlapSphere(transform.position, viewRadius, walls);
        foreach (var target in Targets)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2 || Vector3.Distance(transform.position, target.transform.position) <= awarenessDistance)
            {
                RaycastHit hit;

                var guard = GetComponentInParent<MoveToNewIntersection>();
                if (target.GetComponent<Seeable>() != null)
                    if (target.GetComponent<Seeable>().Seen(target.tag) && guard.foundGrave == false)
                    {
                        var Targets1 = Physics.OverlapSphere(transform.position, GraveRadius, walls);
                        foreach (var target1 in Targets1)
                        {
                            if (target.gameObject == target1.gameObject)
                            {
                                target.GetComponent<Seeable>().alreadySeen = true;
                                guard.FoundEmptyGrave(target.gameObject);
                            }
                        }
                    }
                    else if (target.GetComponent<Seeable>().Seen("coin") && guard.currentPathing is follow)
                    {
                        target.GetComponent<Seeable>().alreadySeen = true;
                        guard.FoundCoin(target.transform);
                    }
                    else if (Physics.Linecast(transform.position, target.transform.position, out hit, ~(1 << LayerMask.NameToLayer("Walls") | 1 << LayerMask.NameToLayer("graveHit"))) && target.tag == "Player")
                    {
                        if (target.GetComponent<Seeable>().Seen(hit.transform.tag))
                        {
                            target.GetComponent<Seeable>().alreadySeen = true;
                            guard.FoundPlayer();
                        }
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

        int layer = ~(1 << LayerMask.NameToLayer("Walls") | 1 << LayerMask.NameToLayer("graveHit"));

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, layer))
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