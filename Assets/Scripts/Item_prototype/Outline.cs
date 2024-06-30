using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    Material outline;
    Renderer renderers;
    List<Material> materialList = new List<Material>();

    void Start()
    {
        outline = new Material(Shader.Find("Unlit/OutlineShader"));
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        
        if(Input.GetMouseButtonUp(0))
        {
            OnMouseUp();

        }
    }

    void OnMouseDown()
    {
        renderers = this.GetComponent<Renderer>();

        materialList.Clear();
        materialList.AddRange(renderers.sharedMaterials);
        materialList.Add(outline);

        renderers.materials = materialList.ToArray();
    }

    void OnMouseUp()
    {
        Renderer renderer = this.GetComponent<Renderer>();

        materialList.Clear();
        materialList.AddRange(renderer.sharedMaterials);       
        materialList.Remove(outline);

        renderer.materials = materialList.ToArray();  
    }

}
