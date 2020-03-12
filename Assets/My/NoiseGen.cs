using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGen : MonoBehaviour
{
    public Texture2D heightMap;
    public int pixelNum=128;
    private Vector3 volume;
    public bool autoUpdate;
    public Vector3 offset;
    public float WorelyWeight;
    private Vector2[,] pos2D ;
    private SimplexNoise noise=new SimplexNoise(0);
    private RenderTexture target;
    [Header("Perlin Setting")]
    public float perlinFrequency;
    public float octave;
    public Texture3D perlinTexture;
    [Header("Worely Setting")]
    public Texture3D worelyTexture;
    public ComputeShader worelyShader;
    public int cellAxisNum = 10;
    public float radius = 1f;
    [Header("Blend Setting")]
    [Range(0,1)]
    public float worelyWeight;
    public ComputeShader blendShader;
    public Texture3D cloud;
    public void Awake()
    {
        GenWorelyNoise();
        Blend();
    }
    public void Blend()
    {
        blendShader.SetTexture(0, "perlin", perlinTexture);
        blendShader.SetTexture(0, "worely", worelyTexture);
        blendShader.SetFloat("worelyWeight", worelyWeight);
        CreateRenderTexture(ref target);
        CreateTexture(ref cloud);
        blendShader.SetTexture(0, "Result", target);
        int threadGroupsX = Mathf.CeilToInt(pixelNum * 1.0f / 8);
        int threadGroupsY = Mathf.CeilToInt(pixelNum * 1.0f / 8);
        int threadGroupsZ = Mathf.CeilToInt(pixelNum * 1.0f / 8);
        blendShader.Dispatch(0, threadGroupsX, threadGroupsY, threadGroupsZ);
        Graphics.CopyTexture(target, cloud);
        //cloud.Apply();
    }
    public void GenPerlinNoise()
    {
        CreateTexture(ref perlinTexture);
        Color32[] colorArr = new Color32[pixelNum * pixelNum*pixelNum];
        for (int channel = 0; channel < 4; channel++)
        {
            float amplitude = .5f;
            float nowFrequency = perlinFrequency*(2<<channel);
            for (int i = 0; i < octave; i++)
            {
                for (int z = 0; z < pixelNum; z++)
                {
                    for (int y = 0; y < pixelNum; y++)
                        for (int x = 0; x < pixelNum; x++)
                        {
                            Color32 target = colorArr[z * pixelNum * pixelNum + y * pixelNum + x];
                            float perlinVal = amplitude * (float)(noise.Evaluate(x * nowFrequency, y * nowFrequency, z * nowFrequency) + 1) / 2;
                            byte res = (byte)(perlinVal * 255);
                            switch (channel)
                            {
                                case 0:
                                    target.r += res;
                                    break;
                                case 1:
                                    target.g += res;
                                    break;
                                case 2:
                                    target.b += res;
                                    break;
                                case 3:
                                    target.a += res;
                                    break;
                            }
                            colorArr[z * pixelNum * pixelNum + y * pixelNum + x] = target;
                        }
                }
                if (i != octave - 1)
                    amplitude *= .5f;
                nowFrequency *= 2;
            }
        }
        perlinTexture.SetPixels32(colorArr);
        perlinTexture.Apply();
    }
    void CreateRenderTexture(ref RenderTexture texture)
    {
        if (texture == null || texture.width != pixelNum || !texture.IsCreated())
        {
            if (texture != null)
                texture.Release();
            texture = new RenderTexture(pixelNum, pixelNum, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true,
                volumeDepth = pixelNum,
                dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
                wrapMode = TextureWrapMode.Repeat,
            };
            texture.Create();
        }
    }
    void CreateTexture(ref Texture3D texture)
    {
        if (texture == null || texture.width != pixelNum)
        {
            texture = new Texture3D(pixelNum, pixelNum, pixelNum, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Repeat
            };
        }
    }
    public void GenWorelyNoise()
    {
        CreateRenderTexture(ref target);
        CreateTexture(ref worelyTexture);
        worelyShader.SetInt("cellAxisNum", cellAxisNum);
        worelyShader.SetFloat("radius", radius);
        worelyShader.SetTexture(0, "Result", target);
        int threadGroupsX = Mathf.CeilToInt(pixelNum* 1.0f / 8);
        int threadGroupsY = Mathf.CeilToInt(pixelNum* 1.0f / 8);
        int threadGroupsZ = Mathf.CeilToInt(pixelNum* 1.0f / 8);
        worelyShader.Dispatch(0, threadGroupsX, threadGroupsY, threadGroupsZ);
        Graphics.CopyTexture(target, worelyTexture);
        //worelyTexture.Apply();
    }

}


