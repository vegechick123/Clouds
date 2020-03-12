using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(NoiseGen))]
public class MyNoiseGenEditor : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        NoiseGen noiseGen = (NoiseGen)target;

        if (GUILayout.Button("Blend") || (DrawDefaultInspector() && noiseGen.autoUpdate))
        {
            noiseGen.Blend();
        }
        if (GUILayout.Button("GenerateWorely"))
        {
            noiseGen.GenWorelyNoise();
        }
        if (GUILayout.Button("GeneratePerlin") )
        {
            noiseGen.GenPerlinNoise();
        }
        

    }
}
