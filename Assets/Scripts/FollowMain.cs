using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMain : MonoBehaviour
{
    // Start is called before the first frame update
    //Transform transform;
    GameObject parent;
    void Start()
    {
        //transform = GetComponent<Transform>();
        parent = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
// transform.position = parent.transform.position;
        this.transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y, -20f);
    }
}
