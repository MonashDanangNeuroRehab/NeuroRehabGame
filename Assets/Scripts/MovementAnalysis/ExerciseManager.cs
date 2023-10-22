using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public List<string> hands;
    public string hand;
    public bool doingExercise;

    // Bones and fingers for hand data measuring
    public int baselineFingerNo;
    public int boneNo;
    public int nextToFingerNo;
    public float currentSetActivityDuration;
    public float currentSetRestDuration;
    public float currentSetMovementDuration;
    public string currentJoint;
    public string currentExerciseType;

    ObjectDetection objectDetectionScript;


    private void OnEnable()
    {
        // Get the data from the table
        ExerciseData data = FindObjectOfType<ExerciseData>();

        // Get the string objects first, no conversion needed
        hands = data.Hand;
        exerciseJoints = data.Joint;
        exerciseTypes = data.ExerciseType;

        // Do the conversion for the other ones
        for (int i = 0; i < data.NoSets.Count; i++)
        {
            if (data.NoSets[i] != null)
            {
                // still have some data to get. Get the data
                exerciseSets.Add(int.Parse(data.NoSets[i]));
                exerciseSetTimes.Add(float.Parse(data.SetDuration[i]));
                exerciseRestTimes.Add(float.Parse(data.SetRestTime[i]));
                exerciseMovementDurations.Add(float.Parse(data.MotionDuration[i]));
            }
            else
            {
                break;
            }
                
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        objectDetectionScript = ObjectDetector.GetComponent<ObjectDetection>();
        exerciseFinished = false;
        StartCoroutine(ExerciseRoutine());
    }

    private IEnumerator ExerciseRoutine()
    {
        if (exerciseSets.Count > 0)
        {

            for (int exerciseNo = 0; exerciseNo < exerciseSets.Count; exerciseNo += 1)
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
                    // Knuckle Joints
                    case "INDEX KNUCKLE":
                        baselineFingerNo = 1;
                        boneNo = 1;
                        nextToFingerNo = 2;
                        break;
                    case "MIDDLE KNUCKLE":
                        baselineFingerNo = 2;
                        boneNo = 1;
                        nextToFingerNo = 3;
                        break;
                    case "RING KNUCKLE":
                        baselineFingerNo = 3;
                        boneNo = 1;
                        nextToFingerNo = 4;
                        break;
                    case "PINKY KNUCKLE":
                        baselineFingerNo = 4;
                        boneNo = 1;
                        nextToFingerNo = 3;
                        break;
                    //Finger Joints
                    case "INDEX FINGER":
                        baselineFingerNo = 1;
                        boneNo = 2;
                        nextToFingerNo = 2;
                        break;
                    case "MIDDLE FINGER":
                        baselineFingerNo = 2;
                        boneNo = 2;
                        nextToFingerNo = 3;
                        break;
                    case "RING FINGER":
                        baselineFingerNo = 3;
                        boneNo = 2;
                        nextToFingerNo = 4;
                        break;
                    case "PINKY FINGER":
                        baselineFingerNo = 4;
                        boneNo = 2;
                        nextToFingerNo = 3;
                        break;
                }
                hand = hands[exerciseNo];
                currentExerciseType = exerciseTypes[exerciseNo];
                currentJoint = exerciseJoints[exerciseNo];
                currentSetActivityDuration = exerciseSetTimes[exerciseNo];
                Debug.Log($"Current activity set duration is: {currentSetActivityDuration}");
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

        // exercises finished, go back to main menu now
        SceneManager.LoadScene(0);
    }

}

