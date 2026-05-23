using UnityEngine;

public class CustomLine : MonoBehaviour
{
    [SerializeField]
    Transform player;
    [SerializeField]
    Transform posInit;
    public float distance;
    [SerializeField]
    LineRenderer lineRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posInit = this.transform;

        TryGetComponent(out lineRenderer);
       lineRenderer.positionCount = 2; 
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = posInit.position;

        Vector3 end = pos + transform.forward * distance;
        lineRenderer.SetPosition(0, pos);
        lineRenderer.SetPosition(1,end);
    }
}
