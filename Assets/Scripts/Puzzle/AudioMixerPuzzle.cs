using UnityEngine;

public class AudioMixerPuzzle : MonoBehaviour
{
    public float maxMove;
    public float buttonMoveSpeed = 1;
    public GameObject[] button;
    public bool drag;

    private int nowDragButton;
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            RaycastHit hit = CastRay();

            for (var i = 0; i < button.Length; i++)
            {
                if (hit.transform != button[i].transform) continue;
                
                nowDragButton = i;
                drag = true;
            }
            
        }
        if(Input.GetMouseButtonUp(0)){
            drag = false;
        }
        
        var buttonPos = button[nowDragButton].transform.localPosition;
        
        if(drag)
        {
            Vector3 position = new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
            
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

            if(buttonPos.z >= -maxMove && buttonPos.z <= maxMove)
            {
                button[nowDragButton].transform.localPosition = new Vector3(buttonPos.x, 0, worldPosition.z * buttonMoveSpeed);
            }
        }
        
        if(buttonPos.z > maxMove)
        {
            button[nowDragButton].transform.localPosition = new Vector3(buttonPos.x, 0, maxMove);
            drag = false;
        }
        else if(buttonPos.z < -maxMove)
        {
            button[nowDragButton].transform.localPosition = new Vector3(buttonPos.x, 0, -maxMove);
            drag = false;
        }

        CheakClear();
    }

    private RaycastHit CastRay(){
        Vector3 screenMousePosFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);

        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);

        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar-worldMousePosNear, out hit);

        return hit;
    }

    private void CheakClear()
    {
        var cheak = 0;
        for (int i = 0; i < button.Length; i++)
        {
            if (button[i].transform.localPosition.z >= maxMove)
            {
                cheak++;
            }
            else
            {
                break;
            }
        }

        if (cheak == button.Length)
        {
            Debug.Log("Clear!");
        }
    }
}
