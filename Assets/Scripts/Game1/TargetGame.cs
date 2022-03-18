using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetGame : MonoBehaviour
{
    public GameObject[] TargetList;
    public GameObject GameGoal;
    public bool TargetReached = false;
    // List of Color displayed on the target
    private Color[] ColorList = { Color.red, Color.blue, Color.green, Color.magenta, Color.yellow, Color.cyan };
    private WaitForSeconds updateInterval = new WaitForSeconds(0.5f);
    private bool GameWon = false;
    private TMP_Text GameNotif;
    private int nextGoalIndex = 0;
    public int gameState = 0;
    public const int REACH_TARGET = 0;
    public const int BACK_OFF = 1;
    // Start is called before the first frame update
    void Start()
    {
        TargetList = GameObject.FindGameObjectsWithTag("Target");
        GameNotif = GameObject.Find("GameNotif").GetComponent<TextMeshPro>();
        GameGoal = GameObject.Find("Goal");
        // Shuffle the list at the start
        ShuffleColorList();
        // Assign random color
        for (int i = 0; i < ColorList.Length; i++)
        {
            TargetList[i].GetComponent<Renderer>().material.color = ColorList[i];
        }
        // Shuffle once again to assign a random goal
        ShuffleColorList();
        GameGoal.GetComponent<Renderer>().material.color = ColorList[nextGoalIndex];
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
            if (TargetReached && gameState == 0)
            {
                GameNotif.text = "Please move back to the starting position";
                GameGoal.SetActive(false);
                gameState = 1;
            }
            else if (gameState == 1)
            {
                TargetReached = false;
                GameNotif.text = "Can you reach this target?";
                GameGoal.SetActive(true);
                nextGoalIndex++;
                if (nextGoalIndex >= TargetList.Length)
                {
                    nextGoalIndex = 0;
                    GameWon = true;
                }
                else
                {
                    GameGoal.GetComponent<Renderer>().material.color = ColorList[nextGoalIndex];
                }
                gameState = 0;
            }
            if (GameWon)
            {
                GameNotif.text = "Congrats! You have completed the game";
            }
            yield return updateInterval;
       }
    }
}
