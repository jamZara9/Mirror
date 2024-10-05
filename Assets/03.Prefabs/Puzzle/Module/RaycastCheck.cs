using UnityEngine;

public class RaycastCheck : MonoBehaviour
{
    protected bool RayHitCheck(Vector3 mousePosition, Camera myCam, Transform target)
    {
        // Ray의 충돌 확인 용도
        RaycastHit raycastHit;
        
        // 카메라의 마우스 위치에서 Ray를 생성
        Ray myRay = myCam.ScreenPointToRay(mousePosition);
        
        // Ray가 물체와 충돌했을 시 true, 아니면 false
        bool weHitSomething = Physics.Raycast(myRay, out raycastHit);
        
        return weHitSomething && raycastHit.transform == target;
    }
}
