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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuncturedPlane1 : MonoBehaviour {

    public int roomWidth;

    //  width of plane

    [Tooltip("Width of plane")]
    public float width;

    //  height of plane
    [Tooltip("Height of plane")]
    public float height;

    // stepSize controls number of triangles - each triangle has short sides of length stepSize
 
    [Tooltip("Step size is length of short sides of triagles and controls resolution")]
    public float stepSize = 0.1f;

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
        //mf.mesh = createMesh(size.x, size.y);

        Mesh hwall1 = createMesh(size.x, size.y);
        
        //need to flip normals
        Mesh hwall2 = (Mesh)Instantiate (hwall1);
        Vector3[] newvert = (Vector3[]) hwall2.vertices.Clone();
        for (int i = 0; i < hwall2.vertexCount; i++) {
            newvert[i] = new Vector3 (hwall2.vertices[i].x, hwall2.vertices[i].y, hwall2.vertices[i].z + roomWidth);
        }
        hwall2.vertices = newvert;
        hwall1 = FlipMesh(hwall1);

        Mesh wwall3 = createMesh(roomWidth, size.y);
        newvert = (Vector3[]) wwall3.vertices.Clone();
        for (int i = 0; i < wwall3.vertexCount; i++) {
            newvert[i] = new Vector3 (wwall3.vertices[i].z, wwall3.vertices[i].y, wwall3.vertices[i].x);
        }
        wwall3.vertices = newvert;

        //need to flip normals
        Mesh wwall4 = (Mesh)Instantiate (wwall3);
        newvert = (Vector3[]) wwall3.vertices.Clone();
        for (int i = 0; i < wwall3.vertexCount; i++) {
            newvert[i] = new Vector3 (wwall3.vertices[i].x + size.x, wwall3.vertices[i].y, wwall3.vertices[i].z);
        }
        wwall4.vertices = newvert;
        wwall4 = FlipMesh(wwall4);
        //wwall4.uv = wwall4.uv.Reverse().ToArray();
        //wwall4.triangles = wwall4.triangles.Reverse().ToArray();

        Mesh floor = createMesh(size.x, roomWidth);
        newvert = (Vector3[]) floor.vertices.Clone();
        for (int i = 0; i < floor.vertexCount; i++) {
            newvert[i] = new Vector3 (floor.vertices[i].x, floor.vertices[i].z, floor.vertices[i].y);
        }
        floor.vertices = newvert;
        // Mesh ceiling = createMesh(size.x, size.y);;
        Mesh ceiling = (Mesh)Instantiate (floor);
        newvert = (Vector3[]) floor.vertices.Clone();
        for (int i = 0; i < floor.vertexCount; i++) {
            newvert[i] = new Vector3 (floor.vertices[i].x, floor.vertices[i].y + size.y, floor.vertices[i].z);
        }
        ceiling.vertices = newvert;
        ceiling = FlipMesh(ceiling);

        wwall3.RecalculateNormals();
        wwall4.RecalculateNormals();
        hwall1.RecalculateNormals();
        //hwall2.RecalculateNormals();
        floor.RecalculateNormals();
        ceiling.RecalculateNormals();

        mf.mesh = CombineMeshes( new List<Mesh> {hwall1, hwall2, wwall3, wwall4, floor, ceiling});
        //mf.mesh.RecalculateNormals();
        //mf.mesh = hwall1;
        return plane;
    }

    private Mesh FlipMesh(Mesh mesh) {
        var indices = mesh.triangles;
        var triangleCount = indices.Length / 3;
        for(var i = 0; i < triangleCount; i++)
        {
            Debug.Log(indices[i*3]);
            Debug.Log(indices[i*3+1]);
            var tmp = indices[i*3];
            indices[i*3] = indices[i*3 + 1];
            indices[i*3 + 1] = tmp;
            Debug.Log(indices[i*3]);
            Debug.Log(indices[i*3+1]);
        }
        mesh.triangles = indices;
        // additionally flip the vertex normals to get the correct lighting
        var normals = mesh.normals;
        for(var n = 0; n < normals.Length; n++)
        {
            normals[n] = -normals[n];
        }
        mesh.normals = normals;
        return mesh;
    }
    private Mesh CombineMeshes(List<Mesh> meshes)
    {
        var combine = new CombineInstance[meshes.Count];
        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i];
            combine[i].transform = transform.localToWorldMatrix;
        }

        var mesh = new Mesh();
        mesh.CombineMeshes(combine);
        return mesh;
    }

    private Mesh createMesh(int width, int height)
    {

        Mesh mesh = new Mesh();

        int tricount = width * height * 6;
        int vcount = (width + 1) * (height + 1);

        Vector3[] vertices = new Vector3[vcount];
        int[] tri = new int[tricount];
        Vector3[] normals = new Vector3[vcount];
        Vector2[] uv = new Vector2[vcount];

        float woffset = 0;
        float hoffset = 0;
        int vindex = 0;
        int tindex = 0;

        int triangleCount = 0;

        for (int h = 0; h <= height; h++, hoffset += stepSize) {
            woffset = 0;
            for (int w = 0; w <= width; w++, woffset += stepSize, vindex++) {
                vertices[vindex] = new Vector3(woffset, hoffset, 0);
                normals[vindex] = -Vector3.forward;
                uv[vindex] = new Vector2((float)w / (float)(width + 1), (float)h / (float)(height + 1));

                // check if vertex is in a hole

                Vector2Int curPos = new Vector2Int(w, h);
                bool inHole = false;
                for (int hole = 0; hole < sHoles.Length; hole++) {
                    if (sHoles[hole].Contains(curPos)) {
                        inHole = true;
                        break;
                    }
                }

                if (inHole)
                    continue;

                if ((w != width) && (h != height)) {
                    tri[tindex++] = vindex;
                    tri[tindex++] = vindex + width + 1;
                    tri[tindex++] = vindex + 1;
                    tri[tindex++] = vindex + width + 1;
                    tri[tindex++] = vindex + width + 2;
                    tri[tindex++] = vindex + 1;
                    triangleCount += 2;
                }
            }
        }
        Debug.Log("Triangle count: " + triangleCount.ToString());
        mesh.vertices = vertices;
        mesh.triangles = tri;
        mesh.normals = normals;
        mesh.uv = uv;
        return mesh;
    }
}
