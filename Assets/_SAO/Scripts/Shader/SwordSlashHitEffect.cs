using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlashHitEffect : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Shader drawShader;
    [SerializeField] private Material mainMat;

    public RenderTexture splatmap;
    private Material drawMat;

    public float brushStrength, brushSize;

    private MeshRenderer meshRender;
    private RaycastHit hit;

    private void Start()
    {
        meshRender = GetComponent<MeshRenderer>();
        splatmap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        drawMat = new Material(drawShader);


        drawMat.SetVector("DrawColor", Color.red);
        drawMat.SetTexture("Splatmap", splatmap);
        mainMat.SetTexture("Splatmap", splatmap);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //_drawMaterial.SetFloat("BrushStrength", brushStrength);
                //_drawMaterial.SetFloat("BrushSize", brushSize);
                RenderTexture _snow = (RenderTexture)meshRender.material.GetTexture( "Splatmap");

                //drawMat.SetVector("Coordinate", new Vector4(_snow.textureCoord.x, _snow.textureCoord.y, 0, 0));
                RenderTexture temp = RenderTexture.GetTemporary(splatmap.width, splatmap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(splatmap, temp);
                Graphics.Blit(temp, splatmap, drawMat);
                RenderTexture.ReleaseTemporary(temp);
            }
        }

    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), splatmap, ScaleMode.ScaleToFit, false, 1);
    }
}