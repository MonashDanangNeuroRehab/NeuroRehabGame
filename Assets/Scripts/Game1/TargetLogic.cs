using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLogic : MonoBehaviour
{
    private TargetGame GameManager;
    private GameObject GameGoal;
    private Color GoalColor;
    private Color myColor;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<TargetGame>();
        GameGoal = GameObject.Find("GameGoal");
        GoalColor = GameGoal.GetComponent<Renderer>().material.color;
        myColor = gameObject.GetComponent<Renderer>().material.color;
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (myColor == GoalColor)
        {
            gameObject.SetActive(false);
            GameManager.TargetReached = true;
        }
    }
}
