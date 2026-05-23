using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class SpawnCharacter : NetworkBehaviour
{
    public static SpawnCharacter spawnCharacter;
    [SerializeField] GameObject gameObjectPlayer;
    [SerializeField] List<GameObject> gameObjectEnemys;
    [SerializeField] List<Transform> positionSpawnPlayers;
    [SerializeField] List<Transform> positionSpawnEnemys;
    [SerializeField] DataCustom dataCustom;

    public int countPlayer;
    bool firstPawnPlayer;

    public int countEnemy;
    int idPlayer;
    [SerializeField] float timeSpawnEnemy;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        idPlayer = 1;
        firstPawnPlayer = false;
        if(spawnCharacter == null)
            spawnCharacter = this;
        else
            Destroy(this.gameObject);
        
        if(gameObjectPlayer == null || gameObjectEnemys == null || positionSpawnEnemys == null || positionSpawnPlayers == null)
            this.enabled = false;

        if(IsServer)
        {
            
        }

        DataCustom data = SaveScript.saveScript.LoadFromJson();
        if(data != null)
        {
            dataCustom = data;
        }

        if(NetworkManager.Singleton.IsClient)
        {
            StartCoroutine(SpawnPlayer(dataCustom,.5f));
        }
    }

    public void ReSpawn()
    {
        StartCoroutine(SpawnPlayer(dataCustom,6f));
    }

    IEnumerator SpawnPlayer(DataCustom dataCustom,float timeSpawn)
    {
        yield return new WaitForSeconds(timeSpawn);

        CustomPlayer Customplayer;
        Customplayer.idHead = dataCustom.idHead;
        Customplayer.idBody = dataCustom.idBody;
        Customplayer.idLeg = dataCustom.idLeg;
        Customplayer.idAcce = dataCustom.idAcce;
        Customplayer.idEqui = dataCustom.idEqui;
        Customplayer.idWeapon = dataCustom.idWeapon;

        
        SpawnPlayerServerRpc(Customplayer);
    }

    [Rpc(SendTo.Server)]
    void SpawnPlayerServerRpc(CustomPlayer Customplayer,string name = "Player",RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        int id = 0;
        int countposePlayer = positionSpawnPlayers.Count;
       
        id = Random.Range(0,countposePlayer);

        GameObject obj =  Instantiate(gameObjectPlayer,positionSpawnPlayers[id].position,positionSpawnPlayers[id].rotation);
        PlayerCustom playerCustom = obj.transform.GetComponent<PlayerCustom>();

        if(playerCustom != null)
        {
            playerCustom.customplayer.Value = Customplayer;
        }

        NetworkObject networkObject = obj.transform.GetComponent<NetworkObject>();
        if(networkObject != null)
        {
            networkObject.SpawnAsPlayerObject(clientId);
            InfoParty.infoParty.CreatedInfo(clientId,name);
        }
        else
        {
            Destroy(obj);
        }
        
    }

    [Rpc(SendTo.Server)]
    void SpawnEnemyServerRpc()
    {
        
    }
}

public struct CustomPlayer : INetworkSerializable
{
    public int idHead;
    public int idBody;
    public int idLeg;
    public int idAcce;
    public int idEqui;
    public int idWeapon;

    public void NetworkSerialize<T> (BufferSerializer<T> serializer) where T :
    IReaderWriter
    {
        serializer.SerializeValue(ref idHead);
        serializer.SerializeValue(ref idBody);
        serializer.SerializeValue(ref idLeg);
        serializer.SerializeValue(ref idAcce);
        serializer.SerializeValue(ref idEqui);
        serializer.SerializeValue(ref idWeapon);
    }
}
