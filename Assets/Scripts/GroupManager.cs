using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    //basically it removes itself if it's empty (e.g. all balls have removed for optimization purposes)
    void FixedUpdate()
    {
        if (transform.childCount <= 0)
            Destroy(this.gameObject);
    }
}
