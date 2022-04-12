using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackGameManager : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject[] goals;
    private int goalIndex;
    private GameObject nextGoal;
    [SerializeField]
    private GameObject scoreDisplay;
    [SerializeField]
    private GameObject bottomWall;
    [SerializeField]
    private GameObject topWall;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 1f;
    public bool goalEncountered = false;
    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        goals = GameObject.FindGameObjectsWithTag("Goal");
        foreach (GameObject goal in goals)
        {
            if (goal.name == "Goal" + (goalIndex + 1))
            {
                nextGoal = goal;
                break; 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Move Bottom wall forward untill the first obstable is met
        if (goalEncountered is false)
        {
            bottomWall.transform.position = Vector3.SmoothDamp(bottomWall.transform.position, nextGoal.transform.position, ref velocity, smoothTime);
        }
        else
        {
            goalIndex++;
            foreach (GameObject goal in goals)
            {
                if (goal.name == "Goal" + (goalIndex + 1))
                {
                    nextGoal = goal;
                    break;
                }
                else
                {
                    nextGoal = topWall;
                }
            }
        }

    }
}
