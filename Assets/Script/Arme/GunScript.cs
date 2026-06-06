// FIX: suppression de "using System.Drawing" — namespace non-Unity, cause des conflits
using UnityEngine;
using Unity.Netcode;

public class GunScript : MonoBehaviour
{
    [SerializeField] AudioClip snGun;
    AudioSource audioSource;
    [SerializeField] PlayerScript playerScript;
    [SerializeField] EnemyScript enemyScript;
    [SerializeField] Arme arme;

    // FIX: renommé de "gameObject" (conflit avec membre hérité MonoBehaviour) en "bulletPrefab"
    [SerializeField]
    GameObject bulletPrefab;

    public Transform spawnPoint;
    [SerializeField]
    GestionPointSpawn gestionPointSpawn;
    bool isfirt = true;

    private void OnEnable() {
         // FIX: condition corrigée — désactiver si AUCUN des deux n'est assigné
        if (playerScript == null && enemyScript == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        if (arme != null && playerScript != null)
        {
            playerScript.arme = arme;
            playerScript.gunScript = this;
            playerScript.typeArme = arme.typeArme;
            if(!isfirt && playerScript.IsMe())
                playerScript.AnimeModeRpc(arme.typeArme);
            else
                isfirt = false;
        }
        else if (arme != null && enemyScript != null)
        {
            enemyScript.arme = arme;
            enemyScript.gunScript = this;
            enemyScript.typeArme = arme.typeArme;
            gestionPointSpawn = enemyScript.transform.GetComponent<GestionPointSpawn>();
        }

        if (gestionPointSpawn != null)
        {
            gestionPointSpawn.SpawnPointActive(spawnPoint);
        }

        
    }

    private void Awake() {
        TryGetComponent(out audioSource);

        if (playerScript != null)
        {
            playerScript.GetComponent<AudioManagerCharacter>().gunScript = this;
        }
    }


    public void SpawnBall()
    {
        //  utilisation de bulletPrefab (anciennement "gameObject", membre hérité écrasé)
        if (bulletPrefab == null || spawnPoint == null) return;
        GameObject bal = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        bal.transform.GetComponent<BallScript>().SetInfo(this,arme.degat, arme.speedBall, arme.distanceVie);
        bal.transform.SetParent(null);
        bal.GetComponent<NetworkObject>().Spawn();
    }

    public void SnGunPlay()
    {
        if(audioSource != null)
            audioSource.PlayOneShot(snGun);
        Debug.Log("Sfx gun play");
    }

    public void InitSnGun()
    {
        if(audioSource != null)
            audioSource.PlayOneShot(snGun,0.1f);
    }

    public void AddKill()
    {
        if(playerScript != null )
            InfoParty.infoParty.AddKillServerRpc(playerScript.GetComponent<NetworkObject>().OwnerClientId);
    }

    private void OnDisable() {
         if (gestionPointSpawn != null)
        {
            gestionPointSpawn.SpawnPointOff(spawnPoint);
        }
    }
}
