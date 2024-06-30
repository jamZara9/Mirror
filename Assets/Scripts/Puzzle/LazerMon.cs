using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(LineRenderer))]
public class LazerMon : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask reflectLayerMask;
    public LayerMask defaultLayerMask;
    public LayerMask clearObj;
    public float defaultLength = 50;
    public float reflectNum = 2;

    private LineRenderer lineRenderer;
    private RaycastHit   hit;

    private Ray ray;
    private Vector3 _direction;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        ReflectLazer(); 
    }
    void ReflectLazer()
    {
        ray = new Ray(transform.position, transform.forward);

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);

        float resetLen = defaultLength;

        for (int i = 0; i < reflectNum; i++)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, resetLen, clearObj))
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount -1, hit.point);
                
                Debug.Log("Clear");
            }
            else if (Physics.Raycast(ray.origin, ray.direction, out hit, resetLen, reflectLayerMask))
            {
                Debug.Log("asdf");
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                // resetLen -= Vector3.Distance(ray.origin, hit.point);

                ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
            }
            else if (Physics.Raycast(ray.origin, ray.direction, out hit, resetLen, defaultLayerMask))
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount -1, hit.point);
            }
            else
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + (ray.direction * resetLen));
            }
        }
    }
}
