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
        Debug.Log("Collided");
        if (col.gameObject.name != "Plane")
        {
            Destroy(col.gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Collided 2");
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("Collided 3");

    }


}
