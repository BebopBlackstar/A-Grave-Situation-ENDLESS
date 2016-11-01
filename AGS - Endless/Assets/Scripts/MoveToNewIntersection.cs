using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityStandardAssets.ImageEffects;
public class MoveToNewIntersection : MonoBehaviour
{
    [Tooltip("Player in scene")]
    public Transform Player;
    [Tooltip("The empty of the Waypoint Markers")]
    public GameObject markerContainer;
    [Tooltip("The layer of Walls")]
    public LayerMask Walls;
    [Tooltip("The move speed of the agent when he has found something")]
    public float findMoveSpeed = 3;
    [Tooltip("How long the guard will stay at the coin in seconds")]
    public float coinStayTime = 5;
    [Tooltip("Ignore")]
    public Pathing currentPathing;
    [Tooltip("How fast the time scale reduces")]
    public float fadeSpeed = .01f;
    [Tooltip("How fast the blur increses")]
    public float blurSpeed = .01f;
    private NavMeshAgent m_agent;
    private List<Transform> m_markers;
    private fieldOfView m_fieldOfView;
    private float normalMoveSpeed;
    private int currentPath;
    private List<Transform> m_searchMarkers = new List<Transform>();
    private int searchPath;
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        var markers = markerContainer.GetComponentsInChildren<Transform>();
        m_fieldOfView = GetComponent<fieldOfView>();
        m_markers = markers.ToList();
        m_markers.RemoveAt(0);
        currentPathing = new follow(m_agent);
        normalMoveSpeed = m_agent.speed;
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
    public void followPath()
    {
        if (m_searchMarkers.Count > 0)
        {
            m_agent.destination = m_searchMarkers[searchPath % m_searchMarkers.Count].position;
            searchPath++;
            if (searchPath >= m_searchMarkers.Count)
            {
                m_searchMarkers = new List<Transform>();
                m_agent.speed = normalMoveSpeed;
            }
        }
        else
        {
            m_agent.destination = m_markers[currentPath % m_markers.Count].position;
            currentPath++;
        }
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
        foreach (var marker in possibleNodes)
        {
            if (i >= percent)
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
        m_agent.speed = findMoveSpeed;
        foreach (var point in m_markers)
            if (Vector3.Distance(transform.position, point.position) <= GetComponent<fieldOfView>().MarkerRadius)
                m_searchMarkers.Add(point.transform);

    }
    IEnumerator StayAtCoin(float sec)
    {
        yield return new WaitForSeconds(sec);
        coin current = currentPathing as coin;
        Destroy(current.m_coin.gameObject);
        currentPathing = new follow(m_agent);
        currentPathing.followPath();
    }
    IEnumerator playerFoundFade(float FadeTick)
    {
        for (;;)
        {
            if (Time.timeScale >= FadeTick)
            {
                Time.timeScale -= FadeTick;
                Debug.Log(Time.timeScale);
                BlurOptimized main = Camera.main.GetComponent<BlurOptimized>();
                main.blurSize += blurSpeed;
                //main.blurIterations = (int)Mathf.Floor(main.blurSize);
                yield return new WaitForSeconds(.01f);
            }
            else
            {
                Time.timeScale = 0;
                Debug.Log(Time.timeScale);
                break;
            }
        }
    }
    public void FoundPlayer()
    {
        if (Player.GetComponent<PlayerCont>().moveSpeed > 0)
        {
            Player.GetComponent<PlayerCont>().moveSpeed = 0;
            BlurOptimized blur = Camera.main.gameObject.GetComponent<BlurOptimized>();
            blur.enabled = true;
            StartCoroutine(playerFoundFade(fadeSpeed));
        }
        
        currentPathing = new follow(m_agent);
        m_agent.destination = Player.position;
        m_agent.speed = findMoveSpeed;
        m_agent.angularSpeed = 500;
        m_agent.acceleration = 500;
    }
    public void FoundCoin(Transform coin)
    {
        m_agent.destination = coin.position;
        coin.GetComponent<CoinGrab>().grabbed = true;
        currentPathing = new coin(m_agent, coin);

        StartCoroutine(StayAtCoin(coinStayTime));
    }
    void Update()
    {
        m_fieldOfView.Find();
        m_fieldOfView.DrawFieldOfView();
        currentPathing.followPath();
    }
}
public class Pathing
{

    protected NavMeshAgent m_agent;
    public Pathing(NavMeshAgent agent)
    {
        m_agent = agent;
    }
    public virtual void followPath()
    {
        m_agent.GetComponent<MoveToNewIntersection>().followPath();
    }
}
public class follow : Pathing
{

    public follow(NavMeshAgent agent) : base(agent)
    {

    }
    public override void followPath()
    {
        if (m_agent.remainingDistance < 1)
        {
            base.followPath();
        }
    }
}
public class coin : Pathing
{
    public Transform m_coin;
    public coin(NavMeshAgent agent, Transform _coin) : base(agent)
    {
        m_coin = _coin;
    }
    public override void followPath()
    {
        m_agent.destination = m_coin.position;
    }
}