using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CameraMaster : MonoBehaviour
{
    // Start is called before the first frame update
    public ComputeShader shader;
    public Texture background;
    public Transform Container;
    public float lowbound = 0;
    public float scartterX=0;
    public float extinctionX = 0;
    public float densityScale;
    [Range(-1,1)]
    public float HGg;
    [Range(0,0.01f)]
    public float sampleScale;
    public float speed;
    public Color ambientCol;
    public float ambientStrength;
    public float erodeStrength;
    public float Hgmin;
    private Vector3 offset=new Vector3(0,0,0);
    public NoiseGen noiseGen;
    private Camera _camera;
    private RenderTexture target;

    public int channel = 0;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode=DepthTextureMode.Depth;
    }
    private void Update()
    {
        offset.x += Time.deltaTime * speed;
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters(source);
        Render(destination);
    }
    
    private void SetShaderParameters(RenderTexture source)
    {
        shader.SetMatrix("CameraToWorld", _camera.cameraToWorldMatrix);
        shader.SetMatrix("CameraInverseProjection", _camera.projectionMatrix.inverse);
        shader.SetVector("containerSize", Container.lossyScale);
        shader.SetVector("containerPos", Container.position);
        shader.SetVector("sampleScale", sampleScale*new Vector3(1,2,1));
        shader.SetFloat("lowbound", lowbound);
        shader.SetFloat("scartterX", scartterX);
        shader.SetFloat("extinctionX", extinctionX);
        shader.SetFloat("densityScale", densityScale);
        shader.SetFloat("ambientStrength", ambientStrength);
        shader.SetFloat("erodeStrength", erodeStrength);
        shader.SetFloat("HGg", HGg);
        shader.SetFloat("Hgmin", Hgmin);
        shader.SetInt("channel", channel);
        Vector3 lightPos = Shader.GetGlobalVector("_WorldSpaceLightPos0");
        shader.SetVector("lightDir", lightPos);
        shader.SetVector("lightCol", new Vector3(1.0f,1.0f,1.0f));
        shader.SetVector("ambientCol", ambientCol);
        shader.SetVector("offset", offset);
        shader.SetTextureFromGlobal(0, "Depth", "_CameraDepthTexture");
        shader.SetTexture(0, "Cloud", noiseGen.cloud);
        shader.SetTexture(0, "Worely", noiseGen.worelyTexture);
        
        shader.SetTexture(0, "Source", source);
    }
    private void Render(RenderTexture destination)
    {
        InitRenderTexture();
        shader.SetTexture(0, "Result", target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width*1.0f / 8);
        int threadGroupsY = Mathf.CeilToInt(Screen.height*1.0f / 8);
        shader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        Graphics.Blit(target, destination);
    }
    private void InitRenderTexture()
    {
        if (target == null)
        {
            if (target != null)
                target.Release();
            target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };
            target.Create();
        }

    }
}
