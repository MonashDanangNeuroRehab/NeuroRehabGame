using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExerciseData : MonoBehaviour
{
    public List<string> Hand = new List<string>();
    public List<string> Joint = new List<string>();
    public List<string> ExerciseType = new List<string>();
    public List<string> NoSets = new List<string>();
    public List<string> SetDuration = new List<string>();
    public List<string> MotionDuration = new List<string>();
    public List<string> SetRestTime = new List<string>();

    public GameObject Table;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void StoreTableData()
    {
        for (int i = 1; i < 6; i++)
        {
            Hand.Add(Table.transform.Find("ColHand").GetChild(i).GetComponent<InputField>().text);
            Joint.Add(Table.transform.Find("ColJoint").GetChild(i).GetComponent<InputField>().text);
            ExerciseType.Add(Table.transform.Find("ColExerciseType").GetChild(i).GetComponent<InputField>().text);
            NoSets.Add(Table.transform.Find("ColNoSets").GetChild(i).GetComponent<InputField>().text);
            SetDuration.Add(Table.transform.Find("ColSetDur").GetChild(i).GetComponent<InputField>().text);
            MotionDuration.Add(Table.transform.Find("ColMotionDur").GetChild(i).GetComponent<InputField>().text);
            SetRestTime.Add(Table.transform.Find("ColRestDur").GetChild(i).GetComponent<InputField>().text);
        }
    }
}
