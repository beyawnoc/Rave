using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Transform table;
    public Transform chair;
    public Transform Whiteboard;
    public Transform Trashcan;

    public Vector3 initialPos = new Vector3(0, 0, 0);

    public void createTable()
    {
        Instantiate(table, initialPos, Quaternion.Euler(-90, 0, 0));
    }

    public void createChair()
    {
        Instantiate(chair, initialPos, Quaternion.Euler(-90, 0, 0));
    }

    public void createWhiteboard()
    {
        Instantiate(Whiteboard, initialPos, Quaternion.Euler(-90, 0, 0));
    }
    public void createTrashcan()
    {
        Instantiate(Trashcan, initialPos, Quaternion.Euler(-90, 0, 0));
    }

    public void resetScene(int sceneNum)
    {
        loadScene(sceneNum);
    }

    public void loadScene(int sceneNum)
    {
        SceneManager.LoadSceneAsync(sceneNum);
    }
}
