using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class SettingHostNetwork : NetworkBehaviour
{
    [SerializeField] GameObject panelClient;
    [SerializeField] GameObject panelServer;
    [SerializeField] TMP_Text statut;
    [SerializeField] GameObject itemPlayer;
    [SerializeField] Transform content;
    [SerializeField] GameObject ButtonServer;
    [SerializeField] GameObject startGame;
    [SerializeField] GameObject CanvasLobby;

    [SerializeField] GameObject ButtonClient;

    public int countPlayer;
    public int countPlay;

    Dictionary<ulong,InfoPlayer> infoPlayers = new Dictionary<ulong, InfoPlayer>();
    Dictionary<ulong,GameObject> gameObjects = new Dictionary<ulong, GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            countPlay = 0;
            countPlayer = 0;
        }

        if ( NetworkManager.Singleton.IsHost)
        {
            startGame.SetActive(true);
        }
        else
        {
            startGame.SetActive(false);
        }
    }



    public void Info( bool client)
    {
        if(!client)
        {
            ButtonServer.SetActive(true);
            ButtonClient.SetActive(false);
        }
        else
        {
            ButtonServer.SetActive(false);
            ButtonClient.SetActive(true);
        }

        string  name = PlayerPrefs.GetString("PlayerName");
        if(string.IsNullOrEmpty(name))
        {
            name = "Player_";
        }

        SpawnItemPlayerServerRpc(name);
        Debug.Log("Spawn");
    }

    // Update is called once per frame

    public void Play()
    {
        if(!IsServer)
            return;
        else
        {
            LoadScene.loadScene.ChangeScene(1);
            OffCanvasLobbyClientRpc();
        }
    }

    [ClientRpc]
    void OffCanvasLobbyClientRpc()
    {
        CanvasLobby.SetActive(false);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SpawnItemPlayerServerRpc(string name, ServerRpcParams rpcParams= default)
    {
        ulong id = rpcParams.Receive.SenderClientId;

        Debug.Log(id);

        if(infoPlayers.Count > 0)
        {
            foreach (var item in infoPlayers)
            {
                SpawnItemPlayerClientRpc(item.Value.name,item.Value.id,false);
            }
        }
        name = name + id;
        SpawnItemPlayerClientRpc(name ,id,false);
    }

    [ClientRpc]
    public void SpawnItemPlayerClientRpc(string name, ulong id, bool play)
    {
        //verification si ces donnees existe ? 
        if(!infoPlayers.ContainsKey(id))
        {   
            GameObject obj = Instantiate(itemPlayer,content);
            obj.transform.GetComponent<PlayerItemNetwork>().id = id;
            obj.transform.GetComponent<PlayerItemNetwork>().play = play;
            obj.transform.GetComponent<PlayerItemNetwork>().name = name;  
            gameObjects.Add(id,obj);

            InfoPlayer infoPlayer = new InfoPlayer(id,play,name);
            infoPlayers.Add(id,infoPlayer);

            countPlayer++;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CloseHostServerRpc()
    {
        CloseHostClientRpc();
    }

    [ClientRpc]
    void CloseHostClientRpc()
    {
        infoPlayers.Clear();
        gameObjects.Clear();
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }

        panelClient.SetActive(true);
        //panelAddHost.SetActive(false);
        panelServer.SetActive(false);

        statut.text = "Recherche des serveurs...";

        StartCoroutine(OffServerNetwork());
    }

    public void CloseHostOne()
    {
        RemovePlayerServerRpc();

        infoPlayers.Clear();
        gameObjects.Clear();
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }

        statut.text = "Recherche des serveurs...";

        StartCoroutine(OffServerNetwork());
    }

    [ServerRpc(RequireOwnership = false)]
    void RemovePlayerServerRpc(ServerRpcParams rpcParams= default)
    {
        ulong id = rpcParams.Receive.SenderClientId;
        RemovePlayerClientRpc(id);
    }

    [ClientRpc]
    void RemovePlayerClientRpc(ulong id)
    {
        if(infoPlayers.ContainsKey(id))
            infoPlayers.Remove(id);

        if(gameObjects.ContainsKey(id))
        {
            Destroy(gameObjects[id]);
            
            gameObjects.Remove(id);
        }
    }

    IEnumerator OffServerNetwork()
    {
        yield return new WaitForSeconds(2f);
        NetworkManager.Singleton.Shutdown();
    }
}

[System.Serializable]
public class InfoPlayer
{
    public string name;

    public bool play;

    public ulong id;

    public InfoPlayer(ulong id,bool play, string name)
    {
        this.id = id;
        this.play = play;
        this.name = name;
    }
}
