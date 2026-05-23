using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager input;

    public Joystick joystickMove;
    public Joystick joystickLook;
    public ButtonAction joystickShoot;

    private void Awake() {

        if(input == null)
        {
            input = this;
        }
        else
            transform.gameObject.SetActive(false);
    }

}
