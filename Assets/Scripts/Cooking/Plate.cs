using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.TryGetComponent<Food>(out Food food))
        {
            Debug.Log("Setting as parent");
            // Only set the food as a child if it's on top of the plate
            if (other.transform.position.y > gameObject.transform.position.y)
            {
                // make the other object the child of this one
                other.transform.SetParent(gameObject.transform);
            }
        }
    }
}
