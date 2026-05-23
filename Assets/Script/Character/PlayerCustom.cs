using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Security.AccessControl;

public class PlayerCustom : NetworkBehaviour
{
    [SerializeField]
    List<GameObject> heads;
    [SerializeField]
    List<GameObject> legs;
    [SerializeField]
    List<GameObject> equis;
    [SerializeField]
    List<GameObject> acces;
    [SerializeField]
    List<GameObject> bodys;
    [SerializeField]
    List<GameObject> weapons;

    [SerializeField]
    GameObject knife;


    public DataCustom dataCustom;

    public NetworkVariable<CustomPlayer> customplayer = new NetworkVariable<CustomPlayer>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        if(heads.Count > 0 && legs.Count > 0 && equis.Count > 0 && acces.Count > 0 && bodys.Count > 0 && weapons.Count > 0)
        {
            dataCustom = new DataCustom(0, 
            customplayer.Value.idHead,
            customplayer.Value.idBody,
            customplayer.Value.idLeg, 
            customplayer.Value.idAcce,
            customplayer.Value.idEqui,
            customplayer.Value.idWeapon);
            DesactiveAll();
            knife.SetActive(false);
            ActiveSkin();
        }
        else
            this.enabled = false;
    }

    void DesactiveAll()
    {
        // head
        foreach (GameObject item in heads)
        {
            if(item != null)
                item.SetActive(false);
        }

        // body
        foreach (GameObject item in bodys)
        {
            if(item != null)
                item.SetActive(false);
        }

        // leg
        foreach (GameObject item in legs)
        {
            if(item != null)
                item.SetActive(false);
        }

        // acce
        foreach (GameObject item in acces)
        {
            if(item != null)
                item.SetActive(false);
        }

        // equi
        foreach (GameObject item in equis)
        {
            if(item != null)
                item.SetActive(false);
        }

        // weapon
        foreach (GameObject item in weapons)
        {
            if(item != null)
                item.SetActive(false);
        }
    }

    void ActiveSkin()
    {
        if(dataCustom.idHead < heads.Count  && heads[dataCustom.idHead] != null)
            heads[dataCustom.idHead].SetActive(true);   

        if(dataCustom.idBody < bodys.Count  && bodys[dataCustom.idBody] != null)
            bodys[dataCustom.idBody].SetActive(true); 

        if(dataCustom.idLeg < legs.Count  && legs[dataCustom.idLeg] != null)  
            legs[dataCustom.idLeg].SetActive(true);  

        if(dataCustom.idAcce < acces.Count  && acces[dataCustom.idAcce] != null)
            acces[dataCustom.idAcce].SetActive(true);  

        if(dataCustom.idEqui < equis.Count  && equis[dataCustom.idEqui] != null)
            equis[dataCustom.idEqui].SetActive(true); 
              
        if(dataCustom.idWeapon < weapons.Count  && weapons[dataCustom.idWeapon] != null)
            weapons[dataCustom.idWeapon].SetActive(true);   
    }

    public void ChangeWeapon(bool changeForKnife)
    {
        if(changeForKnife)
        {
            if(dataCustom.idWeapon < weapons.Count  && weapons[dataCustom.idWeapon] != null)
                weapons[dataCustom.idWeapon].SetActive(false);
            if(knife != null)
            {
                knife.SetActive(true);
            }
        }
        else
        {
           if(dataCustom.idWeapon < weapons.Count  && weapons[dataCustom.idWeapon] != null)
                weapons[dataCustom.idWeapon].SetActive(true);
            if(knife != null)
                knife.SetActive(false); 
        }
    }
}
