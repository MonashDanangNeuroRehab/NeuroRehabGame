using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackGameManager : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject[] goals;
    private int goalIndex;
    private GameObject nextGoal;
    [SerializeField]
    private GameObject bottomWall;
    [SerializeField]
    private GameObject topWall;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 1f;
    public bool goalEncountered = false;

    // Score Display
    [SerializeField]
    private GameObject gameScore;
    [SerializeField]
    private GameObject gameScoreLabel;

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

    // Score display
    void ScoreUpdate(float score)
    {
        gameScore.GetComponent<Text>().text = score.ToString("0");
    }
    void ToggleScoreDisplay()
    {
        gameScore.SetActive(!gameScore.activeInHierarchy);
        gameScoreLabel.SetActive(!gameScoreLabel.activeInHierarchy);
    }
}
