using UnityEngine;
using UnityEngine.InputSystem;

public class InputStarted : MonoBehaviour
{
    public Vector2 move;
    public bool fire;
    public bool spring;
    public bool crouch;
    public bool interaction;
    public bool run;
    public Vector2 look;

    void OnMove(InputValue val)
    {
        move = val.Get<Vector2>();
    }

    void OnAttack(InputValue val)
    {
        fire = val.isPressed;
    }

    void OnLook(InputValue val)
    {
        look = val.Get<Vector2>();
    }

    void OnSprint(InputValue val)
    {
        run = val.isPressed;
    }

    void OnCrouch(InputValue val)
    {
        crouch = val.isPressed;
    }

    void OnInteract(InputValue val)
    {
        interaction = val.isPressed;
    }
}
