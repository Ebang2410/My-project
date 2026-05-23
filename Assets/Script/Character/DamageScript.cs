using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System;

public class DamageScript : NetworkBehaviour
{
    [SerializeField]
    float pm;

    NetworkVariable<float> pmNetwork = new NetworkVariable<float>(0);

    [SerializeField]
    BareScript barScript;

    bool death;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(pm != 0 && IsServer)
            pmNetwork.Value = pm;
        death = false;
        barScript.InitValue(pm);

        pmNetwork.OnValueChanged +=  OnHealthChanged;

    }

    private void OnHealthChanged(float previousValue, float newValue)
    {
        barScript.Val = newValue;
    }

    public bool MinusPm(float point)
    {
        PmServerRpc(-point);

        return VerificationPm();
    }

    public void PlusPm(float point)
    {
        if(!VerificationPm())
            PmServerRpc(point);
    }

    [Rpc(SendTo.Server)]
    void PmServerRpc(float point)
    {
        pmNetwork.Value += point;
    }

    public bool VerificationPm()
    {
        if(IsServer && barScript.Val <= 0 && !death)
        {
            DeathServerRpc();
            death = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    [Rpc(SendTo.Server)]
    void DeathServerRpc()
    {
        GetComponent<Animator>().SetTrigger("death");
    }

    public void Death()
    {
        GetComponent<DestroyNetwork>().Destroy(0.5f);
    }
}
