using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    [SerializeField] private List<Material> materialList = new List<Material>();
    [SerializeField] private Material outlineMaterial; 

    private MeshRenderer meshRenderer;                      // MeshRenderer 참조
    private int outlineIndex = -1;                           // 아웃라인 Material의 인덱스

    public bool isOutlineOn = false;                         // 아웃라인 활성화 여부

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // materialList 초기화
        if (meshRenderer != null && meshRenderer.material != null){
            materialList.Add(meshRenderer.material);
        }

        // outlineMaterial이 null이 아니면 materialList에 추가
        if (outlineMaterial != null){
            materialList.Add(outlineMaterial);
            outlineIndex = materialList.Count - 1;
        }

        // meshRenderer의 materials 속성에 materialList 할당
        if (materialList.Count > 0){
            meshRenderer.materials = materialList.ToArray();
        }

        // 아웃라인의 material의 _Outline_On 속성을 0으로 초기화
        meshRenderer.materials[outlineIndex].SetFloat("_Outline_On", 0f);
    }

    void Update(){
        meshRenderer.materials[outlineIndex].SetFloat("_Outline_On", isOutlineOn ? 1f : 0f);
    }

}
