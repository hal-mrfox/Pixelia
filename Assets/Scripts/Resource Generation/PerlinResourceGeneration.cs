using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine;

public class PerlinResourceGeneration : MonoBehaviour
{
    public ResourceType resourceType;

    [Space(10)]

    public Tile tile;
    public Tile[,] tiles;
    public int numTiles;

    [Space(10)]
    public bool randomSeed;
    public NoiseSettings settings;

    [Button("Generate")]
    public void Generate()
    {
        if (randomSeed)
        {
            settings.seed = Random.Range(-100000, 100000);
        }
        Rect rect = ((RectTransform)transform).rect;
        float[,] noiseMap = ProcessNoiseMap(GenerateNoiseMap(settings), settings.threshold);
        GetComponent<Image>().sprite = Sprite.Create(GenerateResourceTexture(noiseMap), new Rect(0, 0, (int)rect.width, (int)rect.height), new Vector2(.5f, .5f));
        //GetComponent<Image>().color = Random.ColorHSV();

        //set province resources
        #region shit province resource generation lmoa
        //for (int y = 0; y < (int)rect.height; y++)
        //{
        //    for (int x = 0; x < (int)rect.width; x++)
        //    {
        //        if (noiseMap[x, y] > 0)
        //        {
        //            Vector2 worldPosition = transform.TransformPoint(new Vector2(x, y));
        //            worldPosition.x = Mathf.Round(worldPosition.x);
        //            worldPosition.y = Mathf.Round(worldPosition.y);
        //
        //            for (int i = 0; i < CountryManager.instance.provinces.Count; i++)
        //            {
        //                RectTransform rectTransform = (RectTransform)CountryManager.instance.provinces[i].transform;
        //
        //                for (int j = 0; j < rectTransform.rect.height; j++)
        //                {
        //                    for (int k = 0; k < rectTransform.rect.width; k++)
        //                    {
        //                        Vector2 provincePosition = rectTransform.TransformPoint(new Vector2(j, k));
        //                        provincePosition.x = Mathf.Round(provincePosition.x);
        //                        provincePosition.y = Mathf.Round(provincePosition.y);
        //
        //                        if (provincePosition == worldPosition)
        //                        {
        //                            CountryManager.instance.provinces[i].rawResources[(int)resourceType].resourceCount += 1;
        //
        //                            goto NextLoop;
        //                        }
        //                    }
        //                }
        //            
        //                NextLoop:;
        //            }
        //        }
        //    }
        //}
        #endregion
    }

    [System.Serializable]
    public struct NoiseSettings
    {
        public int seed;
        public float scale;
        public int octaves;
        public float persistence;
        [Range(0,1)]public float threshold;
        public float lacunarity;
        public float heightOffset;
        public Vector2 offset;
    }

    #region Generation
    #region Generate Noise Map
    public float[,] GenerateNoiseMap(NoiseSettings noiseSetting)
    {
        FastNoise noise = new FastNoise(noiseSetting.seed);
        Rect rect = ((RectTransform)transform).rect;
        float[,] noiseMap = new float[(int)rect.width, (int)rect.height];
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
        float halfWidth = (int)rect.width * 0.5f;
        float halfHeight = (int)rect.height * 0.5f;

        for (int y = 0; y < (int)rect.height; y++)
        {
            for (int x = 0; x < (int)rect.width; x++)
            {
                amplitude = 1;
                frequency = 1;

                float noiseHeight = 0;

                for (int i = 0; i < noiseSetting.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / noiseSetting.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / noiseSetting.scale * frequency;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
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

        for (int y = 0; y < (int)rect.height; y++)
        {
            for (int x = 0; x < (int)rect.width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
    #endregion
    #region Generate Resource Texture
    public Texture2D GenerateResourceTexture(float[,] heightMap)
    {
        Rect rect = ((RectTransform)transform).rect;
        Color32[] colorMap = new Color32[(int)rect.width * (int)rect.height];

        if (tiles != null)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    Destroy(tiles[x, y].gameObject);
                }
            }
        }

        numTiles = 0;

        tiles = new Tile[(int)rect.width / 4, (int)rect.height / 4];

        //change to y += tile size
        for (int y = 0; y < (int)rect.height; y += 4)
        {
            for (int x = 0; x < (int)rect.width; x += 4)
            {
                int i = x / 4;
                int j = y / 4;
                tiles[i, j] = Instantiate(tile, transform).GetComponent<Tile>();
                tiles[i, j].image.color = Color.Lerp(Color.clear, Color.white, heightMap[i, j]);
                tiles[i, j].transform.sizeDelta = new Vector2(4, 4);
                tiles[i, j].transform.position = new Vector2(i + 2, j + 2) + (Vector2)transform.position - rect.size / 2;
            }
        }

        for (int y = 0; y < tiles.GetLength(1); y++)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                numTiles++;
            }
        }

        Texture2D texture = new Texture2D((int)rect.width, (int)rect.height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels32(colorMap);
        texture.Apply();

        return texture;
    }
    #endregion
    #region Process Noisemap
    public float[,] ProcessNoiseMap(float[,] noiseMap, float threshold)
    {
        Rect rect = ((RectTransform)transform).rect;

        for (int y = 0; y < (int)rect.height; y++)
        {
            for (int x = 0; x < (int)rect.width; x++)
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
    #endregion
    #endregion
}
