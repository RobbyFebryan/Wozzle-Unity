using UnityEngine;

public class PlayerUIButtonInput : MonoBehaviour
{
    private PlayerLuring player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerLuring>();
    }

    public void MoveLeftDown()
    {
        player.SetMoveInput(-1);
    }

    public void MoveRightDown()
    {
        player.SetMoveInput(1);
    }

    public void MoveButtonUp()
    {
        player.SetMoveInput(0);
    }

    public void JumpButtonDown()
    {
        player.OnJumpButtonDown();
    }

    public void JumpButtonUp()
    {
        player.OnJumpButtonUp();
    }

    public void GrabButtonDown()
    {
        player.OnGrabButtonDown();
    }

    public void GrabButtonUp()
    {
        player.OnGrabButtonUp();
    }

    public void MouseGrabInput()
    {
        if (Input.GetMouseButtonDown(0)) // Klik kiri = Grab mulai
        {
            GrabButtonDown();
        }

        if (Input.GetMouseButtonUp(0)) // Lepas klik = Grab berhenti
        {
            GrabButtonUp();
        }
    }

    private void Update()
    {
        MouseGrabInput();
    }
}
