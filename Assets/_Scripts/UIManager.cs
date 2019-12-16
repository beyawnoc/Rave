using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Transform desk;
    public Transform chair;
    public Transform Whiteboard;
    public Transform Trashcan;

    public void createDesk()
    {
        Instantiate(desk);
    }

    public void createChair()
    {
        Instantiate(chair);
    }

    public void createWhiteboard()
    {
        Instantiate(Whiteboard);
    }
    public void createTrashcan()
    {
        Instantiate(Trashcan);
    }
}
