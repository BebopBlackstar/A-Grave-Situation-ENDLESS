using UnityEngine;
using System.Collections;

public class diggable : Seeable
{
    public Transform graveTop;
    public float digSpeed;
    public float dropDistance = 3;
    public int value;
    public int Low = 1;
    public int High = 10;
    private float completion;
    public float percentComplete;
    private bool complete = false;
    public bool collected = false;
    private GameObject player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        value = Random.Range(Low, High);

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
                complete = true;
                Input.GetAxis("breakthatshit");
            }
            else
            {
                completion += digSpeed;
                graveTop.position = new Vector3(graveTop.position.x, graveTop.position.y - digSpeed, graveTop.position.z);
                percentComplete = Mathf.Floor(completion / dropDistance * 100);
            }

            yield return null;
        }
    }
    public override bool Seen()
    {
        if (percentComplete > 30 && !alreadySeen)
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
