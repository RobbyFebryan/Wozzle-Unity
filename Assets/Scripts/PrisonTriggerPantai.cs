using UnityEngine;
using System.Collections;

public class PrisonTriggerPantai : MonoBehaviour
{
    [Header("UI")]
    public GameObject FinishUI;

    [Header("Door Settings")]
    public Transform doorObject;

    [Tooltip("Berapa meter pintu naik ke atas (positif = naik, negatif = turun)")]
    public float moveDistance = 5f;

    [Tooltip("Kecepatan gerakan pintu (unit per detik)")]
    public float moveSpeed = 2f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (ScoreManager.Instance != null && ScoreManager.Instance.CurrentKey >= 3)
            {
                hasTriggered = true;
                StartCoroutine(MoveDoorThenFinish());
            }
        }
    }

    private IEnumerator MoveDoorThenFinish()
    {
        if (doorObject == null)
        {
            Debug.LogError("doorObject belum di-assign di Inspector!");
            yield break;
        }

        Vector3 startPos = doorObject.position;

        // Kunci rotasi awal (tidak boleh berubah)
        Quaternion lockedRotation = doorObject.rotation;

        Vector3 targetPos = new Vector3(
            startPos.x,
            startPos.y + moveDistance,
            startPos.z
        );

        float elapsed = 0f;
        float duration = Mathf.Abs(moveDistance) / moveSpeed;

        while (elapsed < duration)
        {
            // Gerakkan pintu
            doorObject.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);

            // Kunci rotasi agar tidak berubah
            doorObject.rotation = lockedRotation;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Pastikan posisi dan rotasi benar
        doorObject.position = targetPos;
        doorObject.rotation = lockedRotation;

        yield return new WaitForSeconds(1f);

        if (FinishUI != null)
            FinishUI.SetActive(true);
    }
}
