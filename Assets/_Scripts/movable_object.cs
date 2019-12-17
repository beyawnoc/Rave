using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movable_object : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDrag()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y - 1;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }
}
