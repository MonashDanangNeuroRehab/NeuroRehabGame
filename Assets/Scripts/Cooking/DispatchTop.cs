using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispatchTop : MonoBehaviour
{
    /*  
    PURPOSE: To check food and "send out" food after it has been sitting on 
    the dispatch area for more than a set time.

    How it works:

    */
    // Start is called before the first frame update
    public float waitTime;
    private float holdCounter;
    // public foodRequestScript; - this needs to exist for later work (checking)

    void Start()
    {
        waitTime = 10.0f;
        holdCounter = 0.0f;
    }

    // Update is called once per frame

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Food>(out Food food1))
        {
            holdCounter += Time.deltaTime;

            if ((holdCounter > waitTime) && (other.TryGetComponent<Food>(out Food food2)))
            {
                Destroy(other.gameObject);
                holdCounter = 0.0f;
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        // need to reset the timer
        holdCounter = 0.0f;
    }

    private void CheckFood()
    {
        // function here should check the food against what it should be. Not sure how to code this just yet.
    }
}

    