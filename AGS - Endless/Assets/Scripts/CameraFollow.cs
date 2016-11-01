using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Place the target (player) here")]
    public Transform target;
    private Vector3 m_offset;
    [Range(0, 1), Tooltip("How fast the camera rotates and follows target")]
    public float time = .5f;
    [Range(0, 100), Tooltip("How fast the camera resets")]
    public float zoomOutSpeed = .5f;
    [Tooltip("How fast the camera zooms when digging")]
    public float zoomSpeed = 1;
    //public float zoomSpeed = 50;
    //public float minZoom = 1, maxZoom = 100;
    private bool cameraRotated = false;
    private float m_baseCamera;
    private IEnumerator resetRoutine;
    private bool zooming = false;
    // Use this for initialization
    void Start()
    {
        m_offset = transform.position - target.position;
        m_baseCamera = Camera.main.fieldOfView;
    }
    public void zoom()
    {
        Camera.main.fieldOfView -= zoomSpeed;
        zooming = true;
    }
    public void reset()
    {
        if (Camera.main.fieldOfView != 60 && zooming == false)
        {
            StartCoroutine(zoomOut());
        }
        else
        {
            StopCoroutine(zoomOut());
            zooming = false;
        }
    }
    IEnumerator zoomOut()
    {
        for (;;)
        {
            if (Camera.main.fieldOfView < m_baseCamera && zooming == false)
            {
                Camera.main.fieldOfView += zoomOutSpeed;
                yield return null;
            }
            else if(zooming == false)
            {
                Camera.main.fieldOfView = m_baseCamera;
                StopCoroutine(zoomOut());
                yield return null;
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxis("bumpers") != 0 && cameraRotated == false)
        {
            m_offset = Quaternion.AngleAxis(Input.GetAxis("bumpers") * 90, Vector3.up) * m_offset;
            cameraRotated = true;
        }
        else if (Input.GetAxis("bumpers") == 0)
        {
            cameraRotated = false;
        }
        //m_offset = Vector3.MoveTowards(m_offset, target.position, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime);
        //bool tooClose = Vector3.Distance(m_offset, target.position) <= minZoom;
        //bool toofar = Vector3.Distance(m_offset, target.position) >= maxZoom;
        //if (tooClose || toofar)
        //{
        //    m_offset = Vector3.MoveTowards(m_offset, target.position, Input.GetAxis("Mouse ScrollWheel") * -(zoomSpeed + 1) * Time.deltaTime);
        //}
        transform.position = Vector3.Lerp(transform.position, target.position + m_offset, time);
        transform.LookAt(target.position);
    }
}
