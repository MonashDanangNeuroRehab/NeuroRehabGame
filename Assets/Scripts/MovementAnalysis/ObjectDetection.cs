using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using Leap.Unity;
using TMPro;

public class ObjectDetection : MonoBehaviour
{
    public GameObject ExerciseManager;
    public GameObject DetectionCube;
    public GameObject HoldStillText;
    public GameObject HandDataGatherer;
    public GameObject VectorGenerator;
    public GameObject MovementAnalyser;

    public LeapServiceProvider leapServiceProvider;
    public TextMeshProUGUI CommandText;
    private HandDataGathering handDataGatheringScript;
    private ExerciseManager exerciseManagerScript;

    public float timeObjectEntered;
    public bool recordingData;
    public bool isObjectInside;
    public Vector3 palmPos;
    public bool exerciseFinished;
    public string hand;

    // change every exercise
    public float countDownTimer;

    private Hand _hand;

    private void Start()
    {
        // Activate the DetectionCube at the start
        handDataGatheringScript = HandDataGatherer.GetComponent<HandDataGathering>();
        exerciseManagerScript = ExerciseManager.GetComponent<ExerciseManager>();
    }

    private void OnEnable()
    {
        leapServiceProvider.OnUpdateFrame += OnUpdateFrame;
        DetectionCube.SetActive(true);
        
        exerciseFinished = exerciseManagerScript.exerciseFinished;
        hand = exerciseManagerScript.hand;
        recordingData = false;
        isObjectInside = false;
    }

    void OnUpdateFrame(Frame frame)
    {
        // Get the right hand data - something off with below, doesn't like being in the if else loop?

        Hand _rightHand = frame.GetHand(Chirality.Right);
        Hand _leftHand = frame.GetHand(Chirality.Right);
        
        if (exerciseManagerScript.hand.ToUpper() == "LEFT")
        {
            if (!exerciseFinished)
            {
                palmPos = new Vector3(_leftHand.PalmPosition.x, _leftHand.PalmPosition.y, _leftHand.PalmPosition.z);

                // Check if myObject is within the DetectionCube's bounds    

                if (DetectionCube.activeSelf && DetectionCube.GetComponent<Collider>().bounds.Contains(palmPos) && !recordingData)
                {
                    if (!isObjectInside)
                    {
                        isObjectInside = true;
                        timeObjectEntered = Time.time;
                        HoldStillText.SetActive(true);
                    }

                    if (isObjectInside && Time.time - timeObjectEntered >= 1)
                    {
                        Debug.Log("Activating other objects AAA");
                        countDownTimer = exerciseManagerScript.currentSetActivityDuration;
                        recordingData = true;
                        VectorGenerator.SetActive(true);
                        // Debug.Log("Vector Generator Active");

                        MovementAnalyser.SetActive(true);
                        // Debug.Log("Movement Analyser Active");

                        HandDataGatherer.SetActive(true);
                        // Debug.Log("Hand Data Analyser Active");

                        StartCoroutine(ActivateAfterDelay());
                    }
                }
                else
                {
                    if (!recordingData)
                    {
                        isObjectInside = false;
                        HoldStillText.SetActive(false);
                    }
                }

                if (CommandText.gameObject.activeInHierarchy)
                {
                    // add in the time left
                    countDownTimer -= Time.deltaTime;
                    CommandText.text = "Wrist up and down: " + countDownTimer.ToString();
                }
            }
        }

        else
        {
            if (!exerciseFinished)
            {
                palmPos = new Vector3(_rightHand.PalmPosition.x, _rightHand.PalmPosition.y, _rightHand.PalmPosition.z);

                // Check if myObject is within the DetectionCube's bounds    

                if (DetectionCube.activeSelf && DetectionCube.GetComponent<Collider>().bounds.Contains(palmPos) && !recordingData)
                {
                    Debug.Log("Hand detected in sphere");
                    if (!isObjectInside)
                    {
                        isObjectInside = true;
                        timeObjectEntered = Time.time;
                        HoldStillText.SetActive(true);
                    }

                    if (isObjectInside && Time.time - timeObjectEntered >= 1)
                    {
                        Debug.Log("Activating other objects");
                        countDownTimer = exerciseManagerScript.currentSetActivityDuration;
                        recordingData = true;
                        VectorGenerator.SetActive(true);
                        //Debug.Log("Vector Generator Active");

                        MovementAnalyser.SetActive(true);
                        // Debug.Log("Movement Analyser Active");

                        HandDataGatherer.SetActive(true);
                        // Debug.Log("Hand Data Analyser Active");

                        StartCoroutine(ActivateAfterDelay());
                    }
                }
                else
                {
                    if (!recordingData)
                    {
                        isObjectInside = false;
                        HoldStillText.SetActive(false);
                    }
                }

                if (CommandText.gameObject.activeInHierarchy)
                {
                    // add in the time left
                    countDownTimer -= Time.deltaTime;
                    CommandText.text = "Wrist up and down: " + countDownTimer.ToString();
                }
            }
        }
    }

    IEnumerator ActivateAfterDelay()
    {
        Debug.Log("Start gathering data");
        yield return new WaitForSeconds(3);
        HoldStillText.SetActive(false);
        CommandText.gameObject.SetActive(true);

        yield return new WaitForSeconds(exerciseManagerScript.currentSetActivityDuration);
        Debug.Log("Done gathering data");
        HandDataGatherer.SetActive(false);
        Debug.Log("Data gatherer off");

        yield return new WaitForSeconds(2); // takes a little bit to do the analysis
        CommandText.gameObject.SetActive(false);
        VectorGenerator.SetActive(false);
        MovementAnalyser.SetActive(false);
        DetectionCube.SetActive(false);

        yield return new WaitForSeconds(exerciseManagerScript.currentSetRestDuration);
        recordingData = false;
        exerciseManagerScript.exerciseFinished = true;
    }
}
