using UnityEngine;
using System.Collections;

public class PlayerCont : Seeable
{
    public float moveSpeed = 10;
    private Transform m_camera;
    private bool carrying;
    private IEnumerator routine;
    private Collider triggerObject;
    public float turnSpeed = .5f;
    public float moneh, carryMoneh;
    private GameObject body;
    public GameObject deactiveGuard;
    Vector3 movement, moveDirection;
    public float carrySpeed = 5;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("InvisiableWalls"))
            triggerObject = other;
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("InvisiableWalls"))
        {
            StopCoroutine(routine);
            triggerObject = null;
        }
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
        if (Input.GetButtonUp("Jump"))
        {
            StopCoroutine(routine);
        }
    }
    public void carry(float value)
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
        if (Input.GetAxis("Horizontal") != 0)
        {
            deactiveGuard.SetActive(true);
        }
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
