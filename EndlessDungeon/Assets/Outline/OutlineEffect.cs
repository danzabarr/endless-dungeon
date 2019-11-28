using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace deprecated
{
public class OutlineEffect : MonoBehaviour
{



    public bool Gauss;

    public Material WhiteMaker;
    public Material PostOutline;
    public Material GaussianBlur;

    private Camera main;
    private Camera cam;

    public RawImage image;
    private RenderTexture TempRT, whiteRT;

    void Start()
    {
        main = Camera.main;
        cam = GetComponent<Camera>();

        CreateTextures();
    }

    private void ReleaseTextures()
    {
        RenderTexture.ReleaseTemporary(TempRT);
        RenderTexture.ReleaseTemporary(whiteRT);
        cam.targetTexture.Release();
    }

    private void CreateTextures()
    {
        int width = main.pixelWidth;
        int height = main.pixelHeight;

        cam.targetTexture = new RenderTexture(width, height, 24);
        whiteRT = RenderTexture.GetTemporary(width, height, 24);
        TempRT = RenderTexture.GetTemporary(width, height, 24);
        image.texture = cam.targetTexture;
    }

    private void ClearTemporaryTextures()
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = TempRT;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = whiteRT;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = active;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int width = main.pixelWidth;
        int height = main.pixelHeight;

        if (cam.targetTexture.width != width || cam.targetTexture.height != height)
        {
            ReleaseTextures();
            CreateTextures();
        }

        ClearTemporaryTextures();

        Graphics.Blit(source, whiteRT, WhiteMaker);
        if (Gauss)
        {
            Graphics.Blit(whiteRT, TempRT, PostOutline);
            Graphics.Blit(TempRT, destination, GaussianBlur);
        }
        else
        {
            Graphics.Blit(whiteRT, destination, PostOutline);
        }

        //RenderTexture.ReleaseTemporary(TempRT);
        //RenderTexture.ReleaseTemporary(whiteRT);
    }
}
}