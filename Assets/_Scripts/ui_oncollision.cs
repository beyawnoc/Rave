using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_oncollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Furniture")
        {
            Destroy(col.gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Furniture")
        {
            Destroy(col.gameObject);
        }
    }
}
