using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.Burst.Intrinsics;


public class PlayerScript : NetworkBehaviour
{
    //Component
    Rigidbody rb;
    Animator animator;
    AudioSource audioSource;
    InputStarted inputStarted;
    ColliderManager colliderManager;
    GestionPointSpawn gestionPointSpawn;
    AudioManagerCharacter audioManagerCharacter;
    Cinemachine cinemachine;

    [Header("Joystick")]
    [SerializeField]    Joystick joystickMove;
    [SerializeField]    ButtonAction buttonAction;

    [Header(" Info Move")]
    public Arme arme;
    NetworkVariable <bool> isCrouch = new NetworkVariable<bool>(false);//permet de connaitre si le jouer est accroupi ou debou
    NetworkVariable<int> ValueModeMove = new NetworkVariable<int>(0);//type de placement 0 walk, 1 crouch , 2 run
    Transform cam;
    float _timeAttack;//temps d'attente pour la prochaine attack
    Vector3 move3D = Vector3.zero;
    
    [Header("Arme")]
    public TypeArme typeArme;
    public GunScript gunScript;
    public KnifeScript knifeScript;
    [SerializeField] bool KnifeOnDegat;
    public bool OnDegat;
    bool waeponKnife;
    bool isDestroy;
    
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

        ValueModeMove.OnValueChanged += ChangeMoveMode;

        TryGetComponent(out rb);
        TryGetComponent(out colliderManager);
        TryGetComponent(out audioSource);
        TryGetComponent(out cinemachine);
        TryGetComponent(out audioManagerCharacter);
        TryGetComponent(out animator);

        if (IsOwner)
        {
            TryGetComponent(out inputStarted);
            cam = GameObject.FindWithTag("MainCamera").transform;

            joystickMove = InputManager.input.joystickMove;
            buttonAction = InputManager.input.joystickShoot;

            cinemachine.enabled = true;

            GetComponent<PlayerInput>().enabled = true;

            if(arme != null)
            {
                AnimeModeRpc(typeArme);
            }
            
            
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
        {
            rb.isKinematic = false;
        }

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

            if ((inputStarted.fire || buttonAction.fire) && _timeAttack <= 0.0f)
            {
                _timeAttack = arme.timeNextBall;

                AttackRpc();
            }
            else
            {
                _timeAttack -= Time.fixedDeltaTime;
            }

            if (inputStarted.move != Vector2.zero || joystickMove.Direction != Vector2.zero)
            {
                ActArme = 1;
                Vector2 move2d = joystickMove.Direction != Vector2.zero ? joystickMove.Direction : inputStarted.move;

                move3D = new Vector3(move2d.x, 0, move2d.y);
                yAnimation = move2d.y; 
                xAnimation = move2d.y < -.4f? -move2d.x : move2d.x;
                
                if (!isCrouch.Value && !inputStarted.fire)
                {
                    ChangeMoveModeServerRpc(2);
                    speedMove = 1;
                    if (move3D.z >= -0.4f)
                        speed = arme.run + 2f;
                    else if (move3D.z < -0.4f)
                        speed = arme.run;
                }
                else
                {
                    if (isCrouch.Value == true)
                    {
                        ChangeMoveModeServerRpc(1);
                    }
                    else
                    {
                        ChangeMoveModeServerRpc(0);
                    }

                    speedMove = 0.5f;

                    if( move3D.z >= -0.4f)
                        speed = arme.walk + 1;
                    else if (move3D.z < -0.4f)
                        speed = arme.walk;
                }
            }
            else
            {
                move3D = Vector3.zero;
            }

            AnimeMoveRpc(ActArme, isCrouch.Value, speedMove, xAnimation, yAnimation);
            MoveServerRpc(move3D, cam.eulerAngles.y, speed);
        }
    }

    //Collider
    [Rpc(SendTo.Server)]
    void CrouchServerRpc()
    {
        isCrouch.Value = !isCrouch.Value ;
    }

    void CrouchChange(bool previousValue,bool newValue)
    {
        gestionPointSpawn.CrouchChange(newValue);
    }

    //audio de marche 
    [Rpc(SendTo.Server)]
    void ChangeMoveModeServerRpc(int mode)
    {
        ValueModeMove.Value = mode;
    }

    void ChangeMoveMode(int previousValue,int newValue)
    {
        if(audioManagerCharacter != null)
            audioManagerCharacter.ValueModeMove = newValue ;
    }

    //Animation Attack
    [Rpc(SendTo.Server)]
    void AttackRpc()
    {
        animator.SetTrigger("attack");
    }

    [Rpc(SendTo.Server)]
    public void ResetShootTriggerServerRpc()
    {
        animator.ResetTrigger("attack");
    }

    //Animation de deplacement
    [Rpc(SendTo.Server)]
    public void AnimeModeRpc(TypeArme armeAnimation)
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

    [Rpc(SendTo.Server)]
    void AnimeMoveRpc(float ActArme, bool crouch, float speedMove, float x, float y)
    {
        animator.SetFloat("positionX", x);
        animator.SetFloat("positionY", y);
        animator.SetFloat("speed", speedMove);
        animator.SetFloat("arme", ActArme);
        animator.SetBool("crouch", crouch);
    }

    // Deplacement dans le monde
    [Rpc(SendTo.Server)]
    public void MoveServerRpc(Vector3 move3DNetwork, float angleNetwork, float speedNetwork)
    {
        if (angleNetwork != 0.0f)
        {
            transform.rotation = Quaternion.Euler(0, angleNetwork, 0);
        }

        if (move3DNetwork.sqrMagnitude >= 0.01f)
            transform.Translate(move3DNetwork.normalized * speedNetwork * Time.fixedDeltaTime);
    }

    //Activation des degat du Knife
    public void Degat()
    {
        if (IsOwner)
        {
            OnDegat = !OnDegat;
        }
    }  

    public bool IsMe()
    {
        return IsOwner;
    }

    //Spawner la ball dans le reseau
    [Rpc(SendTo.Server)]
    public void SpawnServerRpc()
    {
        if (gunScript != null)
        {
            gunScript.SpawnBall();
        }
    }

    //event animator dans les animation de tire pour spawn la bal
    public void SpawnGun()
    {
        if(IsServer)
        {
            SpawnServerRpc();
        }
        
    }

    //Changer arme du fusil au couteau et vise versa
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

    //Event animator de destruction du joueur
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
