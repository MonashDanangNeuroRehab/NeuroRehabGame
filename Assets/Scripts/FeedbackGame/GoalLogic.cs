using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalLogic : MonoBehaviour
{
    private GameObject GameManager;

    // Variables for testing
    private WaitForFixedUpdate waitTime = new WaitForFixedUpdate();
    private float encounteredTime;
    private float currTime;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");
    }

    private void OnTriggerEnter(Collider other)
    {
        GameManager.GetComponent<FeedbackGameManager>().goalEncountered = true;
        encounteredTime = Time.realtimeSinceStartup;
        currTime = Time.realtimeSinceStartup;
        StartCoroutine(TestCoroutine());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator TestCoroutine()
    {
        while (currTime - encounteredTime <= 5f)
        {
            currTime = Time.realtimeSinceStartup;
            yield return waitTime;
        }
        gameObject.SetActive(false);
        GameManager.GetComponent<FeedbackGameManager>().goalEncountered = false;
        yield break;
    }
}
