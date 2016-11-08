using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; //Load scene testing Michael added, could lower performance?
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
    float baseMoveSpeed;

    [Header("Sounds")]
    [Tooltip("sound played on step")]
    public AudioClip moveSound;
    [Tooltip("sound played on dig")]
    public AudioClip digSound;
    [Tooltip("sound played on dropping body")]
    public AudioClip dropSound;


    [Header("Sprint settings")]
    [Tooltip("How fast the player will run")]
    public float sprintSpeed;
    [Tooltip("How fast the player will run with body")]
    public float bodySprintSpeed;
    [Tooltip("Stamina regen per second")]
    public float stamina;
    [Tooltip("How fast the stamina is consumed")]
    public float consumedSpeed;
    [Tooltip("How fast the stamina is consumed with body")]
    public float bodyConsumedSpeed;
    [Tooltip("Stamina regen per second")]
    public float regen;
    [Tooltip("Stamina regen delay in seconds")]
    public float delay;
    [Tooltip("Speed after sprint without stamina")]
    public float noStaminaSpeed;

    /// <Michael gameobject main menu linking>
    public GameObject isMainMenuActive;
    /// <Michael gameobject main menu linking>


    float maxStamina;
    bool drained = false;
    float timeDrained;
    private float coinLine = .11f;
    [Header("Coin Throwing")]
    [Tooltip("Coin Prefab")]
    public GameObject coin;
    [Tooltip("The max force to throw the coin")]
    public float maxThrowForce = 30;
    [Tooltip("How High the coin will be thrown (higher for less)")]
    public float arkAmount = 2;
    [Tooltip("How fast you wind up the throw")]
    public float throwSpeed = 10;
    [Tooltip("How much moving changes the distance (higher for less)"), Range(.1f, 10)]
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
        maxStamina = stamina;
        baseMoveSpeed = moveSpeed;
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
            if (dropSound != null)
            {
                GetComponent<AudioSource>().clip = dropSound;
                GetComponent<AudioSource>().Play();
            }


            moneh += carryMoneh;
            carryMoneh = 0;
            moveSpeed = carrySpeed;
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
        moveSpeed = carrySpeed;
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
                verts.Add(transform.InverseTransformPoint(PlotTrajectoryAtTime(transform.position, force, i)));
            m_lr.SetVertexCount(verts.Count);
            for (var i = 0; i < verts.Count; i++)
                m_lr.SetPosition(i, verts[i]);
            m_lr.gameObject.SetActive(true);
            yield return new WaitForSeconds(.01f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isMainMenuActive != null) //Michael main menu activation code/pause
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isMainMenuActive.activeSelf)
            { Time.timeScale = 0; isMainMenuActive.SetActive(true); }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 1;
                isMainMenuActive.SetActive(false);
                //SceneManager.LoadScene(0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            SceneManager.LoadScene(0);
        } //Michael main menu activation code/pause

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
                if (target.tag == "coin")
                    if (!target.GetComponent<CoinGrab>().grabbed)
                    {
                        moneh++;
                        Destroy(target.gameObject);
                        break;
                    }

        }
        if (Input.GetAxis("Sprint") != 0 && !drained && movement.magnitude > 0)
        {
            if (stamina >= consumedSpeed)
            {
                if (body.activeSelf == false)
                {
                    moveSpeed = sprintSpeed;
                }
                else
                {
                    moveSpeed = bodySprintSpeed;
                }
                if (body.activeSelf == false)
                    stamina -= consumedSpeed;
                else
                    stamina -= bodyConsumedSpeed;

                if (stamina < consumedSpeed)
                {
                    stamina = 0;
                    timeDrained = Time.time;
                    drained = true;
                }

               


            }
            else
            {
                stamina = 0;
                timeDrained = Time.time;
                drained = true;
            }

        }
        else if (stamina < maxStamina && (Input.GetAxis("Sprint") == 0 || drained))
        {
            if (drained)
            {
                if (body.activeSelf == false)
                    moveSpeed = noStaminaSpeed;
                else
                    moveSpeed = carrySpeed;
            }
            else
            {
                if (body.activeSelf == false)
                    moveSpeed = baseMoveSpeed;
                else
                    moveSpeed = carrySpeed;
            }
            if ((Time.time - timeDrained) > delay)
            {
                stamina += regen;
                if (stamina >= maxStamina)
                {
                    drained = false;
                    stamina = maxStamina;
                }
            }
        }
        else
        {
            if (body.activeSelf == false)
                moveSpeed = baseMoveSpeed;
            else
                moveSpeed = carrySpeed;
        }
    }
    public override bool Seen(string tag)
    {
        if (tag == "Player")
            return true;
        else
            return false;
    }
    void FixedUpdate()
    {
        if (moveDirection.magnitude > 0)
        {
            transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(moveDirection), transform.rotation, turnSpeed);

            if (moveSound != null && !GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().clip = moveSound;
                GetComponent<AudioSource>().Play();
            }
        }
        GetComponent<Rigidbody>().MovePosition(transform.position + movement * Time.deltaTime);
    }







}
