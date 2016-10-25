using UnityEngine;
using System.Collections;

public class Seeable : MonoBehaviour
{
    public bool alreadySeen = false;
    public virtual bool Seen()
    {
        if (alreadySeen) return false;  
        return true;
    }

}
