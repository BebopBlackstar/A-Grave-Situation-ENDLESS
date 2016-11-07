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

    void Start()
    {
        int tempLayer = ~(1 << LayerMask.NameToLayer("Walls"));
        int othertemp = ~(1 << LayerMask.NameToLayer("graveHit"));
        walls = tempLayer;
        gravehit = othertemp;
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
                    if (target.GetComponent<Seeable>().Seen("diggable") && guard.foundGrave == false)
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
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
