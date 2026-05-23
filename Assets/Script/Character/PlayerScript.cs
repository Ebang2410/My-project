using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;


public class PlayerScript : NetworkBehaviour
{
    [Header(" Info Move")]
    public Arme arme;
    NetworkVariable <bool> isCrouch = new NetworkVariable<bool>(false);
    public bool isLooking;
    float turn = 0.14f;
    float smooth;
    public TypeArme typeArme;
    [SerializeField]
    Joystick joystickMove;
    [SerializeField]
    Joystick joystickLook;
    [SerializeField]
    ButtonAction buttonAction;
    Transform cam;
    float angle;
    float timeLook = 5.0f;
    float _timeLook;
    float _timeAttack;
    Rigidbody rb;
    Animator animator;
    AudioSource audioSource;
    InputStarted inputStarted;
    ColliderManager colliderManager;
    GestionPointSpawn gestionPointSpawn;
    Vector3 move3D = Vector3.zero;
    Cinemachine cinemachine;

    public bool OnDegat;
    public GunScript gunScript;
    public KnifeScript knifeScript;
    bool waeponKnife;
    bool isDestroy;
    [SerializeField] bool KnifeOnDegat;

    private void Awake() {
        TryGetComponent(out gestionPointSpawn);
    }
    void Start()
    {
        KnifeOnDegat = false;
        isDestroy = false;
        waeponKnife = false;

        _timeAttack = 0;
        OnDegat = false;

        TryGetComponent(out rb);
        TryGetComponent(out colliderManager);
        TryGetComponent(out audioSource);
        TryGetComponent(out cinemachine);

        _timeLook = 0;

        if (IsOwner)
        {
            TryGetComponent(out inputStarted);
            cam = GameObject.FindWithTag("MainCamera").transform;

            joystickMove = InputManager.input.joystickMove;
            joystickLook = InputManager.input.joystickLook;
            buttonAction = InputManager.input.joystickShoot;

            cinemachine.enabled = true;

            GetComponent<PlayerInput>().enabled = true;
            
        }
        else
        {
            cinemachine.enabled = false;
            GetComponent<PlayerInput>().enabled = false;
        }

        if (!IsServer)
        {
            rb.isKinematic = true;
            
        }
        else
            rb.isKinematic = false;

        TryGetComponent(out animator);

        if(gestionPointSpawn != null)
            isCrouch.OnValueChanged += CrouchChange;
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            if (inputStarted.crouch)
            {
                CrouchServerRpc();
                if (colliderManager != null)
                    colliderManager.CrouchServerRpc(isCrouch.Value);
                inputStarted.crouch = false;
            }

            if(inputStarted.interaction)
            {
                ChangeWeaponServerRpc();
                inputStarted.interaction = false;
            }

            float xAnimation = 0;
            float yAnimation = 0;
            float speedMove = 0;
            float ActArme = 1;

            float speed = 0;
            AnimeModeServerRpc(typeArme);

            if ((inputStarted.fire || buttonAction.fire) && _timeAttack <= 0.0f)
            {
                _timeAttack = arme.timeNextBall;
                _timeLook = timeLook;

                AttackServerRpc();
                gunScript.SnGunPlayClient();
                ShootServerRpc();
                
            }
            else
            {
                _timeAttack -= Time.deltaTime;
            }

            if (_timeLook > 0.0f)
            {
                isLooking = true;
                _timeLook -= Time.fixedDeltaTime;
            }
            else
            {
                isLooking = false;
            }

            if (inputStarted.move != Vector2.zero || joystickMove.Direction != Vector2.zero)
            {
                ActArme = 1;
                Vector2 move2d = joystickMove.Direction != Vector2.zero ? joystickMove.Direction : inputStarted.move;

                move3D = new Vector3(move2d.x, 0, move2d.y);
                angle = cam.eulerAngles.y;
                
                yAnimation = move2d.y;
                if(move2d.y < 0.0f)
                {
                    xAnimation = -move2d.x ;
                }
                else
                {
                    xAnimation = move2d.x;
                }
                
                if (inputStarted.run == true && !isCrouch.Value && !inputStarted.fire)
                {
                    speedMove = 1;
                    if (!isLooking || move3D.z >= 0)
                        speed = arme.run + 2f;
                    else if (move3D.z <= 0)
                        speed = arme.run - 2f;
                }
                else
                {
                    speedMove = 0.5f;

                    if (!isLooking || move3D.z >= 0)
                        speed = arme.walk + 1;
                    else if (move3D.z < 0)
                        speed = arme.walk - 1f;
                }
            }
            else
            {
                move3D = Vector3.zero;
                angle = cam.eulerAngles.y;
            }

            AnimeMoveServerRpc(ActArme, isCrouch.Value, speedMove, xAnimation, yAnimation);
            MoveServerRpc(move3D, angle, speed);
        }
    }

    [Rpc(SendTo.Server)]
    void CrouchServerRpc()
    {
        isCrouch.Value = !isCrouch.Value ;
    }

    void CrouchChange(bool previousValue,bool newValue)
    {
        gestionPointSpawn.CrouchChange(newValue);
    }

    [Rpc(SendTo.Server)]
    void AttackServerRpc()
    {
        animator.SetTrigger("attack");
    }

    [Rpc(SendTo.Server)]
    public void ResetShootTriggerServerRpc()
    {
        animator.ResetTrigger("attack");
    }

    [Rpc(SendTo.Server)]
    void AnimeMoveServerRpc(float ActArme, bool crouch, float speedMove, float x, float y)
    {
        animator.SetFloat("positionX", x);
        animator.SetFloat("positionY", y);
        animator.SetFloat("speed", speedMove);
        animator.SetFloat("arme", ActArme);
        animator.SetBool("crouch", crouch);
    }

    [Rpc(SendTo.Server)]
    public void MoveServerRpc(Vector3 move3DNetwork, float angleNetwork, float speedNetwork)
    {
        if (angleNetwork != 0.0f)
        {
            //float rotate = Mathf.SmoothDampAngle(transform.eulerAngles.y, angleNetwork, ref smooth, turn);
            transform.rotation = Quaternion.Euler(0, angleNetwork, 0);
        }

        if (move3DNetwork.sqrMagnitude >= 0.01f)
            /* rb.MovePosition(transform.position + move3DNetwork * speedNetwork * Time.fixedDeltaTime * -1); */
            transform.Translate(move3DNetwork.normalized * speedNetwork * Time.fixedDeltaTime);
    }

    public void Degat()
    {
        if (IsOwner)
        {
            OnDegat = !OnDegat;
        }
    }

    [Rpc(SendTo.Server)]
    void AnimeModeServerRpc(TypeArme armeAnimation)
    {
        switch (armeAnimation)
        {
            case TypeArme.Hangun:
                animator.SetFloat("type", 0f);
                animator.SetInteger("gun", 0);
                break;
            case TypeArme.Heavy:
                animator.SetFloat("type", 0.25f);
                animator.SetInteger("gun", 1);
                break;
            case TypeArme.Infantry:
                animator.SetFloat("type", 0.5f);
                animator.SetInteger("gun", 2);
                break;
            case TypeArme.Knife:
                animator.SetFloat("type", 0.75f);
                animator.SetInteger("gun", 3);
                break;
            case TypeArme.RocketLauncher:
                animator.SetFloat("type", 1f);
                animator.SetInteger("gun", 4);
                break;
        }
    }

    public bool IsMe()
    {
        return IsOwner;
    }

    [Rpc(SendTo.Server)]
    public void SpawnServerRpc()
    {
        if (gunScript != null)
        {
            gunScript.SpawnBall();
        }
    }

    public void SpawnGun()
    {
        if(IsServer)
        {
            SpawnServerRpc();
        }
        
    }


    [ServerRpc]
    void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;

        List<ulong> targetClients = new List<ulong>();

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            // Exclure celui qui a tiré
            if (clientId != senderClientId)
            {
                targetClients.Add(clientId);
            }
        }

        ClientRpcParams rpcParamsSend = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = targetClients.ToArray()
            }
        };

        PlaySoundClientRpc(rpcParamsSend);
    }

    [ClientRpc]
    void PlaySoundClientRpc(ClientRpcParams rpcParams = default)
    {
        gunScript.SnGunPlayClient();
    }

    [Rpc(SendTo.Server)]
    void ChangeWeaponServerRpc()
    {
        waeponKnife = !waeponKnife;
        ChangeWeaponClientRpc(waeponKnife);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ChangeWeaponClientRpc(bool changeForKnife)
    {
        PlayerCustom playerCustom = GetComponent<PlayerCustom>();

        if(playerCustom != null)
        {
            playerCustom.ChangeWeapon(changeForKnife);
        }
    }

    public void DestroyPlayer()
    {
        if(IsServer)
        {
            rb.isKinematic = true;
            DestroyClientClientRpc(GetComponent<NetworkObject>().OwnerClientId);
        }
        
    }

    [Rpc(SendTo.ClientsAndHost)]
    void DestroyClientClientRpc(ulong idClient)
    {
        Debug.Log("Id Client : " + idClient);
        if(idClient == NetworkManager.Singleton.LocalClientId && !isDestroy)
        {
            isDestroy = true;
            SpawnCharacter.spawnCharacter.ReSpawn();
            this.enabled = false;
            cinemachine.enabled = false;
            colliderManager.OffCollider();
            GetComponent<DestroyNetwork>().Destroy(2.5f);

        }
    }

    public void KnifeDegat()
    {
        if(IsServer)
        {
            KnifeOnDegat = !KnifeOnDegat;
            knifeScript.OffAndOnBoxCollider(KnifeOnDegat);
        }
    }
}
