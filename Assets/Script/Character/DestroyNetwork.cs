using UnityEngine;
using Unity.Netcode;
using System.Collections;
public class DestroyNetwork : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void Destroy(float time)
    {
        StartCoroutine(DestroyTime(time));
    }

    [Rpc(SendTo.Server)]
    void DestroyGameObjectServerRpc()
    {
        transform.GetComponent<NetworkObject>().Despawn();
    }

    IEnumerator DestroyTime(float time)
    {
        yield return new WaitForSeconds(time);

        DestroyGameObjectServerRpc();
    }
}
