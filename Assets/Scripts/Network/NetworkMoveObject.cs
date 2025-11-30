using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

public class NetworkMoveObject : NetworkBehaviour
{
    [SerializeField] private Rigidbody rigidbody3D;
    [SerializeField] private float moveDistance = 3f; // jarak kanan-kiri
    [SerializeField] private float moveSpeed = 2f;    // kecepatan gerak
    [SerializeField] private Vector3 moveDirection = Vector3.right; // arah gerak

    private Vector3 startPos;

    public override void Spawned()
    {
        startPos = transform.position;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        // Hitung posisi target bolak-balik
        float offset = Mathf.Sin(Runner.SimulationTime * moveSpeed) * moveDistance;
        Vector3 targetPos = startPos + moveDirection.normalized * offset;

        // Pindahkan dengan Rigidbody kalau ada
        if (rigidbody3D != null)
        {
            rigidbody3D.MovePosition(targetPos);
        }
        else
        {
            transform.position = targetPos;
        }
    }
}
