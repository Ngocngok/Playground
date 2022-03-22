using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawable : MonoBehaviour
{
    [SerializeField]
    private GameObject eraserGameObject;
    [SerializeField]
    private Texture eraserTexture;

    private Material thisMat;
    private SpriteRenderer thisSpriteRenderer;
    private RenderTexture drawRenderTexture;
    private RenderTexture tmpRenderTexture;

    private Texture2D originTexture;

    void Start()
    {
        //init
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        thisMat = thisSpriteRenderer.material;

        //create draw render texture
        drawRenderTexture = new RenderTexture(Screen.width * 4 / 5, Screen.height * 4 / 5, 24, RenderTextureFormat.ARGB32);

        //create new texture for sprite renderer
        originTexture = new Texture2D(drawRenderTexture.width, drawRenderTexture.height);
        RenderTexture.active = drawRenderTexture;
        originTexture.ReadPixels(new Rect(0, 0, originTexture.width, originTexture.height), 0, 0);
        RenderTexture.active = null;

        //create sprite for this sprite renderer
        thisSpriteRenderer.sprite = Sprite.Create(originTexture, new Rect(0, 0, drawRenderTexture.width, drawRenderTexture.height), new Vector2(.5f, .5f));


        thisMat.SetTexture("_MainTex", drawRenderTexture);

        //GetComponent<SpriteRenderer>().sprite.texture = maskRenderTexture;

        thisMat.SetVector("_ScaleFactor", new Vector2(eraserGameObject.transform.localScale.x / transform.localScale.x, eraserGameObject.transform.localScale.y / transform.localScale.y));
        thisMat.SetTexture("_BrushTex", eraserTexture);

        thisMat.SetTexture("_MainTex", drawRenderTexture);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DrawOnRenderTexture();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            RenderTexture.active = drawRenderTexture;
            //GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;
        }
    }

    private void DrawOnRenderTexture()
    {
        eraserGameObject.SetActive(true);
        Vector3 eraserPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.back * Camera.main.transform.position.z);

        eraserGameObject.transform.position = eraserPos;

        thisMat.SetVector("_BrushPos", eraserPos - transform.position);

        tmpRenderTexture = RenderTexture.GetTemporary(drawRenderTexture.width, drawRenderTexture.height, 24, RenderTextureFormat.ARGB32);
        Graphics.Blit(drawRenderTexture, tmpRenderTexture, thisMat, -1);

        Graphics.Blit(tmpRenderTexture, drawRenderTexture);
        tmpRenderTexture.Release();
    }
}
