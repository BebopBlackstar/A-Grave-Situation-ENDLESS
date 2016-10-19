using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class MoveToNewIntersection : MonoBehaviour
{
    public Transform Player;
    private NavMeshAgent m_agent;
    public GameObject markerContainer;
    private List<Transform> m_markers;
    private fieldOfView m_fieldOfView;
    public float weightingFactor;
    public LayerMask Walls;
    private int currentPath;
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        var markers = markerContainer.GetComponentsInChildren<Transform>();
        m_fieldOfView = GetComponent<fieldOfView>();
        m_markers = markers.ToList();
        m_markers.RemoveAt(0);
    }
    float calculatePathLength(Vector3 startPos, Vector3 endPos)
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, path);
        if (path.corners.Length < 2)
        {
            return 0;
        }
        Vector3 previousCorner = path.corners[0];
        float lengthSoFar = .0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 currentCorner = path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
        }
        return lengthSoFar;
    }
    void newPath()
    {
        Vector3 closest = new Vector3();
        float closestDistance = Mathf.Infinity;
        foreach (var marker in m_markers)
        {
            if (Vector3.Distance(marker.position, m_agent.destination) > 1)
            {
                float calculatedPathLength = calculatePathLength(transform.position, marker.position);
                float distanceFromPlayer = calculatePathLength(marker.position, Player.position);
                float weightedDistance = calculatedPathLength + distanceFromPlayer;
                if (weightedDistance < closestDistance)
                {
                    bool hit = Physics.Linecast(transform.position, marker.position, Walls.value);
                    if (!hit)
                    {
                        closest = marker.position;
                        closestDistance = weightedDistance;
                    }
                }
            }
        }
        m_agent.destination = closest;
    }
    void followPath()
    {
        m_agent.destination = m_markers[currentPath % m_markers.Count].position;
        currentPath++;
    }
    void newWeightedPath()
    {
        List<KeyValuePair<Vector3, float>> possibleNodes = new List<KeyValuePair<Vector3, float>>();
        float completeWeight = 0;
        foreach (var marker in m_markers)
        {
            bool hit = Physics.Linecast(transform.position, marker.position, Walls.value);
            if (!hit)
            {
                float weight = Vector3.Distance(transform.position, marker.position) + calculatePathLength(marker.position, Player.position);
                possibleNodes.Add(new KeyValuePair<Vector3, float>(marker.position, weight));
                completeWeight += weight;
            }
        }
        possibleNodes.Sort((x, y) => y.Value.CompareTo(x.Value));
        float percent = Random.Range(0, completeWeight);
        float i = 0;
        Vector3 target = new Vector3();
        foreach(var marker in possibleNodes)
        {
            if(i >= percent)
            {
                m_agent.destination = target;
                break;
            }
            target = marker.Key;
            i += marker.Value;
        }
        //float distanceToPlayer = calculatePathLength(transform.position, Player.position);
        //float chanceToPlayer = Random.Range(0, distanceToPlayer) / weightingFactor * 100;
        //int thing = (int)Mathf.Round(chanceToPlayer) % possibleNodes.Count;
        //Debug.Log(thing);
        //m_agent.destination = possibleNodes[(int)Random.Range(0, chanceToPlayer % possibleNodes.Count)].Key;

    }
    public void FoundEmptyGrave(GameObject grave)
    {
        Debug.Log("found the grave and its super empty");
    }
    public void FoundPlayer()
    {
        m_agent.destination = Player.position;
        m_agent.speed = 5;
        m_agent.angularSpeed = 500;
        m_agent.acceleration = 500;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("mkay");

        m_fieldOfView.FindPlayer();
        m_fieldOfView.DrawFieldOfView();
        Debug.DrawLine(m_agent.GetComponent<Transform>().position, m_agent.destination);
        if (m_agent.remainingDistance < 1)
        {
            followPath(); //will follow path 1 through n
            //newPath(); //will path directly to player quieckest way
            //newWeightedPath(); //will go towards player if further away(not functioning)
        }

    }
}
