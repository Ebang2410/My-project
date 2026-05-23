using UnityEngine;
using Unity.Netcode;

public class BallScript : NetworkBehaviour
{
    [SerializeField]
    float pd;
    [SerializeField]
    float speed;
    [SerializeField]
    float distance;
    [SerializeField]
    Vector3 initPosition;
    [SerializeField]
     GunScript gunScript;

    bool offMove;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offMove = false;
        transform.GetComponent<CapsuleCollider>().enabled = true;
        transform.GetComponent<MeshRenderer>().enabled = true;
        Vector3 initPosition = transform.position; 

        if(IsServer)
        {
             GetComponent<Rigidbody>().isKinematic =false;
             GetComponent<Rigidbody>().AddForce(transform.forward * speed * 100);
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        
    }

    // Update is called once per frame
   

    private void OnTriggerEnter(Collider other) {
        if(IsServer)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            
            EnabledComponentServerRpc();
            if(other.CompareTag("Enemy"))
            {
                bool death = other.transform.GetComponent<DamageScript>().MinusPm(pd);
                if(death)
                {
                    gunScript.AddKill();
                }
            }

            GetComponent<DestroyNetwork>().Destroy(2.0f);
        }
    }

    [Rpc(SendTo.Server)]
    void EnabledComponentServerRpc()
    {
        offMove = true;
        EnabledComponentClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void EnabledComponentClientRpc()
    {
        transform.GetComponent<CapsuleCollider>().enabled = false;
        transform.GetComponent<MeshRenderer>().enabled = false;
    }

    public void SetInfo(GunScript gun,float degat, float speed, float distance)
    {
        this.gunScript = gun;
        this.pd = degat;
        this.speed = speed;
        this.distance = distance;
    }
}
