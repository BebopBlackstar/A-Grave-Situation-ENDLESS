using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerCont : Seeable
{
    [Header("Player Setup")]
    [Tooltip("How fast the player will move")]
    public float moveSpeed = 10;
    [Tooltip("Speed when carrying body")]
    public float carrySpeed = 5;
    [Tooltip("Speed of the model turning, only for looks")]
    public float turnSpeed = .5f;
    [Tooltip("Current money")]
    public int moneh;
    [Tooltip("Money being carried on body")]
    public int carryMoneh;

    [Header("Coin Throwing")]
    private float coinLine = .11f;
    [Tooltip("Coin Prefab")]
    public GameObject coin;
    [Tooltip("The max force to throw the coin")]
    public float maxThrowForce = 30;
    [Tooltip("How High the coin will be thrown (higher for less)")]
    public float arkAmount = 2;
    [Tooltip("How fast you wind up the throw")]
    public float throwSpeed = 10;
    [Tooltip("How much moving changes the distance (higher for less)"), Range(.1f, 100)]
    public float moveThrow = 1;
    [Tooltip("How far away you can grab coins")]
    public float grabDistance = 2;
    private LineRenderer m_lr;
    private Transform m_camera;
    private bool carrying;
    private IEnumerator routine;
    private Collider triggerObject;
    private GameObject body;
    private IEnumerator lineDraw;
    Vector3 movement, moveDirection;

    private bool droppedThisFrame = true;
    private float timeHeld;
    void Start()
    {
        lineDraw = DrawLine();
        m_camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        body = GameObject.FindGameObjectWithTag("CarryBody");
        m_lr = GetComponentInChildren<LineRenderer>();
        m_lr.gameObject.SetActive(false);
        body.SetActive(false);
    }
    public void OnTriggerEnter(Collider other)
    {
        triggerObject = other;
    }
    public void OnTriggerExit(Collider other)
    {
        if (routine != null)
            StopCoroutine(routine);
        triggerObject = null;
        Camera.main.GetComponent<CameraFollow>().reset();
    }
    public void TriggerHandle()
    {
        if (Input.GetButtonDown("Jump") && triggerObject.tag == "diggable" && body.activeSelf == false)
        {
            routine = triggerObject.gameObject.GetComponent<diggable>().dig();
            StartCoroutine(routine);
        }
        else if (Input.GetButtonDown("Jump") && triggerObject.tag == "DropOff" && body.activeSelf == true)
        {
            moneh += carryMoneh;
            carryMoneh = 0;
            float temp = moveSpeed;
            moveSpeed = carrySpeed;
            carrySpeed = temp;
            body.SetActive(false);
        }
        if (Input.GetButtonUp("Jump") && routine != null)
        {
            StopCoroutine(routine);
            Camera.main.GetComponent<CameraFollow>().reset();
        }
    }
    public void carry(int value)
    {
        carryMoneh = value;
        carrying = true;
        body.SetActive(carrying);
        float temp = moveSpeed;
        moveSpeed = carrySpeed;
        carrySpeed = temp;
    }
    public Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
    {
        return (start + startVelocity * time + Physics.gravity * time * time * coinLine);
    }
    IEnumerator DrawLine()
    {
        for (;;)
        {
            List<Vector3> verts = new List<Vector3>();
            Vector3 force = (transform.forward + (transform.up / arkAmount)).normalized;
            float throwAmount = Mathf.Clamp((Time.time - timeHeld) * throwSpeed, 0, maxThrowForce);
            force = force * throwAmount + (movement.normalized * throwAmount) / moveThrow;
            for (float i = 0; i < 3; i += .1f)
            {
                verts.Add(transform.InverseTransformPoint(PlotTrajectoryAtTime(transform.position, force, i)));
            }
            m_lr.SetVertexCount(verts.Count);
            for (var i = 0; i < verts.Count; i++)
            {
                m_lr.SetPosition(i, verts[i]);
            }
            m_lr.gameObject.SetActive(true);
            yield return new WaitForSeconds(.01f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = m_camera.TransformDirection(moveDirection);
        moveDirection.y *= 0;
        movement = moveDirection.normalized * moveSpeed;
        if (Input.GetAxisRaw("Drop") != 0 && moneh > 0 && droppedThisFrame)
        {
            timeHeld = Time.time;
            StartCoroutine(lineDraw);
            droppedThisFrame = false;
        }
        else if (Input.GetAxisRaw("Drop") == 0 && !droppedThisFrame)
        {
            moneh--;
            droppedThisFrame = true;
            GameObject go = Instantiate(coin, transform.position, Random.rotation) as GameObject;
            Physics.IgnoreCollision(go.GetComponent<Collider>(), GetComponent<Collider>(), true);
            Vector3 force = (transform.forward + (transform.up / arkAmount)).normalized;
            float throwAmount = Mathf.Clamp((Time.time - timeHeld) * throwSpeed, 0, maxThrowForce);
            go.GetComponent<Rigidbody>().AddForce(force * throwAmount);
            go.GetComponent<Rigidbody>().AddForce((movement.normalized * throwAmount) / moveThrow);
            StopCoroutine(lineDraw);
            lineDraw = DrawLine();
            m_lr.gameObject.SetActive(false);
        }
        if (triggerObject != null)
            TriggerHandle();
        if (Input.GetButtonDown("Jump"))
        {
            var targets = Physics.OverlapSphere(transform.position, grabDistance);
            foreach (var target in targets)
            {
                if (target.tag == "coin")
                {
                    if (!target.GetComponent<CoinGrab>().grabbed)
                    {
                        moneh++;
                        Destroy(target.gameObject);
                        break;
                    }
                }
            }
        }
    }
    public override bool Seen(string tag)
    {
        if (tag == "Player")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void FixedUpdate()
    {

        if (moveDirection.magnitude > 0)
            transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(moveDirection), transform.rotation, turnSpeed);
        GetComponent<Rigidbody>().MovePosition(transform.position + movement * Time.deltaTime);
    }
}
