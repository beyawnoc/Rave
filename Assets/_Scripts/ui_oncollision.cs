using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_oncollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(this.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name != "Plane")
        {
            Destroy(col.gameObject);
        }
    }
}
