using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RotateAngle(float direction)
    {
        transform.Rotate(0.0f, direction * 90.0f,0.0f, Space.World);
    }
}
