using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Money : MonoBehaviour
{
    private PlayerCont player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCont>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = player.moneh.ToString();
    }
}
