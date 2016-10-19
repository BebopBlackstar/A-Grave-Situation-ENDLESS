using UnityEngine;
using System.Collections;

public class diggable : Seeable
{
    public Transform graveTop;
    public float digSpeed;
    public float dropDistance = 3;
    public float value;
    private float completion;
    public float percentComplete;
    private bool complete = false;
    private bool collected = false;
    private bool unpressed = false;
    private GameObject player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (value == 0)
        {
            value = Random.Range(10, 100);
        }
    }
    public IEnumerator dig()
    {
        while (true)
        {

            Debug.Log(completion);
            if (collected == false && complete == true)
            {
                collected = true;
                player.GetComponent<PlayerCont>().carry(value);
            }
            else if (completion >= dropDistance)
            {
                Debug.Log("well the grave is perfect");
                complete = true;
                Input.GetAxis("Jump"); //works for some reason dunno why
            }
            else
            {
                completion += digSpeed;
                graveTop.position = new Vector3(graveTop.position.x, graveTop.position.y - digSpeed, graveTop.position.z);
                percentComplete = completion / dropDistance * 100;
            }
         
            yield return null;
        }
    }
    public override bool Seen()
    {
        if (complete)
        {
            return true;
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
    }
}
