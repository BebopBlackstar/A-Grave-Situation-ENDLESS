using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private Vector3 m_offset;
    [Range(0, 1)]
    public float time = .05f;
    public float rotationSpeed = 50;
    public float zoomSpeed = 50;
    public float minZoom = 1, maxZoom = 100;
    private bool cameraRotated = false;
    // Use this for initialization
    void Start()
    {
        m_offset = transform.position - target.position;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxis("bumpers") != 0 && cameraRotated == false)
        {
            m_offset = Quaternion.AngleAxis(Input.GetAxis("bumpers") * 90, Vector3.up) * m_offset;
            cameraRotated = true;
        }
        else if(Input.GetAxis("bumpers") == 0)
        {
            cameraRotated = false;
        }
        m_offset = Vector3.MoveTowards(m_offset, target.position, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime);
        bool tooClose = Vector3.Distance(m_offset, target.position) <= minZoom;
        bool toofar = Vector3.Distance(m_offset, target.position) >= maxZoom;
        if (tooClose || toofar)
        {
            m_offset = Vector3.MoveTowards(m_offset, target.position, Input.GetAxis("Mouse ScrollWheel") * -(zoomSpeed + 1) * Time.deltaTime);
        }
        transform.position = Vector3.Lerp(transform.position, target.position + m_offset, time);
        transform.LookAt(target.position);
    }
}
