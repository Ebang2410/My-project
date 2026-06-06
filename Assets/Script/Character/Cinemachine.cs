using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System;

#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class Cinemachine : NetworkBehaviour {

    #if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
    #endif
    InputStarted _input;
    [SerializeField]
    Joystick joystick;
    [SerializeField]
    ButtonAction joystickShoot;

    [SerializeField]
    GameObject CamShoot; 
    const float _threshold = 0.01f;
    float _currentYaw;
    float _yawVelocity;
   
    [Header("Auto lock")]
    [SerializeField ] float lockRaduis;
    [Range(15f,90f)] 
    [SerializeField] float lockFov;
    [Range(1f,20f)]
    [SerializeField] float lockbodySpeed;
    [SerializeField] float unlockDelay = 0.8f;
    public Transform _lockTarget;
    float _unlockDelay;
    [SerializeField] bool isShooting;

    private void Start() {
        
        if(CamShoot == null)
            this.enabled = false;

        CamShoot.SetActive(true);
        
        if(IsOwner)
        {
            joystick = InputManager.input.joystickLook;
            joystickShoot = InputManager.input.joystickShoot;
            _input = GetComponent<InputStarted>();
            CamShoot.SetActive(true);

            if(joystick == null || joystickShoot == null)
            {
                Debug.Log(" joystick look absent");
                this.enabled = false;
            }
        }
        else
        {
            CamShoot.SetActive(false);

            this.enabled = false;
        }
    }

    [Header("Cinemachine")]
    public GameObject CinemachineCameraTarget;

    public float TopClamp = 70.0f;

    public float BottomClamp = -30.0f;

    public float CameraAngleOverride = 30.0f;

    // cinemachine
    private float _cinemachineTargetYaw;
    [Range(1,200)]
    public int delta = 100;
    [Range(0,1)]
    [SerializeField] float speedShoot;
    [Range(.01f,.9f)][SerializeField] float smoothTime;


    private void LateUpdate() {
        if(!IsOwner) return;

        //if(!isShooting)
            CameraRotation();
        UpdateLock();
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if ( joystick.Direction.sqrMagnitude >= _threshold || joystickShoot.Direction.sqrMagnitude >= _threshold)
        {
  

            Vector2 _look = Vector2.zero;
            if(joystick.Direction.sqrMagnitude >= _threshold)
            {
                _look = joystick.Direction;
            }
            else
            {
                _look = joystickShoot.Direction * speedShoot;
            }
            //Don't multiply mouse input by Time.deltaTime;

            _cinemachineTargetYaw += _look.x * delta * Time.deltaTime ;
            //_cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        //_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        _currentYaw = Mathf.SmoothDampAngle(_currentYaw,_cinemachineTargetYaw, ref _yawVelocity, smoothTime);
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(CameraAngleOverride,
            _currentYaw, 0.0f);
    }

    void UpdateLock()
    {
        if(isShooting)
        {
            _unlockDelay = unlockDelay;
            if(_lockTarget == null)
            {
                _lockTarget = FindBestTarget();
            }
            else if(!IsTargetValid(_lockTarget))
                _lockTarget = FindBestTarget();
        }
        else
        {
            if(_lockTarget != null)
            {
                _unlockDelay -= Time.deltaTime;
                if(_unlockDelay <= 0f)
                    _lockTarget = null;
            }
        }

        if(_lockTarget != null)
            RotateBodyToTarget();
    }

    private void RotateBodyToTarget()
    {
        Vector3 dir = _lockTarget.position - CinemachineCameraTarget.transform.position;
        dir.y = 0;
        if(dir.sqrMagnitude < .001f) return;
        float targetYaw = Mathf.Atan2(dir.x,dir.z) * Mathf.Rad2Deg;
        _cinemachineTargetYaw = Mathf.SmoothDampAngle(_cinemachineTargetYaw,targetYaw,ref _yawVelocity,1/lockbodySpeed);

        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(CameraAngleOverride,_cinemachineTargetYaw,0);                                        
    }

    private bool IsTargetValid(Transform lockTarget)
    {
        if(lockTarget == null || !lockTarget.gameObject.activeInHierarchy) return false;

        float dist  =Vector3.Distance(transform.position, lockTarget.position);

        return dist <= lockRaduis;
    }

    private Transform FindBestTarget()
    {
       Collider[] colliders = Physics.OverlapSphere(transform.position,lockRaduis);
       Transform best = null;
       float bestDist = float.MaxValue;
       float halfFov = lockFov * .5f;

       foreach (var item in colliders)
       {
            if(!item.CompareTag("Enemy")) continue;
            if(item.transform.root == transform.root) continue;

            Vector3 toEnemy = item.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, toEnemy);

            if(angle > halfFov) continue;

            float dist = toEnemy.sqrMagnitude;
            if(dist < bestDist)
            {
                bestDist = dist;
                best = item.transform;
            }
       }

       return best;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}

        