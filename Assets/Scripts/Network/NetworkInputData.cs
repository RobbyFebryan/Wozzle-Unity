using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public NetworkBool isJumpPressed;
    public NetworkBool isJumpHeld;
    public NetworkBool isReviveButtonPressed;
    public NetworkBool isGrabPressed;

    public Vector3 camForward;
    public Vector3 camRight;


}

