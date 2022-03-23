using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erasable : MonoBehaviour
{
    [SerializeField]
    private GameObject eraserGameObject;
    [SerializeField]
    private Shader shader;
    [SerializeField]
    private Texture eraserTexture;

    private Material thisMat;
    private Texture mainTex;
    private RenderTexture maskRenderTexture;
    private RenderTexture tmpRenderTexture;

    private Material tmpMat;


    void Start()
    {
        thisMat = GetComponent<SpriteRenderer>().material;
        //mainTex = thisMat.GetTexture("_MainTex");
        mainTex = GetComponent<SpriteRenderer>().sprite.texture;

        //get copy of draw mesh material
        tmpMat = new Material(shader);
        maskRenderTexture = new RenderTexture(mainTex.width, mainTex.height, 24, RenderTextureFormat.ARGB32);
        tmpMat.SetTexture("_RenderTex", maskRenderTexture);
        tmpMat.SetVector("_ScaleFactor", new Vector2(eraserGameObject.transform.localScale.x / transform.localScale.x, eraserGameObject.transform.localScale.y / transform.localScale.y));
        tmpMat.SetTexture("_BrushTex", eraserTexture);

        thisMat.SetTexture("_RenderTex", maskRenderTexture);

        RenderTexture.active = maskRenderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }

    private Vector3 prevPos;

    private Vector3 currentPos;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            prevPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.back * Camera.main.transform.position.z);
        else if (Input.GetMouseButton(0))
        {
            currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.back * Camera.main.transform.position.z);
            DrawOnRenderTexture();
            prevPos = currentPos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //RenderTexture.active = maskRenderTexture;
            //GL.Clear(true, true, Color.clear);
            //RenderTexture.active = null;
        }
    }

    private void DrawOnRenderTexture()
    {
        eraserGameObject.SetActive(true);

        eraserGameObject.transform.position = currentPos;

        tmpMat.SetVector("_BrushPos", currentPos - transform.position);
        tmpMat.SetVector("_PrevPos", prevPos - transform.position);

        tmpRenderTexture = RenderTexture.GetTemporary(mainTex.width, mainTex.height, 24, RenderTextureFormat.ARGB32);

        RenderTexture.active = tmpRenderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;

        Graphics.Blit(maskRenderTexture, tmpRenderTexture, tmpMat, -1);

        Graphics.Blit(tmpRenderTexture, maskRenderTexture);
        tmpRenderTexture.Release();
    }
}
