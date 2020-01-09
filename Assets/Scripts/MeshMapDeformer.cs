using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMapDeformer : MonoBehaviour
{
    private bool shouldUpdate = false;
    private int radius = 10;
    private Material deformedMeshMat; // Material of the object
    private Renderer curRenderer;

    [HideInInspector] public Texture2D deformedMeshTex; // Texture changing color

    public int textureSize = 64; // size that the texture will be for x and y
    public bool rainbowPixels = false;
    public bool DisplacementMapMainTexture = false;
    public Vector4 colorChangeAmount = new Vector4(0.1f,0.1f,0.1f,0);

    private void Start()
    {
        deformedMeshTex = new Texture2D(textureSize, textureSize);
        deformedMeshTex.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < textureSize; i++) // setting the texture to black
        {
            for (int j = 0; j < textureSize; j++)
            {
                deformedMeshTex.SetPixel(i, j, Color.black);
            }
        }
        deformedMeshTex.Apply();

        if (TryGetComponent<Renderer>(out Renderer rend))
        {
            curRenderer = rend;
            if (DisplacementMapMainTexture == false)
            {
                curRenderer.material.SetTexture("_DisplacementMap", deformedMeshTex); // setting the displacement the newly blacked out texture
            }
            else
            {
                curRenderer.material.SetTexture("_MainTex", deformedMeshTex); // setting the displacement the newly blacked out texture
            }
        }

        if (rainbowPixels)
        {
            PixelColorSizeHelp();
        }
    }

    private void Update()
    {
        ReturningDisplacement();

        if (shouldUpdate) // only send information to the gpu when needed
        {
            deformedMeshTex.Apply();
            shouldUpdate = false;
        }
    }

    // helps visual the pixels
    private void PixelColorSizeHelp()
    {
        Texture2D rainbow = new Texture2D(textureSize, textureSize);

        Color[] colorArr = new Color[9];
        colorArr[0] = Color.black;
        colorArr[1] = Color.blue;
        colorArr[2] = Color.cyan;
        colorArr[3] = Color.gray;
        colorArr[4] = Color.green;
        colorArr[5] = Color.magenta;
        colorArr[6] = Color.red;
        colorArr[7] = Color.white;
        colorArr[8] = Color.yellow;

        for (int i = 0; i < textureSize; i++) // setting the mesh to black
        {
            for (int j = 0; j < textureSize; j++)
            {
                rainbow.SetPixel(i, j, colorArr[Random.Range(0, 9)]);
            }
        }

        rainbow.Apply();
        curRenderer.material.SetTexture("_MainTex", rainbow);
    }

    private void ReturningDisplacement() // returning the color back to black
    {
        Color tempColorChange;

        for (int i = 0; i < textureSize; i++)
        {
            for(int j = 0; j < textureSize; j++)
            {
                tempColorChange = deformedMeshTex.GetPixel(i, j) - new Color(colorChangeAmount.x, colorChangeAmount.y, colorChangeAmount.z);

                deformedMeshTex.SetPixel(i, j, tempColorChange);
            }
        }

        deformedMeshTex.Apply();
    }

    private void OnCollisionStay(Collision collision)
    {
        // When Colliding need to find out what pixels are being touched and then turn them white
        List<ContactPoint> points = new List<ContactPoint>();
        int contactCount = collision.GetContacts(points);

        for(int i = 0; i < contactCount; i++)
        {
            RaycastHit hit;

            // raycasting from middle of object in direction of contacts normal
            if (Physics.Raycast(points[i].otherCollider.gameObject.transform.position, points[i].normal, out hit))
            {
                // grabs the shader uvs
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= textureSize;
                pixelUV.y *= textureSize;

                // setting the pixels to white where they are colliding with
                deformedMeshTex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.white);
                shouldUpdate = true;
            }
        }
    }
}