using UnityEngine;
using System.Collections;

public class PrisonTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject FinishUI;

    [Header("Door Settings")]
    public Transform rotatingObject;
    public float rotateSpeed = 90f; // derajat per detik

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (ScoreManager.Instance != null && ScoreManager.Instance.CurrentKey >= 3)
            {
                hasTriggered = true;
                StartCoroutine(RotateThenFinish());
            }
        }
    }

    private IEnumerator RotateThenFinish()
    {
        if (rotatingObject == null)
        {
            Debug.LogError("rotatingObject belum di-assign di Inspector!");
            yield break;
        }

        Quaternion startRot = rotatingObject.rotation;
        Quaternion targetRot = Quaternion.Euler(-90f, 150f, 0f);

        float angleDiff = Quaternion.Angle(startRot, targetRot);

        float duration = angleDiff / rotateSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rotatingObject.rotation = Quaternion.Lerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rotatingObject.rotation = targetRot;

        // delay 2 detik sebelum UI muncul
        yield return new WaitForSeconds(1f);

        if (FinishUI != null)
            FinishUI.SetActive(true);
    }
}
