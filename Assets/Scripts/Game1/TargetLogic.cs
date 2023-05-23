using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLogic : MonoBehaviour
{
    private TargetGame GameManager;
    private GameObject GameGoal;
    [SerializeField]
    private GameObject Target;
    [SerializeField]
    private void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<TargetGame>();
        GameGoal = GameObject.Find("GameGoal");
        Target = gameObject.transform.parent.gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Target.GetComponent<Renderer>().material.color == GameGoal.GetComponent<Renderer>().material.color && GameManager.TargetReached is false)
        {
            Debug.Log("Target Reached");
            GameManager.TargetReached = true;
        }
    }
}
