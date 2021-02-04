/*
 * Mesh utilities
 * Mark Brand
 * mail@bymarkbrand.com
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// Helper class with Mesh utilities.
/// </summary>
public class MeshUtilities
{
    /// <summary>
    /// Unifies the meshes.
    /// </summary>
    /// <returns>The meshes.</returns>
    /// <param name="meshes">Meshes.</param>
    public static Mesh UnifyMeshes(Mesh[] meshes)
    {
        Mesh combinedMesh = new Mesh();

        CombineInstance[] combine = new CombineInstance[meshes.Length];

        for (int i = 0; i < meshes.Length; i++)
        {
            combine[i].mesh = meshes[i];
        }

        combinedMesh.CombineMeshes(combine, true, false);

        List<Vector3> vertices = combinedMesh.vertices.ToList();
        List<int> triangles = new List<int>();

        vertices = vertices.Distinct().ToList();

        for (int i = 0; i < combinedMesh.triangles.Length; i++)
        {
            Vector3 v1 = combinedMesh.vertices[combinedMesh.triangles[i]];

            foreach (Vector3 v2 in vertices) {
                if (v1.Equals(v2)) {
                    triangles.Add(vertices.IndexOf(v2));
                    break;
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }

    /// <summary>
    /// Flattens the mesh.
    /// </summary>
    /// <returns>The mesh.</returns>
    /// <param name="mesh">Mesh.</param>
    public static Mesh FlattenMesh(Mesh mesh)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int v0 = mesh.triangles[i + 0];
            int v1 = mesh.triangles[i + 1];
            int v2 = mesh.triangles[i + 2];

            Vector3 a = mesh.vertices[v0];
            Vector3 b = mesh.vertices[v1];
            Vector3 c = mesh.vertices[v2];

            Vector3 normal = Vector3.Cross(a - b, a - c);
            normal.Normalize();

            triangles.Add(i + 0);
            triangles.Add(i + 1);
            triangles.Add(i + 2);

            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);

            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
        }

        mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }

    /// <summary>
    /// Creates a primitive mesh. DEPRECATED.
    /// </summary>
    /// <returns>The primitive mesh.</returns>
    /// <param name="type">Type of primitive.</param>
    public static Mesh CreatePrimitiveMesh(PrimitiveType type)
    {
        GameObject gameObject = GameObject.CreatePrimitive(type);

        Mesh defaultMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        Mesh mesh = new Mesh();

        mesh.vertices = defaultMesh.vertices;
        mesh.triangles = defaultMesh.triangles;
        mesh.normals = defaultMesh.normals;
        mesh.tangents = defaultMesh.tangents;
        mesh.colors = defaultMesh.colors;
        mesh.uv = defaultMesh.uv;
        mesh.name = type.ToString();

        Object.DestroyImmediate(gameObject);

        return mesh;
    }

    /// <summary>
    /// Duplicates a mesh.
    /// </summary>
    /// <returns>Duplicated mesh.</returns>
    /// <param name="mesh">Mesh.</param>
    public static Mesh DuplicateMesh(Mesh mesh)
    {
        Mesh dupelicateMesh = new Mesh();

        dupelicateMesh.vertices = mesh.vertices;
        dupelicateMesh.triangles = mesh.triangles;
        dupelicateMesh.normals = mesh.normals;
        dupelicateMesh.tangents = mesh.tangents;
        dupelicateMesh.colors = mesh.colors;
        dupelicateMesh.uv = mesh.uv;
        dupelicateMesh.name = mesh.name;

        return dupelicateMesh;
    }

    /// <summary>
    /// Gets a random point on mesh.
    /// </summary>
    /// <returns>The random point on mesh.</returns>
    /// <param name="mesh">Mesh.</param>
    public static Vector3 GetRandomPointOnMesh(Mesh mesh)
    {
        int triangleCount = mesh.triangles.Length / 3;
        float[] sizes = new float[triangleCount];
        for (int i = 0; i < triangleCount; i++)
        {
            Vector3 va = mesh.vertices[mesh.triangles[i * 3 + 0]];
            Vector3 vb = mesh.vertices[mesh.triangles[i * 3 + 1]];
            Vector3 vc = mesh.vertices[mesh.triangles[i * 3 + 2]];

            sizes[i] = .5f * Vector3.Cross(vb - va, vc - va).magnitude;
        }

        // if you're repeatedly doing this on a single mesh, you'll likely want to cache cumulativeSizes and total
        float[] cumulativeSizes = new float[sizes.Length];
        float total = 0;

        for (int i = 0; i < sizes.Length; i++)
        {
            total += sizes[i];
            cumulativeSizes[i] = total;
        }

        // so everything above this point wants to be factored out
        float randomsample = Random.value * total;

        int triIndex = -1;

        for (int i = 0; i < sizes.Length; i++)
        {
            if (randomsample <= cumulativeSizes[i])
            {
                triIndex = i;
                break;
            }
        }

        Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3 + 0]];
        Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
        Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

        // generate random barycentric coordinates
        float r = Random.value;
        float s = Random.value;

        if (r + s >= 1)
        {
            r = 1 - r;
            s = 1 - s;
        }
        // and then turn them back to a Vector3
        Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
        return pointOnMesh;
    }

    //public static Vector3 GetRandomPointOnMesh(Mesh mesh)
    //{
    //    int triangleCount = mesh.triangles.Length / 3;
    //    float[] sizes = new float[triangleCount];
    //    for (int i = 0; i < triangleCount; i++)
    //    {
    //        Vector3 va = mesh.vertices[mesh.triangles[i * 3 + 0]];
    //        Vector3 vb = mesh.vertices[mesh.triangles[i * 3 + 1]];
    //        Vector3 vc = mesh.vertices[mesh.triangles[i * 3 + 2]];

    //        sizes[i] = .5f * Vector3.Cross(vb - va, vc - va).magnitude;
    //    }

    //    // if you're repeatedly doing this on a single mesh, you'll likely want to cache cumulativeSizes and total
    //    float[] cumulativeSizes = new float[sizes.Length];
    //    float total = 0;

    //    for (int i = 0; i < sizes.Length; i++)
    //    {
    //        total += sizes[i];
    //        cumulativeSizes[i] = total;
    //    }

    //    // so everything above this point wants to be factored out
    //    float randomsample = Random.value * total;

    //    int triIndex = -1;

    //    for (int i = 0; i < sizes.Length; i++)
    //    {
    //        if (randomsample <= cumulativeSizes[i])
    //        {
    //            triIndex = i;
    //            break;
    //        }
    //    }

    //    if (triIndex == -1) Debug.LogError("triIndex should never be -1");

    //    Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3 + 0]];
    //    Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
    //    Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

    //    // generate random barycentric coordinates
    //    float r = Random.value;
    //    float s = Random.value;

    //    if (r + s >= 1)
    //    {
    //        r = 1 - r;
    //        s = 1 - s;
    //    }
    //    // and then turn them back to a Vector3
    //    Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
    //    return pointOnMesh;
    //}

    /// <summary>
    /// converts Mesh to string.
    /// </summary>
    /// <returns>The Mesh as string.</returns>
    /// <param name="mesh">Mesh.</param>
    public static string MeshToString(Mesh mesh)
    {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("g Exported Mesh").Append("\n");
        foreach (Vector3 v in mesh.vertices)
        {
            stringBuilder.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        stringBuilder.Append("\n");
        foreach (Vector3 v in mesh.normals)
        {
            stringBuilder.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        stringBuilder.Append("\n");
        foreach (Vector3 v in mesh.uv)
        {
            stringBuilder.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }

        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            stringBuilder.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Exports Mesh to object.
    /// </summary>
    /// <param name="mesh">Mesh.</param>
    /// <param name="filename">Filename.</param>
    public static void ExportToOBJ(Mesh mesh, string filename)
    {
        string extension = Path.GetExtension(filename);

        if (extension.ToLower() != ".obj")
        {
            filename += ".obj";
        }

        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.Write(MeshToString(mesh));
        }
    }
}
