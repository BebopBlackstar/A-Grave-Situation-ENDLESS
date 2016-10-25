using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class digComletionDisplay : MonoBehaviour
{
    public GameObject graves;
    private diggable[] diggableGraves;
    // Use this for initialization
    void Start()
    {
        diggableGraves = graves.GetComponentsInChildren<diggable>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var grave in diggableGraves)
        {
            if(grave.percentComplete > 0)
            {
                
            }
        }
    }
}
