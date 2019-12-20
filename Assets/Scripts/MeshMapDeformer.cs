using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMapDeformer : MonoBehaviour
{
    private bool shouldUpdate = false;
    private Material deformedMeshMat; // Material of the object
    private Renderer curRenderer;

    [HideInInspector] public Texture2D deformedMeshTex; // Texture changing color

    public int textureSize = 64; // size that the texture will be for x and y

    private void Start()
    {
        deformedMeshTex = new Texture2D(textureSize, textureSize);
        deformedMeshTex.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < textureSize; i++) // setting the mesh to black
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
            curRenderer.material.SetTexture("_DisplacementMap", deformedMeshTex);
        }
    }

    private void Update()
    {
        if (shouldUpdate) // only send information to the gpu when needed
        {
            deformedMeshTex.Apply();
            shouldUpdate = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // When Colliding need to find out what pixels are being touched and then turn them white
        List<ContactPoint> points = new List<ContactPoint>();
        collision.GetContacts(points);

        Debug.Log(collision.contactCount);

        foreach (var contact in points)
        {
            Vector3 diff = this.transform.position - contact.point; // gets the position difference of the objects

            float rows = (curRenderer.bounds.size.x / textureSize) * diff.x;
            float cols = (curRenderer.bounds.size.z / textureSize) * diff.z;

            int x = textureSize - Mathf.RoundToInt(textureSize * rows);
            int y = textureSize - Mathf.RoundToInt(textureSize * cols);

            deformedMeshTex.SetPixel(x, y, Color.white);
            shouldUpdate = true;
        }
    }
}