using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject firstMenu;
    // Use this for initialization
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale > 0)
        {
            Time.timeScale = 0;
            firstMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 0)
        {
            Time.timeScale = 1;
            firstMenu.SetActive(false);
        }
    }
}
