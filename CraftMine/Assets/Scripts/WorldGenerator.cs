using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    public enum colorType {
        Grayscale,
        Colors
    }

    public colorType colortype;

    [Range(2, 128)]
    public int length = 64;

    public int depth = 128;

    [Range(0, 128)]
    public int z = 0;

    public int waterLevel = 64;

    public float frequency = 10f;

    [Range(1, 8)]
    public int octaves = 2;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    [Range(1, 3)]
    public int dimensions = 2;

    public NoiseMethodType type;

    public Material Air;
    public Material Land;
    public Material Stone;
    public Material Bedrock;

    private Texture2D texture;

    private void Start() {
        SpawnBlock();
    }

    private void OnEnable() {
        if (texture == null) {
            texture = new Texture2D(length, length, TextureFormat.RGB24, true);
            texture.name = "Procedural Texture";
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            texture.anisoLevel = 9;
            //GetComponent<MeshRenderer>().material.mainTexture = texture;
            //FillTexture();
        }
    }

    private void Update() {
        if (transform.hasChanged) {
            transform.hasChanged = false;
            //FillTexture();
        }
    }

    public void SpawnBlock() {
        Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, 0, -0.5f));
        Vector3 point10 = transform.TransformPoint(new Vector3(0.5f, 0, -0.5f));
        Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0, 0.5f));
        Vector3 point11 = transform.TransformPoint(new Vector3(0.5f, 0, 0.5f));

        NoiseMethod method = Noise.noiseMethods[(int)type][dimensions - 1];
        float stepSize = 1f / length;
        for (int z = 0; z < length; z++) {
            Vector3 point0 = Vector3.Lerp(point00, point01, (z + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (z + 0.5f) * stepSize);
            for (int x = 0; x < length; x++) {
                Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
                if (type != NoiseMethodType.Value) {
                    sample = sample * 0.5f + 0.5f;
                }
                sample *= depth;
                //PlaceBlock(sample, x, y, 64);
                for (int i = 50; i < depth; i++) {
                    PlaceBlock(sample, x, i, z);
                }
            }
        }
    }

    private void PlaceBlock(float noiseSample, int x, int y, int z) {

        int dirtLayer = Mathf.FloorToInt(noiseSample);
        int dirtThickness = dirtLayer / 32 - 6;
        int stoneLayer = dirtLayer + dirtThickness;

        if (y == 0) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = Bedrock;
            cube.transform.position = new Vector3(x, y, z);
        } else if (y <= stoneLayer) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = Stone;
            cube.transform.position = new Vector3(x, y, z);
        } else if (y <= dirtLayer) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = Land;
            cube.transform.position = new Vector3(x, y, z);
        } else return;
    }

    public void FillTexture() {
        if (texture.width != length) {
            texture.Resize(length, length);
        }

        Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
        Vector3 point10 = transform.TransformPoint(new Vector3(0.5f, -0.5f));
        Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
        Vector3 point11 = transform.TransformPoint(new Vector3(0.5f, 0.5f));

        NoiseMethod method = Noise.noiseMethods[(int)type][dimensions - 1];
        float stepSize = 1f / length;
        for (int y = 0; y < length; y++) {
            Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
            for (int x = 0; x < length; x++) {
                Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
                if (type != NoiseMethodType.Value) {
                    sample = sample * 0.5f + 0.5f;
                }
                sample *= depth;
                if (colortype == colorType.Colors) {
                    texture.SetPixel(x, y, BlockColor(sample, z));
                } else {
                    texture.SetPixel(x, y, Color.white * sample);
                }
                
            }
        }
        texture.Apply();
    }

    private Color BlockColor(float noiseSample, int z) {
        Color color = Color.white;

        int dirtLayer = Mathf.FloorToInt(noiseSample);
        int dirtThickness = dirtLayer / 32 - 6;
        int stoneLayer = dirtLayer + dirtThickness;

        if (z == 0)
            color = Color.black;
        else if (z <= stoneLayer)
            color = Color.gray;
        else if (z <= dirtLayer)
            color = Color.green;


        return color;
    }

}
