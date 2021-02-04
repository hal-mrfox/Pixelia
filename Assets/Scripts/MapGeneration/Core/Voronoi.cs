using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MIConvexHull;


public static class Voronoi
{
    public static Mesh GenerateVoronoiMesh(Vector3[] points, Vector3 origin)
    {
        Vertex3[] verticesVoronoi = new Vertex3[points.Length + 1];
        for (int i = 0; i < points.Length; i++)
        {
            verticesVoronoi[i] = new Vertex3(points[i] + origin);
        }

        verticesVoronoi[verticesVoronoi.Length - 1] = new Vertex3(origin);

        VoronoiMesh<Vertex3, Cell3, VoronoiEdge<Vertex3, Cell3>> voronoiMesh;
        voronoiMesh = VoronoiMesh.Create<Vertex3, Cell3>(verticesVoronoi);

        List<Vector3> vertices = new List<Vector3>();
        foreach (VoronoiEdge<Vertex3, Cell3> edge in voronoiMesh.Edges)
        {
            Vector3 source = edge.Source.Center;

            vertices.Add(source);
        }

        vertices = vertices.Distinct().ToList();

        return ConvexHull.GenerateMesh(vertices.ToArray());
    }
}
