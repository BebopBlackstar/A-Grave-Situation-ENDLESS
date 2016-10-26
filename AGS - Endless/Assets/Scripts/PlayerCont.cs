using UnityEngine;
using System.Collections;

public class PlayerCont : Seeable
{
    [Header("Player Setup")]
    [Tooltip("How fast the player will move")]
    public float moveSpeed = 10;
    [Tooltip("Speed of the model turning, only for looks")]
    public float turnSpeed = .5f;
    [Tooltip("Current money")]
    public int moneh;
    [Tooltip("Money being carryed on body")]
    public int carryMoneh;

    [Header("Coin Throwing")]
    [Tooltip("Coin Prefab")]
    public GameObject coin;
    [Tooltip("The max force to throw the coin")]
    public float maxThrowForce = 30;
    [Tooltip("How High the coin will be thrown")]
    public float arkAmount = 2;
    [Tooltip("How fast you wind up the throw")]
    public float throwSpeed = 10;

    private Transform m_camera;
    private bool carrying;
    private IEnumerator routine;
    private Collider triggerObject;
    private GameObject body;

    Vector3 movement, moveDirection;
    public float carrySpeed = 5;
    private bool droppedThisFrame = true;
    private float timeHeld;
    public void OnTriggerEnter(Collider other)
    {
        triggerObject = other;
    }
    public void OnTriggerExit(Collider other)
    {
        if (routine != null)
            StopCoroutine(routine);
        triggerObject = null;
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
    // Use this for initialization
    void Start()
    {

        m_camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        body = GameObject.FindGameObjectWithTag("CarryBody");
        body.SetActive(false);
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
            droppedThisFrame = false;
            Debug.Log(Time.time - timeHeld);
        }
        else if (Input.GetAxisRaw("Drop") == 0 && !droppedThisFrame)
        {
            moneh--;
            droppedThisFrame = true;
            GameObject go = Instantiate(coin, transform.position, Random.rotation) as GameObject;
            Physics.IgnoreCollision(go.GetComponent<Collider>(), GetComponent<Collider>(), true);
            Vector3 force = (transform.forward + (transform.up / arkAmount)).normalized;
            go.GetComponent<Rigidbody>().AddForce(force * Mathf.Clamp((Time.time - timeHeld) * throwSpeed, 0, maxThrowForce));
        }
        if (Input.GetAxisRaw("Drop") != 0 && moneh > 0)
        {

        }

        //if (Input.GetAxisRaw("Drop") != 0 && !droppedThisFrame && moneh > 0)
        //{
        //    moneh--;
        //    droppedThisFrame = true;
        //    GameObject go = Instantiate(coin, transform.position, Random.rotation) as GameObject;
        //    Physics.IgnoreCollision(go.GetComponent<Collider>(), GetComponent<Collider>(), true);
        //}
        //else if (Input.GetAxisRaw("Drop") == 0)
        //{
        //    droppedThisFrame = false;
        //}
        if (triggerObject != null)
            TriggerHandle();
    }
    public override bool Seen()
    {
        moveSpeed = 0;
        return true;
    }
    void FixedUpdate()
    {

        if (moveDirection.magnitude > 0)
            transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(moveDirection), transform.rotation, turnSpeed);
        GetComponent<Rigidbody>().MovePosition(transform.position + movement * Time.deltaTime);
    }
}
