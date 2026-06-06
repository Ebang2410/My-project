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
     GunScript gunScript;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public GameObject Bull;

    bool offMove;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offMove = false;
        transform.GetComponent<CapsuleCollider>().enabled = true;
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

        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.GetComponent<NetworkObject>().Spawn();
            muzzleVFX.transform.forward = gameObject.transform.forward;
            var psMuzzle = muzzleVFX.GetComponent<ParticleSystem>();
            if (psMuzzle != null)
            {
                muzzleVFX.GetComponent< DestroyNetwork>().Destroy(psMuzzle.main.duration);
                Debug.Log("destroy Muzzle");
            }
        }
        
    }

    // Update is called once per frame
   

    private void OnCollisionEnter(Collision other) {

        if(IsServer)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            
            EnabledComponentServerRpc();
            if(other.transform.CompareTag("Enemy"))
            {
                bool death = other.transform.GetComponent<DamageScript>().MinusPm(pd);
                if(death)
                {
                    gunScript.AddKill();
                }
            }

            GetComponent<DestroyNetwork>().Destroy(2.0f);

            ContactPoint contact = other.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            if(hitPrefab != null)
            {
                var hitVFX = Instantiate(hitPrefab, pos, rot);
                var psHit = hitVFX.GetComponent<ParticleSystem>();
                if (psHit != null) 
                {
                    hitVFX.GetComponent<DestroyNetwork>().Destroy(psHit.main.duration);
                    //Destroy(hitVFX, psHit.main.duration);
                }

                hitVFX.GetComponent<NetworkObject>().Spawn();
            }
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
        if(Bull != null) Bull.SetActive(false);
    }

    public void SetInfo(GunScript gun,float degat, float speed, float distance)
    {
        this.gunScript = gun;
        this.pd = degat;
        this.speed = speed;
        this.distance = distance;
    }
}
