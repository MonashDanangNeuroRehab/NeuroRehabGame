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
        for (int i = 1; i <= 6; i++)
        {
            Hand.Add(GetInputFieldText($"ColHand/row{i}"));
            Joint.Add(GetInputFieldText($"ColJoint/row{i}"));
            ExerciseType.Add(GetInputFieldText($"ColExerciseType/row{i}"));
            NoSets.Add(GetInputFieldText($"ColNoSets/row{i}"));
            SetDuration.Add(GetInputFieldText($"ColSetDur/row{i}"));
            MotionDuration.Add(GetInputFieldText($"ColMotionDur/row{i}"));
            SetRestTime.Add(GetInputFieldText($"ColRestDur/row{i}"));
        }
    }

    private string GetInputFieldText(string path)
    {
        InputField inputField = Table.transform.Find(path).GetComponent<InputField>();
        if (inputField != null)
        {
            return inputField.text;
        }
        Debug.LogError($"InputField not found at path: {path}");
        return "";
    }
}
