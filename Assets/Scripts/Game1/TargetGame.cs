using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Leap.Unity.Examples;

public class TargetGame : MonoBehaviour
{
    public GameObject[] TargetList;
    public GameObject GameGoal;
    public bool TargetReached = false;
    // List of Color displayed on the target
    private Color[] ColorList = { Color.red, Color.blue, Color.green, Color.magenta, Color.yellow, Color.cyan };
    private WaitForFixedUpdate updateInterval = new WaitForFixedUpdate();
    private bool GameWon = false;
    private TMP_Text GameNotif;
    private int nextGoalIndex = 0;
    public int gameState = 0;
    public const int REACH_TARGET = 0;
    public const int BACK_OFF = 1;
    [SerializeField]
    private GameObject GameUI;
    
    // Start is called before the first frame update
    void Start()
    {
        TargetList = GameObject.FindGameObjectsWithTag("Target");
        GameNotif = GameObject.Find("GameNotif").GetComponent<TextMeshPro>();
        // Shuffle the list at the start
        ShuffleColorList();
        // Assign random color
        for (int i = 0; i < ColorList.Length; i++)
        {
            // Debug.Log(TargetList[i].name);
            // Debug.Log(ColorList[i]);
            TargetList[i].GetComponent<Renderer>().material.color = ColorList[i];
        }
        // Shuffle once again to assign a random goal
        ShuffleColorList();
        GameGoal.GetComponent<Renderer>().material.color = ColorList[nextGoalIndex];
        StartCoroutine(CheckEndGameCondition());
    }
    // Color List Shuffle
    private void ShuffleColorList()
    {
        Color tempValue;
        for (int i = 0; i < ColorList.Length; i++)
        {
            int rnd = Random.Range(i, ColorList.Length);
            tempValue = ColorList[rnd];
            ColorList[rnd] = ColorList[i];
            ColorList[i] = tempValue;
        }
    }
    IEnumerator CheckEndGameCondition()
    {
       while(!GameWon)
       {
            if (TargetReached)
            {
                TargetReached = false;
                nextGoalIndex++;
                if (nextGoalIndex >= TargetList.Length)
                {
                    nextGoalIndex = 0;
                    GameWon = true;
                    
                }
                else
                {
                    Debug.Log(nextGoalIndex);
                    GameGoal.GetComponent<Renderer>().material.color = ColorList[nextGoalIndex];
                }
            }
            if (GameWon)
            {
                GameGoal.SetActive(false);
                GameNotif.text = "Congrats! You have completed the game";
            }
            yield return updateInterval;
       }
    }
}
