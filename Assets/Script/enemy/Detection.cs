using UnityEngine;

public class Detection : MonoBehaviour
{
    [SerializeField]Transform enemy;
    [SerializeField]float angleDetec;
    [SerializeField] Transform posInit;

    bool OnDetec;
    [SerializeField]float timeDetec;
    float timeDetecCount;
    [SerializeField]float raduis;
    [SerializeField]LayerMask layerMask;
    [SerializeField]LayerMask hitMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnDetec = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(OnDetec)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position,raduis,layerMask);

            int i = 0;

            while (i < colliders.Length)
            {
                if(colliders[i].transform.CompareTag("Enemy") || colliders[i].transform.CompareTag("Player"))
                {
                    Vector3 pos = colliders[i].transform.position - posInit.position;
                    if(Vector3.Angle(transform.forward,pos) <= angleDetec)
                    {
                        if(Physics.Raycast(posInit.position, pos,out RaycastHit hit,raduis,hitMask))
                        {
                            enemy = hit.transform == colliders[i].transform ? hit.transform : null;
                        }
                    }
                    else
                        i++;
                }
                else
                    i ++;
            }
            timeDetecCount = timeDetec;
            OnDetec = false;
        }
        else
        {
            timeDetecCount -= Time.deltaTime;

            OnDetec = timeDetecCount <= 0.0f ? true : false;
        }
    }  

    #if UNITY_EDITOR
        private void OnDrawGizmos() { 
            Gizmos.DrawWireSphere(posInit.position,raduis);
        }
    #endif
}
