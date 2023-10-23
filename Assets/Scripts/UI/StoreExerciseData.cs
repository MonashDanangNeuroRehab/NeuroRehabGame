using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreExerciseData : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnSaveButtonPressed()
    {
        GetComponent<ExerciseData>().StoreTableData();
        // You can then switch to your next scene if desired.
    }
}
