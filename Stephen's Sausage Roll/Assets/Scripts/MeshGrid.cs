using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGrid : MonoBehaviour
{
    public float GridX { get { return transform.localPosition.x; } private set { } }
    public float GridY { get { return transform.localPosition.z; } private set { } }

    private float _side = 1;

    void DrawRectangle(float width, float height)
    {
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        if (null == mf)
            mf = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = mf.mesh;

        Vector3[] v = new Vector3[4];

        float halfDiffx = (_side - width) / 2;
        float halfDiffy = (_side - height) / 2;
        v[0] = new Vector3(halfDiffx, halfDiffy, 0);
        v[1] = new Vector3(halfDiffx, _side - halfDiffy, 0);
        v[2] = new Vector3(_side - halfDiffx, halfDiffy, 0);
        v[3] = new Vector3(_side - halfDiffx, _side - halfDiffy, 0);

        mesh.vertices = v;

        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 3;
        triangles[4] = 2;
        triangles[5] = 1;

        mesh.triangles = triangles;

    }
    public void Init()
    {
        DrawRectangle(0.9f, 0.9f);
    }
}
