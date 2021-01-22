using System.Collections;
using NaughtyAttributes;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Image mapPreview;
    public Image voronoi;
    public bool autoGenerate;
    [Range(0, 1)] public float threshold;
    public AnimationCurve falloffCurve;
    public AnimationCurve squareFalloffCurve;
    public int width;
    public int height;
    [Range(0, 0.5f)] public float jitter;
    //public Gradient colorMap;
    public NoiseSettings simplexSettings;
    public NoiseSettings voronoiSettings;

    [System.Serializable]
    public struct NoiseSettings
    {
        public bool voronoi;
        public int seed;
        public float scale;
        public int octaves;
        public float persistence;
        public float lacunarity;
        public float heightOffset;
        public Vector2 offset;
    }

    [Button]
    public void GenerateMap()
    {
        simplexSettings.seed = Random.Range(-100000, 100000);
        voronoiSettings.seed = Random.Range(-100000, 100000);
        float[,] noiseMap = ProcessNoiseMap(AddFalloffMap(GenerateNoiseMap(simplexSettings)));
        mapPreview.sprite = Sprite.Create(GenerateTexture(noiseMap), new Rect(0, 0, width, height), new Vector2(.5f, .5f));
        voronoi.sprite = Sprite.Create(GenerateTexture(VoronoiLand(noiseMap, GenerateNoiseMap(voronoiSettings))), new Rect(0, 0, width, height), new Vector2(.5f, .5f));
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

    public float[,] VoronoiLand(float[,] noiseMap, float[,] voronoiMap)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (noiseMap[x, y] == 1)
                {
                    voronoiMap[x, y] = 1;
                }
            }
        }

        return voronoiMap;
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

    public float[,] ProcessNoiseMap(float[,] noiseMap)
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
}