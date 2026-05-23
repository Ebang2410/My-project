using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


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
        if(IsOwner)
            CameraRotation();
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

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}

        