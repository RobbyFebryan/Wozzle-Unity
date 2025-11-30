using UnityEngine;

public class LimbCollision : MonoBehaviour
{
    public PlayerController playerController;

    private void Start()
    {
        playerController = GameObject.FindAnyObjectByType<PlayerController>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        playerController.isGrounded = true;
    }

    /*private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            playerController.isGrounded = false;
        }
    }*/
}