using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    public enum colorType {
        Grayscale,
        Colors
    }

    public colorType colortype;

    [Range(2, 512)]
    public int resolution = 128;

    public float frequency = 10f;

    [Range(1, 8)]
    public int octaves = 2;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    [Range(1, 3)]
    public int dimensions = 3;

    public NoiseMethodType type;

    public TerrainType[] blocks;

    private Texture2D texture;

    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float cutOffHeight;
        public Color color;
    }

    private void OnEnable() {
        if (texture == null) {
            texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
            texture.name = "Procedural Texture";
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            texture.anisoLevel = 9;
            GetComponent<MeshRenderer>().material.mainTexture = texture;
            FillTexture();
        }
    }

    private void Update() {
        if (transform.hasChanged) {
            transform.hasChanged = false;
            FillTexture();
        }
    }

    public void FillTexture() {
        if (texture.width != resolution) {
            texture.Resize(resolution, resolution);
        }

        Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
        Vector3 point10 = transform.TransformPoint(new Vector3(0.5f, -0.5f));
        Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
        Vector3 point11 = transform.TransformPoint(new Vector3(0.5f, 0.5f));

        NoiseMethod method = Noise.noiseMethods[(int)type][dimensions - 1];
        float stepSize = 1f / resolution;
        for (int y = 0; y < resolution; y++) {
            Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
            for (int x = 0; x < resolution; x++) {
                Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
                if (type != NoiseMethodType.Value) {
                    sample = sample * 0.5f + 0.5f;
                }
                if (colortype == colorType.Colors) {
                    texture.SetPixel(x, y, BlockColor(sample));
                } else {
                    texture.SetPixel(x, y, Color.white * sample);
                }
                
            }
        }
        texture.Apply();
    }

    private Color BlockColor(float noiseSample) {
        Color color = new Color();

        for (int i = 0; i < blocks.Length; i++) {
            if (noiseSample <= blocks[i].cutOffHeight) {
                color = blocks[i].color;
                break;
            }
        }
        return color;
    }

}
