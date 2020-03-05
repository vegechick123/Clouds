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

        if (GUILayout.Button("Generate")||(DrawDefaultInspector()&&noiseGen.autoUpdate))
        {
            noiseGen.Gen();
        }

    }
}
