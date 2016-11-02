using UnityEngine;
using System.Collections;

public class Seeable : MonoBehaviour
{
    public bool alreadySeen = false;
    public virtual bool Seen(string tag)
    {
        if (alreadySeen && tag == gameObject.tag) return false;  
        return true;
    }

}
