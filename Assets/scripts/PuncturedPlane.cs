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

public class PuncturedPlane : MonoBehaviour {

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

    // The holes array contains the rectangular cutout definitions

    [Tooltip("Coordinates define the rectangular cutouts ")]
    public Rect[] holes = new Rect[0];

    private RectInt[] sHoles;

    void Start () {

        // convert dimensions into setpSize units

        int sWidth = (int)Mathf.Ceil(width / stepSize);
        int sHeight = (int)Mathf.Ceil(height / stepSize);

        sHoles = new RectInt[holes.Length];

        for (int i = 0; i < holes.Length; i++) {
            sHoles[i].x = (int)Mathf.Ceil(holes[i].x / stepSize);
            sHoles[i].y = (int)Mathf.Ceil(holes[i].y / stepSize);
            sHoles[i].width = (int)Mathf.Ceil(holes[i].width / stepSize);
            sHoles[i].height = (int)Mathf.Ceil(holes[i].height / stepSize);
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

        Vector3[] vertices = new Vector3[inUseVertexIndices.Count];
        Vector2[] uv = inUseUV.ToArray();
        Vector3[] normals = new Vector3[inUseVertexIndices.Count];

        for (int i = 0; i < inUseVertexIndices.Count; i++) {
            vertices[i] = rawVertices[inUseVertexIndices[i]];
            normals[i] = -Vector3.forward;
        }

        // create reduced sized tri array

        int[] tri = new int[tindex];
        
        for (int i = 0; i < tri.Length; i++) {
            tri[i] = inUseVertexIndices.IndexOf(rawTri[i]);
        }

        Debug.Log("Triangle count: " + triangleCount.ToString());
        Debug.Log("Vertex count: " + vertices.Length.ToString() + " (originally " + rawVertices.Length.ToString() + ")");

        mesh.vertices = vertices;
        mesh.triangles = tri;
        mesh.normals = normals;
        mesh.uv = uv;
        return mesh;
    }

    bool inHole(int w, int h)
    {
        Vector2Int curPos = new Vector2Int(w, h);
        for (int hole = 0; hole < sHoles.Length; hole++) {
            if (sHoles[hole].Contains(curPos)) {
                return true;
            }
        }

        return false;
    }
}
