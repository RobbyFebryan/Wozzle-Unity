using UnityEngine;
using TMPro;

public class ShowUIScore : MonoBehaviour
{
    public TMP_Text scoreUI;
    public TMP_Text scoreUIResult;

    // Update is called once per frame
    void Update()
    {
        scoreUIResult.text = scoreUI.text;
    }
}
