using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.TryGetComponent<Food>(out Food food))
        {
            // Debug.Log("Setting as parent");
            // Whichever is higher is set as the child
            if (other.transform.position.y > gameObject.transform.position.y)
            {
                // make the other object the child of this one
                other.transform.SetParent(gameObject.transform);
            }
            else
            {
                gameObject.transform.SetParent(other.transform);
            }
            
            //rigidBody.isKinematic = true;
        }
    }
}

