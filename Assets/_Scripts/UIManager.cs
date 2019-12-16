using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Transform cylinder;
    public Transform cube;
    public Transform sphere;

    public void createCube()
    {
        Instantiate(cube);
    }

    public void createCylinder()
    {
        Instantiate(cylinder);
    }

    public void createSphere(string test)
    {
        Debug.Log(test);
        Instantiate(sphere);
    }

    //public void instantiateFurniture(Furniture furniture)
    //{
    //    Instantiate()
    //}

    //public enum Furniture
    //{
    //    Sphere_Prefab,
    //    Cylinder_Prefab,
    //    Cube_Prefab
    //}
}
