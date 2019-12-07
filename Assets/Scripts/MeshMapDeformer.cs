using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMapDeformer : MonoBehaviour
{
    [HideInInspector] public Material deformedMeshMat;
    [HideInInspector] public Texture2D deformedMeshTex;

    public int textureSize = 64;

    // Start is called before the first frame update
    void Start()
    {
        deformedMeshTex = new Texture2D(textureSize, textureSize);
        

        for(int i = 0; i < textureSize; i++) // setting the mesh to black
        {
            for(int j = 0; j < textureSize; j++)
            {
                deformedMeshTex.SetPixel(i, j, Color.black);
            }
        }
        deformedMeshTex.Apply();

        if(TryGetComponent<Material>(out Material mat))
        {
            mat.SetTexture(Shader.PropertyToID("deformedMeshTex"), deformedMeshTex);
        }
    }
}