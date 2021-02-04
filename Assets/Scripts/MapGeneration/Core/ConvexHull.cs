using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MIConvexHull;

public static class ConvexHull
{
    public static Mesh GenerateMesh(Vector3[] vertices)
    {
        Vertex3[] convexHullVertices = new Vertex3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            convexHullVertices[i] = new Vertex3(vertices[i]);
        }

        ConvexHullCreationResult<Vertex3, Face3> convexHull = MIConvexHull.ConvexHull.Create<Vertex3, Face3>(convexHullVertices);

        List<Vertex3> convexHullPoints = new List<Vertex3>(convexHull.Result.Points);
        List<Face3> convexHullFaces = new List<Face3>(convexHull.Result.Faces);
        List<int> convexHullIndices = new List<int>();

        foreach (Face3 f in convexHullFaces)
        {
            convexHullIndices.Add(convexHullPoints.IndexOf(f.Vertices[0]));
            convexHullIndices.Add(convexHullPoints.IndexOf(f.Vertices[1]));
            convexHullIndices.Add(convexHullPoints.IndexOf(f.Vertices[2]));
        }

        Mesh mesh = GetMesh(convexHullPoints.ToArray(), convexHullIndices.ToArray());

        return mesh;
    }

    private static Mesh GetFlatMesh(Vertex3[] convexHullVertices, int[] convexHullIndices)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();

        // CREATE TRIANGLES FOR MESH
        for (int j = 0; j < convexHullIndices.Length; j += 3)
        {
            int v0 = convexHullIndices[j + 0];
            int v1 = convexHullIndices[j + 1];
            int v2 = convexHullIndices[j + 2];

            Vector3 a = new Vector3((float)convexHullVertices[v0].x, (float)convexHullVertices[v0].y, (float)convexHullVertices[v0].z);
            Vector3 b = new Vector3((float)convexHullVertices[v1].x, (float)convexHullVertices[v1].y, (float)convexHullVertices[v1].z);
            Vector3 c = new Vector3((float)convexHullVertices[v2].x, (float)convexHullVertices[v2].y, (float)convexHullVertices[v2].z);

            Vector3 normal = Vector3.Cross(a - b, a - c);
            normal.Normalize();

            triangles.Add(j + 0);
            triangles.Add(j + 1);
            triangles.Add(j + 2);

            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);

            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }

    private static Mesh GetMesh(Vertex3[] convexHullVertices, int[] convexHullIndices)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Populate vertices
        for (int i = 0; i < convexHullVertices.Length; i++)
        {
            Vector3 v = new Vector3((float)convexHullVertices[i].x, (float)convexHullVertices[i].y, (float)convexHullVertices[i].z);
            vertices.Add(v);

            Vector3 normal = v.normalized;
            normals.Add(normal);
        }
        vertices.Distinct();

        // CREATE TRIANGLES FOR MESH
        for (int j = 0; j < convexHullIndices.Length; j += 3)
        {
            int v0 = convexHullIndices[j + 0];
            int v1 = convexHullIndices[j + 1];
            int v2 = convexHullIndices[j + 2];

            Vector3 a = new Vector3((float)convexHullVertices[v0].x, (float)convexHullVertices[v0].y, (float)convexHullVertices[v0].z);
            Vector3 b = new Vector3((float)convexHullVertices[v1].x, (float)convexHullVertices[v1].y, (float)convexHullVertices[v1].z);
            Vector3 c = new Vector3((float)convexHullVertices[v2].x, (float)convexHullVertices[v2].y, (float)convexHullVertices[v2].z);

            triangles.Add(GetVerticeIndex(a, vertices.ToArray()));
            triangles.Add(GetVerticeIndex(b, vertices.ToArray()));
            triangles.Add(GetVerticeIndex(c, vertices.ToArray()));
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }

    private static int GetVerticeIndex(Vector3 v, Vector3[] vertices)
    {
        int index = 0;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (v.Equals(vertices[i]))
            {
                index = i;
                break;
            }
        }

        return index;
    }
}
