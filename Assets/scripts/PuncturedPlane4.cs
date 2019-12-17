////////////////////////////////////////////////////////////////////////////
//
//  This file is part of UnityProjects
//
//  Copyright (c) 2018, richardstech
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy of
//  this software and associated documentation files (the "Software"), to deal in
//  the Software without restriction, including without limitation the rights to use,
//  copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//  Software, and to permit persons to whom the Software is furnished to do so,
//  subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
//  INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
//  PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//  SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using UnityEngine;
using System.Collections.Generic;

public class PuncturedPlane4 : MonoBehaviour {

    //  width of plane

    [Tooltip("Width of plane")]
    public float width;

    //  height of plane
    [Tooltip("Height of plane")]
    public float height;

    // stepSize controls number of triangles - each triangle has short sides of length stepSize

    [Tooltip("Step size is length of short sides of triagles and controls resolution")]
    [Range(0.001f, 1)]
    public float stepSize = 0.1f;

    // stepMax controls the maximum number of steps in a triangle

    [Range(1, 1000)]
    [Tooltip("Step max is the maximum number of steps in a triangle")]
    public int stepMax = 10;

    //  Material to use for plane

    public Material planeMaterial;

    // The rectangular holes array contains the rectangular cutout definitions

    [Tooltip("Coordinates define the rectangular cutouts ")]
    public Rect[] rectangularHoles = new Rect[0];

    // The ellipse holes array contains the elliptical cutout definitions

    [Tooltip("Coordinates define the elliptical cutouts ")]
    public Rect[] ellipticalHoles = new Rect[0];

    // ellipseCount controls number of triangles in ellipse 

    [Tooltip("Number of triangles to use for ellipse quadrants")]
    [Range(10, 1000)]
    public float ellipseCount = 20;
    
    private RectInt[] sRHoles;
    private RectInt[] sEHoles;

    void Start () {

        // convert dimensions into setpSize units

        int sWidth = (int)Mathf.Ceil(width / stepSize);
        int sHeight = (int)Mathf.Ceil(height / stepSize);

        //  prepare cutout arrays

        sRHoles = new RectInt[rectangularHoles.Length];

        for (int i = 0; i < rectangularHoles.Length; i++) {
            sRHoles[i].x = (int)Mathf.Ceil(rectangularHoles[i].x / stepSize);
            sRHoles[i].y = (int)Mathf.Ceil(rectangularHoles[i].y / stepSize);
            sRHoles[i].width = (int)Mathf.Ceil(rectangularHoles[i].width / stepSize);
            sRHoles[i].height = (int)Mathf.Ceil(rectangularHoles[i].height / stepSize);
        }

        sEHoles = new RectInt[ellipticalHoles.Length];

        for (int i = 0; i < ellipticalHoles.Length; i++) {
            sEHoles[i].x = (int)Mathf.Ceil(ellipticalHoles[i].x / stepSize);
            sEHoles[i].y = (int)Mathf.Ceil(ellipticalHoles[i].y / stepSize);
            sEHoles[i].width = (int)Mathf.Ceil(ellipticalHoles[i].width / stepSize);
            sEHoles[i].height = (int)Mathf.Ceil(ellipticalHoles[i].height / stepSize);
        }

        //  create plane

        GameObject myPlane = createPlane(new Vector3Int(sWidth, sHeight, 3));
        myPlane.transform.position = new Vector3(0, 0, 2);
        myPlane.GetComponent<Renderer>().material = planeMaterial;
    }

    void Update () {
	}

    private GameObject createPlane(Vector3Int size)
    {
        MeshFilter mf;

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plane.name = "Plane";
        mf = plane.GetComponent<MeshFilter>();
        mf.mesh = createMesh(size.x, size.y);

        return plane;
    }

    private Mesh createMesh(int width, int height)
    {

        Mesh mesh = new Mesh();

        int tricount = width * height * 6;
        int vcount = (width + 1) * (height + 1);

        Vector3[] rawVertices = new Vector3[vcount];
        int[] rawTri = new int[tricount];

        // these lists are used to track vertex indices that are actually used in triangles

        List<int> inUseVertexIndices = new List<int>();
        List<Vector2> inUseUV = new List<Vector2>();

        // entry in processed will be set to true if a vertex has been processed (i.e. is within a generated triangle side of a triangle)

        bool[] processed = new bool[width * height];

        for (int i = 0; i < width * height; i++)
            processed[i] = false;
 
        float woffset = 0;
        float hoffset = 0;
        int vindex = 0;
        int tindex = 0;
        int triangleCount = 0;

        for (int h = 0; h <= height; h++, hoffset += stepSize) {
            woffset = 0;
            for (int w = 0; w <= width; w++, woffset += stepSize, vindex++) {

                rawVertices[vindex] = new Vector3(woffset, hoffset, 0);

                if (inHole(w, h))
                    continue;

                // check if vertex is already included in a triangle

                if ((w < width) && (h < height)) {
                    if (processed[h * width + w])
                        continue;

                    // see how big a rectangle could be generated

                    int wExtent = 0;
                    int hExtent = height - h;
 
                    while (++wExtent < (width - w)) {
                        if (inHole(w + wExtent, h) || processed[w + wExtent + h * width])
                            // gone too far
                            break;
                        int hMax = 0;
                        while (++hMax < (height - h)) {
                            if (inHole(w + wExtent, h + hMax) || processed[w + wExtent + width * (h + hMax)])
                                break;
                        }
                        if (hExtent > hMax) {
                            hExtent = hMax;
                        }
                    }

                    // make sure triangles aren't too big

                    if (wExtent > stepMax)
                        wExtent = stepMax;

                    if (hExtent > stepMax)
                        hExtent = stepMax;

                    // now have defined rectangle

                    rawTri[tindex++] = vindex;
                    rawTri[tindex++] = vindex + (width + 1) * hExtent;
                    rawTri[tindex++] = vindex + wExtent;
                    rawTri[tindex++] = vindex + (width + 1) * hExtent;
                    rawTri[tindex++] = vindex + (width + 1) * hExtent + wExtent;
                    rawTri[tindex++] = vindex + wExtent;
                    triangleCount += 2;

                    for (int yy = h; yy < h + hExtent; yy++) {
                        for (int xx = w; xx < w + wExtent; xx++) {
                            processed[yy * width + xx] = true;
                        }
                    }

                    // now add used vertices to list if not already there

                    if (inUseVertexIndices.IndexOf(vindex) == -1) {
                        inUseVertexIndices.Add(vindex);
                        inUseUV.Add(new Vector2((float)w / (float)(width + 1), (float)h / (float)(height + 1)));
                    }

                    if (inUseVertexIndices.IndexOf(vindex + (width + 1) * hExtent) == -1) {
                        inUseVertexIndices.Add(vindex + (width + 1) * hExtent);
                        inUseUV.Add(new Vector2((float)w / (float)(width + 1), (float)(h + hExtent) / (float)(height + 1)));
                    }

                    if (inUseVertexIndices.IndexOf(vindex + wExtent) == -1) {
                        inUseVertexIndices.Add(vindex + wExtent);
                        inUseUV.Add(new Vector2((float)(w + wExtent) / (float)(width + 1), (float)h / (float)(height + 1)));
                    }

                    if (inUseVertexIndices.IndexOf(vindex + (width + 1) * hExtent + wExtent) == -1) {
                        inUseVertexIndices.Add(vindex + (width + 1) * hExtent + wExtent);
                        inUseUV.Add(new Vector2((float)(w + wExtent) / (float)(width + 1), (float)(h + hExtent) / (float)(height + 1)));
                    }
                }
            }
        }

        // create reduced versions of the other arrays

        List<Vector3> interVertices = new List<Vector3>();

        for (int i = 0; i < inUseVertexIndices.Count; i++) {
            interVertices.Add(rawVertices[inUseVertexIndices[i]]);
        }

        // create reduced sized tri list

        List<int> interTri = new List<int>();
        
        for (int i = 0; i < rawTri.Length; i++) {
            interTri.Add(inUseVertexIndices.IndexOf(rawTri[i]));
        }

        //  now need to full in the rectangles created for the ellipses

        vindex = interVertices.Count;

        for (int hole = 0; hole < sEHoles.Length; hole++) {
            int xCenter = sEHoles[hole].x + sEHoles[hole].width / 2;
            int yCenter = sEHoles[hole].y + sEHoles[hole].height / 2;
            int vx;
            int vy;

            for (int quad = 0; quad < 4; quad++) {
                if ((quad == 0) || (quad == 3))
                    vx = sEHoles[hole].x + sEHoles[hole].width;
                else
                    vx = sEHoles[hole].x;

                if ((quad == 0) || (quad == 1))
                    vy = sEHoles[hole].y + sEHoles[hole].height;
                else
                    vy = sEHoles[hole].y;

                interVertices.Add(new Vector3(vx * stepSize, vy * stepSize, 0));
                inUseUV.Add(new Vector2((float)vx / (float)(width + 1), (float)vy / (float)(height + 1)));
                int urvindex = vindex++;

                for (int ai = 0; ai <= ellipseCount; ai++) {
                    float angle0 = (quad * (Mathf.PI / 2)) + (Mathf.PI / (2 * ellipseCount)) * ai;
                    float angle1 = (quad * (Mathf.PI / 2)) + (Mathf.PI / (2 * ellipseCount)) * (ai + 1);
                    float ex0 = Mathf.Cos(angle0) * sEHoles[hole].width / 2 + xCenter;
                    float ey0 = Mathf.Sin(angle0) * sEHoles[hole].height / 2 + yCenter;
                    float ex1 = Mathf.Cos(angle1) * sEHoles[hole].width / 2 + xCenter;
                    float ey1 = Mathf.Sin(angle1) * sEHoles[hole].height / 2 + yCenter;

                    interVertices.Add(new Vector3(ex0 * stepSize, ey0 * stepSize, 0));
                    interVertices.Add(new Vector3(ex1 * stepSize, ey1 * stepSize, 0));

                    interTri.Add(urvindex);
                    interTri.Add(vindex++);
                    interTri.Add(vindex++);

                    inUseUV.Add(new Vector2((float)ex0 / (float)(width + 1), (float)ey0 / (float)(height + 1)));
                    inUseUV.Add(new Vector2((float)ex1 / (float)(width + 1), (float)ey1 / (float)(height + 1)));
                }

            }
        }

        Vector3[] vertices = interVertices.ToArray();
        int[] tri = interTri.ToArray();
        Vector3[] normals = new Vector3[vertices.Length];
        Vector2[] uv = inUseUV.ToArray();

        for (int i = 0; i < vertices.Length; i++) {
             normals[i] = -Vector3.forward;
        }

        Debug.Log("Triangle count: " + triangleCount.ToString());
        Debug.Log("Vertex count: " + vertices.Length.ToString());

        mesh.vertices = vertices;
        mesh.triangles = tri;
        mesh.normals = normals;
        mesh.uv = uv;
        return mesh;
    }

    bool inHole(int w, int h)
    {
        Vector2Int curPos = new Vector2Int(w, h);

        // Do rectangular hole check

        for (int hole = 0; hole < sRHoles.Length; hole++) {
            if (sRHoles[hole].Contains(curPos)) {
                return true;
            }
        }

        // Do elliptical hole check

        for (int hole = 0; hole < sEHoles.Length; hole++) {
            if (sEHoles[hole].Contains(curPos)) {
                return true;
            }
        }
        return false;
    }
}
