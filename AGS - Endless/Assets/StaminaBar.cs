using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StaminaBar : MonoBehaviour
{

    private PlayerCont player;
    private float startStamina;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCont>();
        startStamina = player.stamina;
    }

    // Update is called once per frame
    void Update()
    {        
        GetComponentInChildren<RectTransform>().localScale = new Vector3(player.stamina / startStamina, GetComponentInChildren<RectTransform>().localScale.y);
    }
}
