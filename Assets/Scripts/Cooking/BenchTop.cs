using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchTop : MonoBehaviour
{
    Vector3 currentPos;
    public GameObject food;
    // Start is called before the first frame update
    void Start()
    {
        currentPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnMouseDown()
    {
        Instantiate(food, currentPos + new Vector3(0f, 0f, 0f), Quaternion.identity);
    }
}
