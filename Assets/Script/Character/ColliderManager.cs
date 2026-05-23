using UnityEngine;
using Unity.Netcode;

public class ColliderManager : NetworkBehaviour
{
    [Header("collider Position")]
    [SerializeField]
    CapsuleCollider colliderSquat;
    [SerializeField]
    CapsuleCollider colliderCrouch;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(colliderCrouch != null && colliderSquat != null)
        {
            colliderSquat.enabled = true;
            colliderCrouch.enabled = false;
        }
    }

    public void CrouchOn()
    {
        if(colliderCrouch != null && colliderSquat != null)
        {
            colliderSquat.enabled = false;
            colliderCrouch.enabled = true;
        }
    }

    public void CrouchOff()
    {
        if(colliderCrouch != null && colliderSquat != null)
        {
            colliderSquat.enabled = true;
            colliderCrouch.enabled = false;
        }
    }

    [Rpc(SendTo.Server)]
    public void CrouchServerRpc(bool crouch)
    {
        CrouchClientRpc(crouch);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void CrouchClientRpc(bool crouch)
    {
        if(crouch)
            CrouchOn();
        else
            CrouchOff();
    }

    public void OffCollider()
    {
        colliderSquat.enabled = false;
        colliderCrouch.enabled = false;
    }


}
