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
        thisMat = GetComponent<MeshRenderer>().material;
        mainTex = thisMat.mainTexture;

        //get copy of draw mesh material
        tmpMat = new Material(shader);
        maskRenderTexture = new RenderTexture(mainTex.width, mainTex.height, 24, RenderTextureFormat.ARGB32);
        tmpMat.SetTexture("_RenderTex", maskRenderTexture);
        tmpMat.SetVector("_ScaleFactor", new Vector2(eraserGameObject.transform.localScale.x / transform.localScale.x, eraserGameObject.transform.localScale.y / transform.localScale.y));
        tmpMat.SetTexture("_EraserTex", eraserTexture);

        thisMat.SetTexture("_RenderTex", maskRenderTexture);

        RenderTexture.active = maskRenderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DrawOnRenderTexture();
            Debug.Log(maskRenderTexture);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            RenderTexture.active = maskRenderTexture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;
        }
    }

    private void DrawOnRenderTexture()
    {
        eraserGameObject.SetActive(true);
        Vector3 eraserPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.back * Camera.main.transform.position.z);

        eraserGameObject.transform.position = eraserPos;

        tmpMat.SetVector("_EraserPos", eraserPos - transform.position);

        tmpRenderTexture = RenderTexture.GetTemporary(mainTex.width, mainTex.height, 24, RenderTextureFormat.ARGB32);
        Graphics.Blit(maskRenderTexture, tmpRenderTexture, tmpMat, -1);

        Graphics.Blit(tmpRenderTexture, maskRenderTexture);
        tmpRenderTexture.Release();
    }
}
