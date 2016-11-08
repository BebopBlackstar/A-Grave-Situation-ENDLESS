using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class objective : MonoBehaviour {
    [Tooltip("how many graves you need to dig")]
    public int ObjectiveNum;
    private int Complete;
	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = Complete + "/" + ObjectiveNum;
    }
	public void Increment()
    {
        Complete++;
        GetComponent<Text>().text = Complete + "/" + ObjectiveNum;
    }

}
