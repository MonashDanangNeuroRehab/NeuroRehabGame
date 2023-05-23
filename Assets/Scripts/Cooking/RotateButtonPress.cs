using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateButtonPress : MonoBehaviour
{
    private Button button;
    private RotateCamera rotateCameraScript;
    public float direction;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        rotateCameraScript = GameObject.Find("Main Camera").GetComponent<RotateCamera>();
        button.onClick.AddListener(SetRotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetRotation()
    {
        rotateCameraScript.RotateAngle(direction);
    }
}
