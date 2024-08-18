using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아웃라인 클래스
/// 
/// <para>
/// author  : Argonaut
/// </para>
/// </summary>
public class Outline : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> meshRenderers = new();            // Material 리스트
    [SerializeField] private Material outlineMaterial; 

    void Start()
    {
        // 자식 오브젝가 존재한다면 자식 오브젝트의 MeshRenderer/Material을 가져와서 materialList에 추가
        if (transform.childCount > 0){
            foreach (Transform child in transform){
                if (child.TryGetComponent<MeshRenderer>(out var childMeshRenderer)){
                    // 자식의 MeshRender에 outlineMaterial을 추가
                    List<Material> materials = new List<Material>(childMeshRenderer.materials)
                    {
                        outlineMaterial
                    };
                    
                    childMeshRenderer.materials = materials.ToArray();

                    // 자식의 MeshRender를 meshRenderers에 추가
                    meshRenderers.Add(childMeshRenderer);

                    // 초기에는 아웃라인을 끄고 시작
                    childMeshRenderer.materials[materials.Count - 1].SetFloat("_Outline_On", 0f);
                }
            }
        }else{  // 자식 오브젝트가 없다면 자신의 MeshRenderer/Material을 가져와서 materialList에 추가
            if (TryGetComponent<MeshRenderer>(out var meshRenderer)){
                List<Material> materials = new List<Material>(meshRenderer.materials)
                {
                    outlineMaterial
                };
                
                meshRenderer.materials = materials.ToArray();
                meshRenderers.Add(meshRenderer);
                meshRenderer.materials[materials.Count - 1].SetFloat("_Outline_On", 0f);
            }
        }
    }    

    /// <summary>
    /// 아웃라인 on/off
    /// </summary>
    /// <param name="state">on/off 기능 추가</param>
    public void SetOutline(bool state){
        foreach (var meshRenderer in meshRenderers){
            // meshRenderer.materials[meshRenderer.materials.Length - 1].SetFloat("_Outline_On", state ? 1f : 0f);
            meshRenderer.materials[^1].SetFloat("_Outline_On", state ? 1f : 0f);
        }
    }

}
