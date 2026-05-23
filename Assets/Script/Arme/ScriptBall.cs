using UnityEngine;

public class ScriptBall : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    public float maxDistance;
    Vector3 initpos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        initpos = transform.position;
        Debug.Log("Debut");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if(Vector3.Distance(initpos,transform.position) >= maxDistance)
        {
            Debug.Log("Detruite");
            Destroy(this.gameObject);
        }
    }
}
