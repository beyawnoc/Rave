using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movable_object : MonoBehaviour
{
    public int rotateSpeed = 3;
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

        if (Input.GetKey("q"))
        {
            transform.Rotate(0,0, rotateSpeed);
        } else if (Input.GetKey("e"))
        {
            transform.Rotate(0, 0, (rotateSpeed * -1));

        }

        Vector3 mousePos = Input.mousePosition;
        var translatedPos = Camera.allCameras[1].ScreenToWorldPoint(mousePos);
        translatedPos.y = 0;
        transform.position = translatedPos;
    }
}
