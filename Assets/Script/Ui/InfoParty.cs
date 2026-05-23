using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class InfoParty : NetworkBehaviour
{
    public static InfoParty infoParty;

    [SerializeField] Transform content;
    [SerializeField] GameObject infoItem;
    [SerializeField] GameObject infoKill;

    [SerializeField] Dictionary<ulong,InfoPlayer> infoPlayers = new Dictionary<ulong, InfoPlayer>();
    [SerializeField] Dictionary<ulong,GameObject> infoGameObjects = new Dictionary<ulong, GameObject>();

    void Awake()
    {
        if(infoParty == null)
            infoParty = this;
    }

    public void CreatedInfo(ulong id,string name = "Player")
    {
        if (IsServer)
        {
            if (name == "Player")
            {
                name = "Player_"+id;
            }

            foreach (var item in infoPlayers)
            {
                CreatedInfoClientRpc(item.Value.getIdPlayer(),item.Value.getName());
            }

            CreatedInfoClientRpc(id,name);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void CreatedInfoClientRpc(ulong id,string name)
    {
        //Verification du player
        int i = 0;
        foreach (var item in infoPlayers)
        {
            if (item.Value.getIdPlayer() != id)
            {
                i++;
            }
        }

        if(infoPlayers.Count == i)
        {
            InfoPlayer infoPlayer = new InfoPlayer(id,name);

            infoPlayers.Add(id,infoPlayer);
            CreatedInfoGameObject(infoPlayer);
        }
    }

    //Creer un le gameObject du player chez chaque client du serveur
    void CreatedInfoGameObject(InfoPlayer infoPlayer)
    {
        GameObject infoGameObject = Instantiate(infoItem,content);
        infoGameObjects.Add(infoPlayer.getIdPlayer(),infoGameObject);

        TMP_Text mP_Text = infoGameObject.transform.Find("Name").gameObject.GetComponent<TMP_Text>();
        mP_Text.text = infoPlayer.getName();

        TMP_Text mP_Tex = infoGameObject.transform.Find("Kill").gameObject.GetComponent<TMP_Text>();
        mP_Tex.text = ""+ 0;
    }

    [Rpc(SendTo.Server)]
    public void AddKillServerRpc(ulong idPlayer)
    {
        AddKillClientRpc(idPlayer);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void AddKillClientRpc(ulong idPlayer)
    {
        if(infoPlayers.ContainsKey(idPlayer))
        {
            int kill = infoPlayers[idPlayer].numberKill + 1;
            infoPlayers[idPlayer].numberKill = kill;
            
            if (infoGameObjects.ContainsKey(idPlayer))
            {
                TMP_Text mP_Text = infoGameObjects[idPlayer].transform.Find("Name").gameObject.GetComponent<TMP_Text>();
                mP_Text.text = infoPlayers[idPlayer].getName();

                TMP_Text mP_Tex = infoGameObjects[idPlayer].transform.Find("Kill").gameObject.GetComponent<TMP_Text>();
                mP_Tex.text = ""+kill;

                int position = (int)infoPlayers[idPlayer].position;

                if( position != 0)
                {
                    position = position- 1;
                    Transform Grandchild = content.transform.GetChild(position);

                    string killText = infoGameObjects[idPlayer].transform.Find("Kill").gameObject.GetComponent<TMP_Text>().text;

                    if (int.Parse(killText ) <= kill)
                    {
                        infoGameObjects[idPlayer].transform.SetSiblingIndex(position);
                    }
                }
                
            }

        }
    }
}

[System.Serializable]
public class InfoPlayer {
    private ulong idPlayer;
    private string name;
    public int numberKill;

    public ulong position;

    public InfoPlayer(ulong id,string name)
    {
        this.idPlayer = id;
        this.name = name;
        numberKill = 0;
        position = id;
    }

    public ulong getIdPlayer()
    {
        return this.idPlayer;
    }

    public string getName()
    {
        return this.name;
    }
}
