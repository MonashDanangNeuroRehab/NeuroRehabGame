using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshPro scoreDisplay;
    private Camera mainCamera;
    private bool isDisplayOn = true;
    private float score;
    private Vector3 currEulerAngles;
    private Vector3 rotationFix = new Vector3(45, 180, 0);
    private void Awake()
    {
        scoreDisplay = GetComponent<TextMeshPro>();
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        transform.LookAt(mainCamera.transform, Vector3.forward);
        currEulerAngles = transform.eulerAngles;
        currEulerAngles += rotationFix;
        transform.eulerAngles = currEulerAngles;
    }
    public void UpdateScore(float val)
    {
        score = val;
    }
    public void ToggleScoreDisplay()
    {
        if (isDisplayOn)
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
