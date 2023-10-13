using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExerciseManager : MonoBehaviour
{
    public GameObject ObjectDetector;
    public List<string> exerciseJoints; // What joint is being exercised for this one
    public List<string> exerciseTypes; // What is the type of exercise 
    public List<int> exerciseSets; // Number of sets
    public List<float> exerciseSetTimes; // Seconds
    public List<float> exerciseRestTimes; // Seconds
    public List<float> exerciseMovementDurations; // Seconds
    public bool exerciseFinished;
    public string hand;
    public bool doingExercise;

    // Bones and fingers for hand data measuring
    public int baselineFingerNo;
    public int boneNo;
    public int nextToFingerNo;
    public float currentSetActivityDuration;
    public float currentSetRestDuration;
    public float currentSetMovementDuration;

    ObjectDetection objectDetectionScript;



    // Start is called before the first frame update
    void Start()
    {
        objectDetectionScript = ObjectDetector.GetComponent<ObjectDetection>();

        StartCoroutine(ExerciseRoutine());
    }

    private IEnumerator ExerciseRoutine()
    {
        if (exerciseJoints.Count > 0)
        {

            for (int exerciseNo = 0; exerciseNo < exerciseJoints.Count; exerciseNo += 1)
            {
                Debug.Log("Exercise No: " + exerciseNo.ToString());
                // Perform a lookup of the joint
                switch(exerciseJoints[exerciseNo].ToUpper())
                {
                    case "WRIST":
                        baselineFingerNo = 1;
                        boneNo = 0;
                        nextToFingerNo = 2;
                        break;
                    // Add more joints here...
                }

                // Exercise type lookup
                switch (exerciseTypes[exerciseNo].ToUpper())
                {
                    case "EXTENSION/FLEXION":
                        // don't need to actually do anything yet, if I do the other movements I will need to though
                        break;
                }

                currentSetActivityDuration = exerciseSetTimes[exerciseNo];
                Debug.Log($"Current activity set duration is: {currentSetActivityDuration}");
                currentSetRestDuration = exerciseRestTimes[exerciseNo];
                Debug.Log($"Current activity set rest times are: {currentSetRestDuration}");
                currentSetMovementDuration = exerciseMovementDurations[exerciseNo];
                Debug.Log($"Current activity set movement durations are: {currentSetMovementDuration}");

                // Do for the number of sets
                for (int exerciseSetNo = 0; exerciseSetNo < exerciseSets[exerciseNo]; exerciseSetNo += 1)
                {
                    // Logic already exists for repeating exercises
                    // Set the objectDetector game object to Active
                    exerciseFinished = false;
                    Debug.Log($"Exercise Set No: {exerciseSetNo}");

                    ObjectDetector.SetActive(true);
                    Debug.Log("Set the object detector ON");

                    
                    
                    while (!exerciseFinished)
                    {
                        yield return null; // This will wait for the next frame
                    }

                    ObjectDetector.SetActive(false);
                    Debug.Log("Set the object detector off");
                }
            }
        }
    }

}

