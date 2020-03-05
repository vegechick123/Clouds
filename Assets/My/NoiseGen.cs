using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGen : MonoBehaviour
{
    public Texture3D clound;
    public float voxelPerUnit = 1;
    private Vector3 volume;
    public bool autoUpdate;

    public float frequency;
    public Vector3 offset;
    public void Gen()
    {
        Vector3 volume = transform.lossyScale;
        Vector3Int size = new Vector3Int(Mathf.CeilToInt(volume.x*voxelPerUnit), Mathf.CeilToInt(volume.y * voxelPerUnit), Mathf.CeilToInt(volume.z * voxelPerUnit));
        clound = new Texture3D(size.x, size.y, size.z, TextureFormat.RGBA32,true);
        Color32[] colorArr = new Color32[size.x*size.y*size.z];
        for(int x=0;x<size.x;x++)
            for(int y=0;y<size.y;y++)
                for(int z=0;z<size.z;z++)
                {
                    float dx = (x +offset.x)* frequency;
                    float dy = (y+offset.y) * frequency;
                    float dz = (z+offset.z) * frequency;
                    colorArr[x * size.y * size.z + y * size.z + z] = PerlinNoise3D.Noise(dx,dy,dz) * Color.white;

                }

        clound.SetPixels32(colorArr);
        clound.Apply();
    }
}
