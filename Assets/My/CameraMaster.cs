using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMaster : MonoBehaviour
{
    // Start is called before the first frame update
    public ComputeShader shader;
    public Texture background;
    public Transform Container;
    public NoiseGen noiseGen;
    private Camera _camera;
    private RenderTexture target;
    private Material material;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode=DepthTextureMode.Depth;
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
        Vector3 lightPos = Shader.GetGlobalVector("_WorldSpaceLightPos0");
        shader.SetVector("lightDir", lightPos);
        shader.SetVector("lightCol", new Vector3(1.0f,1.0f,1.0f));
        shader.SetTextureFromGlobal(0, "Depth", "_CameraDepthTexture");
        shader.SetTexture(0, "Cloud", noiseGen.clound);
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
            target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create();
        }
    }
}
