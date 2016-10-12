using UnityEngine;
using System.Collections;

public class CharacterControllerScript : MonoBehaviour
{
    private int down, up;
    private Collider triggerObject;
    private IEnumerator routine;
    public float Speed;

    // Use this for initialization
    void Start()
    {
        //var vertices = GetComponent<MeshFilter>().mesh.vertices;
        //for(int i = 0 ; i < vertices.Length; i++)
        //{
        //    Debug.Log(vertices[i]);
        //    vertices[i] = transform.TransformPoint(vertices[i]);
        //    Debug.Log(vertices[i]);
        //    Debug.DrawLine(Vector3.zero, vertices[i], Color.blue, 10f);
        //}   
        
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "diggable")
        {
            triggerObject = other;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "diggable")
        {
            StopCoroutine(routine);
            triggerObject = null;
        }
    }
/*    public void diggingAHole() //because we are obviously a dwarf
    {
        if (Input.GetButtonDown("Jump"))
        {
           routine = triggerObject.gameObject.GetComponent<diggable>().dig();
           StartCoroutine(routine);         
        }
        if (Input.GetButtonUp("Jump"))
        {
            StopCoroutine(routine);
            Debug.Log("nonono, no dig right now");
            Debug.Log(up + " " + down);
        }
    } */
    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= Speed;
        GetComponent<CharacterController>().Move(moveDirection * Time.deltaTime);
//        if(triggerObject != null)  diggingAHole();

    }
}
