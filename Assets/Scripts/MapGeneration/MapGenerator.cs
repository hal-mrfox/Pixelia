using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public CountryManager countryManager;
    public Transform provinceParent;
    public Transform countryParent;
    public bool generate;

    [Space(10)]
    public Image mapPreview;
    public Image voronoi;
    public Image stoneMap;
    public Image woodMap;
    public bool autoGenerate;
    public AnimationCurve falloffCurve;
    public AnimationCurve squareFalloffCurve;
    public int width;
    public int height;
    [Range(0, 0.5f)] public float jitter;
    //public Gradient colorMap;
    public NoiseSettings simplexSettings;
    public NoiseSettings voronoiSettings;
    public NoiseSettings stoneSettings;
    public NoiseSettings woodSettings;
    public int points;
    int addingNumber;

    [System.Serializable]
    public struct NoiseSettings
    {
        public bool voronoi;
        public int seed;
        public float scale;
        public int octaves;
        public float persistence;
        [Range(0, 1)] public float threshold;
        public float lacunarity;
        public float heightOffset;
        public Vector2 offset;
    }


    [Button]
    public void GenerateMap()
    {
        addingNumber = 0;
        if (Application.isPlaying)
        {
            for (int i = 0; i < countryManager.provinces.Count; i++)
            {
                Destroy(countryManager.provinces[i].gameObject);
            }
            for (int i = 0; i < countryManager.countries.Count; i++)
            {
                Destroy(countryManager.countries[i].gameObject);
            }
        }
        else
        {
            for (int i = 0; i < countryManager.provinces.Count; i++)
            {
                DestroyImmediate(countryManager.provinces[i].gameObject);
            }
            for (int i = 0; i < countryManager.countries.Count; i++)
            {
                DestroyImmediate(countryManager.countries[i].gameObject);
            }
        }
        countryManager.provinces.Clear();
        countryManager.countries.Clear();
        //map and provinces
        simplexSettings.seed = Random.Range(-100000, 100000);
        voronoiSettings.seed = Random.Range(-100000, 100000);
        //resources
        stoneSettings.seed = Random.Range(-100000, 100000);
        woodSettings.seed = Random.Range(-100000, 100000);
        //generate games heightmap
        float[,] noiseMap = ProcessNoiseMap(AddFalloffMap(GenerateNoiseMap(simplexSettings)), simplexSettings.threshold);
        mapPreview.sprite = Sprite.Create(GenerateTexture(noiseMap), new Rect(0, 0, width, height), new Vector2(.5f, .5f));
        //creating provinces
        SeparateVoronoi(VoronoiLandCheck(noiseMap, GenerateNoiseMap(voronoiSettings)));
        //creating stone resources
        noiseMap = ProcessNoiseMap(GenerateNoiseMap(stoneSettings), stoneSettings.threshold);
        stoneMap.sprite = Sprite.Create(GenerateResourceTexture(noiseMap), new Rect(0, 0, width, height), new Vector2(.5f, .5f));
        //creating wood resources
        noiseMap = ProcessNoiseMap(GenerateNoiseMap(woodSettings), woodSettings.threshold);
        woodMap.sprite = Sprite.Create(GenerateResourceTexture(noiseMap), new Rect(0, 0, width, height), new Vector2(.5f, .5f));

        countryManager.playerCountry = countryManager.countries[Random.Range(0, countryManager.countries.Count)];
    }

    public void Awake()
    {
        //GenerateVoronoi();
        if (generate)
        {
            GenerateMap();
        }
    }

    public void OnValidate()
    {
        if (autoGenerate)
        {
            GenerateMap();
        }
    }

    public float[,] AddFalloffMap(float[,] noiseMap)
    {
        Vector2 center = new Vector2(width * .5f, height * .5f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float distanceFromCenter = Vector2.Distance(center, new Vector2(x, y));
                float value;

                if (1 - distanceFromCenter / width >= 0)
                {
                    value = 1 - distanceFromCenter / width;
                }
                else
                {
                    value = 0;
                }

                float i = (float)x / width * 2 - 1;
                float j = (float)y / height * 2 - 1;

                value = falloffCurve.Evaluate(value);
                value += squareFalloffCurve.Evaluate(Mathf.Max(i < 0 ? -i : i, j < 0 ? -j : j));
                noiseMap[x, y] = value - noiseMap[x, y];
            }
        }

        return noiseMap;
    }

    public Color[] CheckLand(Color[] map)
    {
        for (int i = 0; i < map.Length; i++)
        {
            if (map[i].r == 0)
            {
                map[i].a = 0;
            }
        }

        return map;
    }

    public float[,] VoronoiLandCheck(float[,] noiseMap, float[,] map)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (noiseMap[x, y] == 1)
                {
                    map[x, y] = 1;
                }
            }
        }

        return map;
    }

    public float[,] GenerateNoiseMap(NoiseSettings noiseSetting)
    {
        FastNoise noise = new FastNoise(noiseSetting.seed);
        noise.SetCellularJitter(jitter);
        float[,] noiseMap = new float[width, height];
        Random.InitState(noiseSetting.seed);
        Vector2[] octaveOffsets = new Vector2[noiseSetting.octaves];

        float amplitude = 1;
        float frequency = 0;

        for (int i = 0; i < noiseSetting.octaves; i++)
        {
            octaveOffsets[i] = new Vector2(Random.Range(-100000, 100000), Random.Range(-100000, 100000)) + noiseSetting.offset;

            amplitude *= noiseSetting.persistence;
        }

        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                amplitude = 1;
                frequency = 1;

                float noiseHeight = 0;

                for (int i = 0; i < noiseSetting.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / noiseSetting.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / noiseSetting.scale * frequency;
                    float perlinValue = noiseSetting.voronoi ? noise.GetCellular(sampleX, sampleY) : Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= noiseSetting.persistence;
                    frequency *= noiseSetting.lacunarity;
                }

                noiseHeight += noiseSetting.heightOffset;

                if (noiseHeight > maxHeight)
                {
                    maxHeight = noiseHeight;
                }
                else if (noiseHeight < minHeight)
                {
                    minHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    public Texture2D GenerateTexture(float[,] heightMap)
    {
        Color32[] colorMap = new Color32[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[width * y + x] = Color.Lerp(Color.white, Color.black, heightMap[x, y]);
                //colorMap[width * y + x] = this.colorMap.Evaluate(heightMap[x, y]);
            }
        }

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels32(colorMap);
        texture.Apply();

        return texture;
    }

    public Texture2D GenerateResourceTexture(float[,] heightMap)
    {
        Color32[] colorMap = new Color32[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[width * y + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                if (colorMap[width * y + x].r == 255)
                {
                    colorMap[width * y + x].a = 0;
                }
                //colorMap[width * y + x] = this.colorMap.Evaluate(heightMap[x, y]);
            }
        }

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels32(colorMap);
        texture.Apply();

        return texture;
    }

    public float[,] ProcessNoiseMap(float[,] noiseMap, float threshold)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (noiseMap[x, y] > threshold)
                {
                    noiseMap[x, y] = 1;
                }
                else
                {
                    noiseMap[x, y] = 0;
                }
            }
        }

        return noiseMap;
    }

    void SeparateVoronoi(float[,] noiseMap)
    {
        List<float> values = new List<float>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (noiseMap[x, y] < 1f && !values.Contains(noiseMap[x, y]))
                {
                    values.Add(noiseMap[x, y]);
                }
            }
        }

        for (int i = 0; i < values.Count; i++)
        {
            var textures = CCL(noiseMap, values[i]);

            for (int j = 0; j < textures.Count; j++)
            {
                //find small provinces?
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {

                    }
                }
                //Creating Province and Country
                Sprite sprite = Sprite.Create(textures[j], new Rect(0, 0, width, height), new Vector2(.5f, .5f));
                GameObject province = new GameObject();
                GameObject country = new GameObject();
                province.name = "Province " + addingNumber;
                country.name = "Country " + addingNumber;
                addingNumber++;
                province.transform.parent = provinceParent;
                country.transform.parent = countryParent;
                province.AddComponent<Image>().sprite = sprite;
                province.AddComponent<Province>();
                country.AddComponent<Country>();
                province.GetComponent<Province>().owner = country.GetComponent<Country>();
                province.GetComponent<Province>().religion = country.GetComponent<Country>().religion;
                province.GetComponent<Province>().culture = country.GetComponent<Country>().culture;
                province.GetComponent<Province>().ideology = country.GetComponent<Country>().ideology;
                province.GetComponent<Province>().buildingCapacity = 12;
                province.GetComponent<Province>().buildingsParent = countryManager.buildingParent;
                country.GetComponent<Country>().capitalProvince = province.GetComponent<Province>();
                Color color = Random.ColorHSV(0f, 1f, 0.3f, 1f, 0.5f, 1f);
                country.GetComponent<Country>().countryColor = color;
                province.GetComponent<Image>().color = country.GetComponent<Country>().countryColor;
                countryManager.countries.Add(country.GetComponent<Country>());
                countryManager.provinces.Add(province.GetComponent<Province>());
                (province.transform as RectTransform).sizeDelta = new Vector2(width, height);
                province.transform.localPosition = Vector2.zero;
            }
        }
    }

    public List<Texture2D> CCL(float[,] noiseMap, float value)
    {

        int[,] matrix = new int[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                matrix[x, y] = noiseMap[x, y] == value ? 1 : 0;
            }
        }

        int[,] regionMatrix = new int[width, height];

        Dictionary<int, List<int>> equivalentRegions = new Dictionary<int, List<int>>();

        int currentRegion = 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (matrix[x, y] == 1)
                {
                    // Neighbors - Check blocks to the left and bottom of this one
                    List<(Vector2Int, int)> neighbors = new List<(Vector2Int, int)>();

                    if (y - 1 > -1)
                    {
                        if (matrix[x, y - 1] != 0)
                        {
                            neighbors.Add((new Vector2Int(x, y - 1), matrix[x, y - 1]));
                        }
                    }
                    if (x - 1 > -1)
                    {
                        if (matrix[x - 1, y] != 0)
                        {
                            neighbors.Add((new Vector2Int(x - 1, y), matrix[x - 1, y]));
                        }
                    }

                    // Process neighbors and add to matrix
                    int matchCount = neighbors.Count;
                    if (matchCount == 0)
                    {
                        regionMatrix[x, y] = currentRegion;
                        equivalentRegions.Add(currentRegion, new List<int>() { currentRegion });
                        currentRegion++;
                    }
                    else if (matchCount == 1)
                    {
                        regionMatrix[x, y] = regionMatrix[neighbors[0].Item1.x, neighbors[0].Item1.y];
                    }
                    else
                    {
                        List<int> distinctRegions = neighbors.Select(region => regionMatrix[region.Item1.x, region.Item1.y]).Distinct().ToList();

                        while (distinctRegions.Remove(0)) ;

                        if (distinctRegions.Count == 1)
                        {
                            regionMatrix[x, y] = distinctRegions[0];
                        }
                        else if (distinctRegions.Count > 1)
                        {
                            int firstRegion = distinctRegions[0];
                            regionMatrix[x, y] = firstRegion;

                            for (int i = 0; i < distinctRegions.Count; i++)
                            {
                                if (!equivalentRegions[firstRegion].Contains(distinctRegions[i]))
                                {
                                    equivalentRegions[firstRegion].Add(distinctRegions[i]);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Union-Find algorithm - Merge connected regions
        Dictionary<int, int> roots = new Dictionary<int, int>();

        foreach (int index in equivalentRegions.Keys)
        {
            for (int i = 0; i < equivalentRegions[index].Count; i++)
            {
                if (!roots.ContainsKey(equivalentRegions[index][i]))
                {
                    roots.Add(equivalentRegions[index][i], -1);
                }
            }
        }

        foreach (int index in equivalentRegions.Keys)
        {
            for (int i = 0; i < equivalentRegions[index].Count; i++)
            {
                roots[equivalentRegions[index][i]] = equivalentRegions[index][0];
            }
        }

        foreach (int index in equivalentRegions.Keys)
        {
            List<int> set = equivalentRegions[index];

            for (int i = 0; i < set.Count; i++)
            {
                for (int j = i + 1; j < set.Count; j++)
                {
                    // Unite
                    int item1Root = roots[set[i]];
                    for (int k = 0; k < roots.Count; k++)
                    {
                        int item = roots.Keys.ElementAt(k);

                        if (roots[item] == item1Root)
                        {
                            roots[item] = roots[set[j]];
                        }
                    }
                }
            }
        }

        equivalentRegions.Clear();

        foreach (var pair in roots)
        {
            if (equivalentRegions.ContainsKey(pair.Value))
            {
                equivalentRegions[pair.Value].Add(pair.Key);
            }
            else
            {
                equivalentRegions.Add(pair.Value, new List<int> { pair.Key });
            }
        }

        Dictionary<int, List<Vector2Int>> voronoiRegions = new Dictionary<int, List<Vector2Int>>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (regionMatrix[x, y] != 0)
                {
                    int number = regionMatrix[x, y];
                    foreach (var region in equivalentRegions)
                    {
                        if (region.Value.Contains(number))
                        {
                            number = region.Key;
                            break;
                        }
                    }

                    if (voronoiRegions.ContainsKey(number))
                    {
                        voronoiRegions[number].Add(new Vector2Int(x, y));
                    }
                    else
                    {
                        voronoiRegions.Add(number, new List<Vector2Int> { { new Vector2Int(x, y) } });
                    }
                }
            }
        }

        List<Texture2D> textures = new List<Texture2D>();

        foreach (var region in voronoiRegions)
        {
            Vector2Int minCoordinate = new Vector2Int(int.MaxValue, int.MaxValue);
            Vector2Int maxCoordinate = new Vector2Int(int.MinValue, int.MinValue);

            foreach (Vector2Int coordinate in region.Value)
            {
                if (coordinate.x < minCoordinate.x)
                {
                    minCoordinate.x = coordinate.x;
                }
                else if (coordinate.x > maxCoordinate.x)
                {
                    maxCoordinate.x = coordinate.x;
                }

                if (coordinate.y < minCoordinate.y)
                {
                    minCoordinate.y = coordinate.y;
                }
                else if (coordinate.y > maxCoordinate.y)
                {
                    maxCoordinate.y = coordinate.y;
                }
            }

            Color32[] regionMap = new Color32[width * height];
            //Color32[] regionMap = new Color32[(maxCoordinate.x - minCoordinate.x) * (maxCoordinate.y - minCoordinate.y)];
            Color color = Random.ColorHSV(0f, 1f, 0.3f, 1f, 0.5f, 1f);

            foreach (Vector2Int coordinate in region.Value)
            {
                regionMap[width * coordinate.y + coordinate.x] = color;
            }

            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels32(regionMap);
            texture.Apply();

            textures.Add(texture);
        }

        return textures;
    }

    public float[,] GenerateVoronoi()
    {
        TriangleNet.Geometry.Polygon polygon = new TriangleNet.Geometry.Polygon();

        for (int i = 0; i < points; i++)
        {
            double x = Random.Range(0, width);
            double y = Random.Range(0, height);
            polygon.Add(new TriangleNet.Geometry.Vertex(x, y));
        }

        ConstraintOptions options = new ConstraintOptions()
        {
            ConformingDelaunay = true,
        };

        TriangleNet.Mesh mesh = (TriangleNet.Mesh)polygon.Triangulate(options);

        var triangles = mesh.Triangles;

        //foreach (var triangle in triangles)
        //{
        //    for (int i = 0; i < triangle.vertices.Length; i++)
        //    {
        //        GameObject region = new GameObject();
        //        region.transform.parent = provinceParent;
        //        region.transform.localPosition = new Vector2((float)triangle.vertices[i].x, (float)triangle.vertices[i].y);
        //        region.AddComponent<Image>();
        //    }
        //}
        float[,] noiseMap = new float[width, height];
        List<float> noiseValues = new List<float>();

        foreach (var triangle in triangles)
        {
            noiseValues.Add(Random.value);
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = 0;
                foreach (var triangle in triangles)
                {
                    if (PointInTriangle(new Vector2(x, y), VertexToVector(triangle.vertices[0]), VertexToVector(triangle.vertices[1]), VertexToVector(triangle.vertices[2])))
                    {
                        noiseMap[x, y] = noiseValues[i];
                    }
                    i++;
                }
            }
        }

        //TriangleNet.Voronoi.StandardVoronoi voronoiMesh = new TriangleNet.Voronoi.StandardVoronoi(mesh);
        var oldVertices = mesh.vertices.Values.ToArray();
        Vector3[] vertices = new Vector3[oldVertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = VertexToVector(oldVertices[i]);
        }
        Mesh voronoiMesh = Voronoi.GenerateVoronoiMesh(vertices, Vector3.zero);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = 0;
                for (int j = 0; j < voronoiMesh.triangles.Length; j += 3)
                {
                    if (PointInTriangle(new Vector2(x, y), vertices[j], vertices[j + 1], vertices[j + 2]))
                    {
                        noiseMap[x, y] = noiseValues[i];
                    }
                    i++;
                }
            }
        }

        return noiseMap;
    }

    //public static bool IsInConvexPolygon(Point testPoint, Polygon polygon)
    //{
    //    //Check if a triangle or higher n-gon
    //    Debug.Assert(polygon.Length >= 3);
    //
    //    //n>2 Keep track of cross product sign changes
    //    var pos = 0;
    //    var neg = 0;
    //
    //    for (var i = 0; i < polygon.Count; i++)
    //    {
    //        //If point is in the polygon
    //        if (polygon[i] == testPoint)
    //            return true;
    //
    //        //Form a segment between the i'th point
    //        var x1 = polygon[i].X;
    //        var y1 = polygon[i].Y;
    //
    //        //And the i+1'th, or if i is the last, with the first point
    //        var i2 = i < polygon.Count - 1 ? i + 1 : 0;
    //
    //        var x2 = polygon[i2].X;
    //        var y2 = polygon[i2].Y;
    //
    //        var x = testPoint.X;
    //        var y = testPoint.Y;
    //
    //        //Compute the cross product
    //        var d = (x - x1)(y2 - y1) - (y - y1)(x2 - x1);
    //
    //        if (d > 0) pos++;
    //        if (d < 0) neg++;
    //
    //        //If the sign changes, then point is outside
    //        if (pos > 0 && neg > 0)
    //            return false;
    //    }
    //
    //    //If no change in direction, then on same side of all segments, and thus inside
    //    return true;
    //}

    //    bool inside_convex_polygon(Vector2 point,Vector2[] vertices)
    //    {
    //        previous_side = None
    //        int n_vertices = vertices.Length;
    //        for (int i = 0; i < length; i++)
    //        {
    //            Vector2 a = vertices[i];
    //            Vector2 b = vertices[(i + 1) % n_vertices];
    //            affine_segment = v_sub(b, a)
    //            affine_point = v_sub(point, a)
    //            current_side = get_side(affine_segment, affine_point)
    //            if current_side is None:
    //                return False #outside or over an edge
    //            elif previous_side is None: #first segment
    //                previous_side = current_side
    //            elif previous_side != current_side:
    //            return False
    //        }
    //    for n in xrange(n_vertices) :
    //        
    //    return True
    //    }
    //
    //def get_side(a, b) :
    //    x = cosine_sign(a, b)
    //    if x< 0:
    //        return LEFT
    //    elif x> 0: 
    //        return RIGHT
    //    else:
    //        return None
    //
    //def v_sub(a, b) :
    //    return (a[0]-b[0], a[1]-b[1])
    //
    //def cosine_sign(a, b):
    //    return a[0] b[1]-a[1] b[0]

    public Vector2 VertexToVector(Vertex vertex)
    {
        return new Vector2((float)vertex.x, (float)vertex.y);
    }

    float sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float d1, d2, d3;
        bool has_neg, has_pos;

        d1 = sign(pt, v1, v2);
        d2 = sign(pt, v2, v3);
        d3 = sign(pt, v3, v1);

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }
}