using UnityEngine;
using System.Collections;

public class graveText : MonoBehaviour
{
    [Tooltip("Grave this is over")]
    public diggable grave;
    private TextMesh m_text;
    // Use this for initialization
    void Start()
    {
        m_text = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (grave.percentComplete > 0)
        {
            m_text.text = grave.percentComplete.ToString() + "%";
            
            m_text.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            if(grave.percentComplete == 100)
            {
                m_text.color = Color.green;
            }
            if(grave.collected == true)
            {
                m_text.text = "";
            }
        }
        else
        {
            m_text.text = "";
        }
    }
}
