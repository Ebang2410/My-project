using UnityEngine;
using Unity.Netcode;

public class KnifeScript : NetworkBehaviour
{
    [SerializeField]
    PlayerScript playerScript;
    [SerializeField]
    EnemyScript enemyScript;
    [SerializeField]
    Arme arme;
    BoxCollider boxCollider;
    private void OnEnable() {
        if (playerScript == null && enemyScript == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        if (arme != null && playerScript != null)
        {
            playerScript.arme = arme;
            playerScript.knifeScript = this;
            playerScript.typeArme = arme.typeArme;
        }
        else if (arme != null && enemyScript != null)
        {
            enemyScript.arme = arme;
            enemyScript.knifeScript = this;
            enemyScript.typeArme = arme.typeArme;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryGetComponent(out boxCollider);
        OffAndOnBoxCollider(false);
    }

    private void OnTriggerEnter(Collider other) {
        if(IsServer)
        {          
            if(other.CompareTag("Enemy"))
            {
                bool death = other.transform.GetComponent<DamageScript>().MinusPm(arme.degat);
                if(death)
                {
                    AddKill();
                }
            }
        }
    }

    public void AddKill()
    {
        if(playerScript != null && IsServer)
            InfoParty.infoParty.AddKillServerRpc(playerScript.GetComponent<NetworkObject>().OwnerClientId);
    }

    public void OffAndOnBoxCollider(bool collide)
    {
        if(collide)
        {
            boxCollider.enabled = true;
        }
        else
        {
            boxCollider.enabled = false;
        }
    }

}
