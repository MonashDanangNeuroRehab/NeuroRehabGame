using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalLogic : MonoBehaviour
{
    private GameObject GameManager;

    // Variables for testing
    private WaitForFixedUpdate waitTime = new WaitForFixedUpdate();
    private float encounteredTime;
    private float _currTime;
    private float _finishedGameTime;
    public bool gameFinished = false;
    private int _finishingStage = 3;
    private const int CONGRATS_MESSAGE = 1;
    private const int WAIT_FOR_COLLAPSE = 2;
    private const int FINISHED = 0;
    private const int START_SEQUENCE = 3;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");
    }

    private void OnTriggerEnter(Collider other)
    {
        GameManager.GetComponent<FeedbackGameManager>().goalEncountered = true;
        encounteredTime = Time.realtimeSinceStartup;
        _currTime = Time.realtimeSinceStartup;
        
        if (TryGetComponent(out RehabMiniGame1 game1))
        {
            Debug.Log("Game 1 found");
            game1.enabled = true;
        }
        else if (TryGetComponent(out RehabMiniGame2 game2))
        {
            Debug.Log("Game 2 found");
            game2.enabled = true;
        }
        else if (TryGetComponent(out RehabMiniGame3 game3))
        {
            Debug.Log("Game 3 found");
            game3.enabled = true;
        }
        else if (TryGetComponent(out RehabMiniGame4 game4))
        {
            Debug.Log("Game 4 found");
            game4.enabled = true;
        }
        else if (TryGetComponent(out RehabMiniGame5 game5))
        {
            Debug.Log("Game 5 found");
            game5.enabled = true;
        }
        
        // StartCoroutine(TestCoroutine());
    }
    private void Update()
    {
        if (gameFinished)
        {
            switch(_finishingStage)
            {
                case START_SEQUENCE:
                    _finishedGameTime = Time.realtimeSinceStartup;
                    _finishingStage = CONGRATS_MESSAGE;
                    Debug.Log("Start Congrats");
                    break;
                case CONGRATS_MESSAGE:
                    if (Time.realtimeSinceStartup - _finishedGameTime > 1.0f)
                    {
                        Debug.Log("Start Collapsing");
                        _finishingStage = WAIT_FOR_COLLAPSE;
                        _finishedGameTime = Time.realtimeSinceStartup;
                        if (TryGetComponent(out RehabMiniGame1 game1_1))
                        {
                            game1_1.gameNotifText.text = "Prepare to move to the next obstacle";
                        }
                        else if (TryGetComponent(out RehabMiniGame2 game2_1))
                        {
                            game2_1.gameNotifText.text = "Prepare to move to the next obstacle";
                        }
                        else if (TryGetComponent(out RehabMiniGame3 game3_1))
                        {
                            game3_1.gameNotifText.text = "Prepare to move to the next obstacle";
                        }
                        else if (TryGetComponent(out RehabMiniGame4 game4_1))
                        {
                            game4_1.gameNotifText.text = "Prepare to move to the next obstacle";
                        }
                        else if (TryGetComponent(out RehabMiniGame5 game5_1))
                        {
                            game5_1.gameNotifText.text = "Prepare to move to the next obstacle";
                        }
                    }
                    break;
                case WAIT_FOR_COLLAPSE:

                    if (Time.realtimeSinceStartup - _finishedGameTime > 3.0f)
                    {
                        Debug.Log("Going to Finished");
                        _finishingStage = FINISHED;
                    }
                    break;
                case FINISHED:
                    gameFinished = false;
                    if (TryGetComponent(out RehabMiniGame1 game1))
                    {
                        game1.gameNotifText.text = "Moving toward the next goal";
                    }
                    else if (TryGetComponent(out RehabMiniGame2 game2))
                    {
                        game2.gameNotifText.text = "Moving toward the next goal";
                    }
                    else if (TryGetComponent(out RehabMiniGame3 game3))
                    {
                        game3.gameNotifText.text = "Moving toward the next goal";
                    }
                    else if (TryGetComponent(out RehabMiniGame4 game4))
                    {
                        game4.gameNotifText.text = "Moving toward the next goal";
                    }
                    else if (TryGetComponent(out RehabMiniGame5 game5))
                    {
                        game5.gameNotifText.text = "Moving toward the next goal";
                    }
                    gameObject.SetActive(false);
                    GameManager.GetComponent<FeedbackGameManager>().goalEncountered = false;
                    _finishingStage = START_SEQUENCE;
                    break;
                default:
                    break;
            }
        }
        
    }
    // Update is called once per frame
    IEnumerator TestCoroutine()
    {
        while (_currTime - encounteredTime <= 3f)
        {
            _currTime = Time.realtimeSinceStartup;
            yield return waitTime;
        }
        gameObject.SetActive(false);
        GameManager.GetComponent<FeedbackGameManager>().goalEncountered = false;
        gameFinished = true;
        yield break;
    }

}